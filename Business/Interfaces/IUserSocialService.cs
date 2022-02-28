using Database.Models;

namespace Business.Interfaces;

public interface IUserSocialService
{
    bool Check(UserSocial? userSocial);
    Task<UserSocial> LoginOrRegister(UserSocial userSocial);
    Task<UserSocial?> Get(string uid, string email, string network);
}