using Business.Dto;
using UserSocial = Database.Models.UserSocial;

namespace Business.Interfaces.Model;

public interface IUserSocialService
{
    bool Check(UserSocial? userSocial);
    Task<UserSocial> LoginOrRegisterAsync(UserSocial userSocial, CancellationToken token);
    Task<UserSocial?> GetAsync(UserClaims userClaims, CancellationToken token);
}