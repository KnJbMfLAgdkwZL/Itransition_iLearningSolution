using Database.Models;

namespace Business.Interfaces.Model;

public interface IRoleService
{
    Task<Role?> GetRoleById(int id);
}