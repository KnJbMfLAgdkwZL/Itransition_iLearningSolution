using System.Linq.Expressions;
using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class UserService : IUserService
{
    private readonly IGeneralRepository<User> _userRepository;

    public UserService(IGeneralRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> LoginOrRegister(User user)
    {
        return await _userRepository.AddIfNotExistAsync(u => u.SocialId == user.SocialId, user, CancellationToken.None);
    }

    public async Task<User> Update(User user)
    {
        return await _userRepository.UpdateAsync(user, CancellationToken.None);
    }

    public async Task<User?> GetUserBySocialId(int socialId)
    {
        return await _userRepository.GetOneIncludeAsync(user => user.SocialId == socialId, user => user.Role,
            CancellationToken.None);
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _userRepository.GetOneIncludeAsync(user => user.Id == id, user => user.Role,
            CancellationToken.None);
    }

    public async Task UpdateReviewsLikes(int id, int count)
    {
        var user = await GetUserById(id);
        if (user != null)
        {
            user.ReviewsLikes = count;
            await _userRepository.UpdateAsync(user, CancellationToken.None);
        }
    }

    public async Task<List<User>> GetAllInclude()
    {
        var includes = new List<Expression<Func<User, object>>>()
        {
            user => user.Role,
            user => user.Social
        };
        var users = await _userRepository.GetAllIncludeManyAsync(user => user.Id > 0, includes, CancellationToken.None);
        return users;
    }

    public async Task<User?> GetIncludesForAdmin(int id)
    {
        var includes = new List<Expression<Func<User, object>>>()
        {
            user => user.Role,
            user => user.Social,
            user => user.Comment,
            user => user.Review
        };
        return await _userRepository.GetOneIncludeManyAsync(user => user.Id == id, includes, CancellationToken.None);
    }
}