using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;
public class UserService : IUserService
{
    private readonly IGeneralRepository<User> _userRepository;

    public UserService(IGeneralRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> LoginOrRegister(User user)
    {
        return await _userRepository.AddIfNotExistAsync(u =>
                u.SocialId == user.SocialId,
            user, CancellationToken.None);
    }

    public async Task<User> Update(User user)
    {
        return await _userRepository.UpdateAsync(user, CancellationToken.None);
    }
}