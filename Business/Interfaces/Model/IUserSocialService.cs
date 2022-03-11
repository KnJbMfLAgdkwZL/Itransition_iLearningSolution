using Business.Dto;
using UserSocial = Database.Models.UserSocial;

namespace Business.Interfaces.Model;

public interface IUserSocialService
{
    bool Check(UserSocial? userSocial);
    Task<UserSocial> LoginOrRegister(UserSocial userSocial);
    Task<UserSocial?> Get(UserClaims userClaims);
}