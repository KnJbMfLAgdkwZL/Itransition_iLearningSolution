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
        return await _userRepository.GetOneAsync(user => user.SocialId == socialId, CancellationToken.None);
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _userRepository.GetOneAsync(user => user.Id == id, CancellationToken.None);
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
}