using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class RoleService : IRoleService
{
    private readonly IGeneralRepository<Role> _roleRepository;

    public RoleService(
        IGeneralRepository<Role> roleRepository
    )
    {
        _roleRepository = roleRepository;
    }

    public async Task<Role?> GetRoleByIdAsync(int id, CancellationToken token)
    {
        return await _roleRepository.GetOneAsync(role => role.Id == id, token);
    }

    public async Task<List<Role>> GetRoleAllAsync(CancellationToken token)
    {
        return await _roleRepository.GetAllAsync(role => role.Id > 0, token);
    }
}