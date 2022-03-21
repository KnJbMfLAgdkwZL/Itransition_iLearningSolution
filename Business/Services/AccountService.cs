using System.Security.Claims;
using Business.Interfaces;
using Business.Interfaces.Model;
using Database.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Business.Services;

public class AccountService : IAccountService
{
    private readonly IConverterService _converterService;
    private readonly IUserSocialService _userSocialService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public AccountService(IConverterService converterService, IUserSocialService userSocialService,
        IUserService userService, IRoleService roleService)
    {
        _converterService = converterService;
        _userSocialService = userSocialService;
        _userService = userService;
        _roleService = roleService;
    }

    public async Task<bool> LoginOrRegister(string json, HttpContext httpContext, CancellationToken token)
    {
        var userSocial = _converterService.UserSociaModel(json);
        if (_userSocialService.Check(userSocial) == false)
        {
            return false;
        }

        var userSocialRes = await _userSocialService.LoginOrRegister(userSocial!);
        var nickname = "Anonymous";
        if (userSocialRes.FirstName != string.Empty && userSocialRes.LastName != string.Empty)
        {
            nickname = $"{userSocialRes.FirstName} {userSocialRes.LastName}";
        }

        var lastLoginDate = DateTime.Now;
        var user = new User()
        {
            SocialId = userSocialRes.Id,
            RoleId = 1,
            RegistrationDate = DateTime.Now,
            Avatar = "Default",
            ReviewsLikes = 0,
            Nickname = nickname,
            LastLoginDate = lastLoginDate
        };
        var userRes = await _userService.LoginOrRegister(user);
        if (userRes.Id > 0)
        {
            userRes.LastLoginDate = lastLoginDate;
            await _userService.Update(userRes);
        }

        var role = await _roleService.GetRoleById(userRes.RoleId);
        if (role == null)
        {
            return false;
        }

        var userCheck = await _userService.GetUserBySocialId(userSocialRes.Id);
        if (userCheck == null || userCheck.Role.Name != "Admin" && userCheck.Role.Name != "User")
        {
            return false;
        }

        await AuthenticateAsync(userSocial!, httpContext, role.Name);
        return true;
    }

    private async Task AuthenticateAsync(UserSocial userSocial, HttpContext httpContext, string role)
    {
        var claims = new List<Claim>
        {
            new("Uid", userSocial.Uid),
            new("Email", userSocial.Email),
            new("Network", userSocial.Network),
            new(ClaimsIdentity.DefaultRoleClaimType, role)
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
    }
}