using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;

public class RoleService : IRoleService
{
    private readonly IGeneralRepository<Role> _roleRepository;

    public RoleService(IGeneralRepository<Role> roleRepository)
    {
        _roleRepository = roleRepository;
    }
}
