using System.Linq.Expressions;
using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

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