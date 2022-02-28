using Database.Models;

namespace Business.Interfaces;

public interface IUserService
{
    Task<User> LoginOrRegister(User user);
    Task<User> Update(User user);
}