using Database.Models;

namespace Business.Interfaces.Model;

public interface IRoleService
{
    Task<Role?> GetRoleByIdAsync(int id, CancellationToken token);
    Task<List<Role>> GetRoleAllAsync(CancellationToken token);
}