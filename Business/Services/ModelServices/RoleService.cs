using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class RoleService : IRoleService
{
    private readonly IGeneralRepository<Role> _roleRepository;

    public RoleService(IGeneralRepository<Role> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Role?> GetRoleById(int id)
    {
        return await _roleRepository.GetOneAsync(role => role.Id == id, CancellationToken.None);
    }
}