using Business.Dto.Frontend.FromForm;
using DataAccess.Dto;
using Database.Models;

namespace Business.Interfaces.Model;

public interface IUserService
{
    Task<User> LoginOrRegisterAsync(User user, CancellationToken token);
    Task<User> UpdateAsync(User user, CancellationToken token);
    Task<User?> GetUserBySocialIdAsync(int socialId, CancellationToken token);
    Task<User?> GetUserByIdAsync(int id, CancellationToken token);
    Task UpdateReviewsLikesAsync(int id, int count, CancellationToken token);
    Task<List<User>> GetAllIncludeAsync(CancellationToken token);
    Task<PageResult<User>> GetAllIncludeAsync(UsersFilterForm filterForm, CancellationToken token);
    Task<User?> GetIncludesForAdminAsync(int id, CancellationToken token);
}