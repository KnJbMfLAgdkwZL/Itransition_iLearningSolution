using Business.Dto;
using Business.Interfaces.Model;
using DataAccess.Interfaces;
using UserSocial = Database.Models.UserSocial;

namespace Business.Services.ModelServices;

public class UserSocialService : IUserSocialService
{
    private readonly IGeneralRepository<UserSocial> _userSocialRepository;

    public UserSocialService(
        IGeneralRepository<UserSocial> userSocialRepository
    )
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

    public async Task<UserSocial> LoginOrRegisterAsync(UserSocial userSocial, CancellationToken token)
    {
        return await _userSocialRepository.AddIfNotExistAsync(social =>
                social.Uid == userSocial!.Uid &&
                social.Email == userSocial.Email &&
                social.Network == userSocial.Network,
            userSocial!, token);
    }

    public async Task<UserSocial?> GetAsync(UserClaims userClaims, CancellationToken token)
    {
        return await _userSocialRepository.GetOneAsync(userSocial =>
                userSocial.Uid == userClaims.Uid &&
                userSocial.Email == userClaims.Email &&
                userSocial.Network == userClaims.Network,
            token);
    }
}