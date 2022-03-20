using System.Linq.Expressions;
using Database.Models;

namespace Business.Interfaces.Model;

public interface IUserService
{
    Task<User> LoginOrRegister(User user);
    Task<User> Update(User user);
    Task<User?> GetUserBySocialId(int socialId);
    Task<User?> GetUserBySocialIdWithRole(int socialId);
    Task<User?> GetUserById(int id);
    Task UpdateReviewsLikes(int id, int count);
    Task<List<User>> GetAllInclude();
    Task<User?> GetIncludesForAdmin(int id);
}