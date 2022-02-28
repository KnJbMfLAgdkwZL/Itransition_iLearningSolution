using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;

public class UserSocialService : IUserSocialService
{
    private readonly IGeneralRepository<UserSocial> _userSocialRepository;

    public UserSocialService(IGeneralRepository<UserSocial> userSocialRepository)
    {
        _userSocialRepository = userSocialRepository;
    }

    public bool Check(UserSocial? userSocial)
    {
        if (userSocial == null)
        {
            return false;
        }

        return userSocial.Uid != string.Empty &&
               userSocial.Email != string.Empty &&
               userSocial.Network != string.Empty;
    }

    public async Task<UserSocial> LoginOrRegister(UserSocial userSocial)
    {
        return await _userSocialRepository.AddIfNotExistAsync(us =>
                us.Uid == userSocial!.Uid &&
                us.Email == userSocial.Email &&
                us.Network == userSocial.Network,
            userSocial!, CancellationToken.None);
    }

    public async Task<UserSocial?> Get(string uid, string email, string network)
    {
        return await _userSocialRepository.GetOneAsync(us =>
                us.Uid == uid &&
                us.Email == email &&
                us.Network == network,
            CancellationToken.None);
    }
}