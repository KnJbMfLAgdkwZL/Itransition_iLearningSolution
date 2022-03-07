using Business.Dto;
using Database.Models;

namespace Business.Interfaces;

public interface IUserService
{
    Task<User> LoginOrRegister(User user);
    Task<User> Update(User user);
    Task<User?> GetUser(UserClaims userClaims);
}