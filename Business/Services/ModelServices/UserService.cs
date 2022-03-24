using System.Linq.Expressions;
using Business.Dto.Frontend.FromForm;
using Business.Interfaces.Model;
using DataAccess.Dto;
using DataAccess.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Services.ModelServices;

public class UserService : IUserService
{
    private readonly IGeneralRepository<User> _userRepository;

    public UserService(
        IGeneralRepository<User> userRepository
    )
    {
        _userRepository = userRepository;
    }

    public async Task<User> LoginOrRegisterAsync(User user, CancellationToken token)
    {
        return await _userRepository.AddIfNotExistAsync(u => u.SocialId == user.SocialId, user, token);
    }

    public async Task<User> UpdateAsync(User user, CancellationToken token)
    {
        return await _userRepository.UpdateAsync(user, token);
    }

    public async Task<User?> GetUserBySocialIdAsync(int socialId, CancellationToken token)
    {
        return await _userRepository.GetOneIncludeAsync(user => user.SocialId == socialId, user => user.Role, token);
    }

    public async Task<User?> GetUserByIdAsync(int id, CancellationToken token)
    {
        return await _userRepository.GetOneIncludeAsync(user => user.Id == id, user => user.Role, token);
    }

    public async Task UpdateReviewsLikesAsync(int id, int count, CancellationToken token)
    {
        var user = await GetUserByIdAsync(id, token);
        if (user != null)
        {
            user.ReviewsLikes = count;

            await _userRepository.UpdateAsync(user, token);
        }
    }

    public async Task<List<User>> GetAllIncludeAsync(CancellationToken token)
    {
        var includes = new List<Expression<Func<User, object>>>()
        {
            user => user.Role,
            user => user.Social
        };

        var users = await _userRepository.GetAllIncludeManyAsync(user => user.Id > 0, includes, token);

        return users;
    }

    public async Task<PageResult<User>> GetAllIncludeAsync(UsersFilterForm filterForm, CancellationToken token)
    {
        Expression<Func<User, bool>> condition = (user) =>
            (filterForm.Id <= 0 || user.Id == filterForm.Id) &&
            EF.Functions.Like(user.Nickname, $"%{filterForm.Nickname}%") &&
            EF.Functions.Like(user.Role.Name, $"%{filterForm.Role}%") &&
            EF.Functions.Like(user.Social.Email, $"%{filterForm.Email}%") &&
            EF.Functions.Like(user.Social.Network, $"%{filterForm.Network}%") &&
            EF.Functions.Like(user.Social.FirstName!, $"%{filterForm.FirstName}%") &&
            EF.Functions.Like(user.Social.LastName!, $"%{filterForm.LastName}%");

        Expression<Func<User, object>> orderBy = filterForm.OrderBy switch
        {
            "Id" => (user) => user.Id,
            "Nickname" => (user) => user.Nickname,
            "ReviewsLikes" => (user) => user.ReviewsLikes,
            "Role" => (user) => user.Role.Name,
            "Email" => (user) => user.Social.Email,
            "Network" => (user) => user.Social.Network,
            "FirstName" => (user) => user.Social.FirstName!,
            "LastName" => (user) => user.Social.LastName!,
            "LastLoginDate" => (user) => user.LastLoginDate,
            "RegistrationDate" => (user) => user.RegistrationDate,
            _ => (phone) => phone.Id
        };

        var includes = new List<Expression<Func<User, object>>>()
        {
            user => user.Role,
            user => user.Social
        };

        return await _userRepository.GetAllIncludeManyAsync(
            condition, includes, orderBy,
            filterForm.Page,
            filterForm.PageSize,
            token);
    }

    public async Task<User?> GetIncludesForAdminAsync(int id, CancellationToken token)
    {
        var includes = new List<Expression<Func<User, object>>>()
        {
            user => user.Role,
            user => user.Social,
            user => user.Comment,
            user => user.Review
        };

        return await _userRepository.GetOneIncludeManyAsync(user => user.Id == id, includes, token);
    }
}