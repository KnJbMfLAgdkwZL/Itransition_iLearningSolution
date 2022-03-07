using Business.Dto;
using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;



public class UserService : IUserService
{
    private readonly IGeneralRepository<User> _userRepository;
    private readonly IUserSocialService _userSocialService;

    public UserService(IGeneralRepository<User> userRepository, IUserSocialService userSocialService)
    {
        _userRepository = userRepository;
        _userSocialService = userSocialService;
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

    public async Task<User?> GetUser(UserClaims userClaims)
    {
        var userSocial = await _userSocialService.Get(userClaims);
        if (userSocial != null)
        {
            return await _userRepository.GetOneAsync(u =>
                    u.SocialId == userSocial.Id,
                CancellationToken.None);
        }

        return null;
    }
}