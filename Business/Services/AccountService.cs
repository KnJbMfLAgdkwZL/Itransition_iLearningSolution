using System.Security.Claims;
using Business.Dto;
using Business.Interfaces;
using Business.Interfaces.Model;
using Database.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UserSocial = Database.Models.UserSocial;

namespace Business.Services;

public class AccountService : IAccountService
{
    private readonly IUserSocialService _userSocialService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IClaimsService _claimsService;

    public AccountService(
        IUserSocialService userSocialService,
        IUserService userService,
        IRoleService roleService,
        IClaimsService claimsService
    )
    {
        _userSocialService = userSocialService;
        _userService = userService;
        _roleService = roleService;
        _claimsService = claimsService;
    }

    public async Task<bool> LoginOrRegister(string json, HttpContext httpContext, CancellationToken token)
    {
        var userSocialDto = JsonConvert.DeserializeObject<Dto.UserSocial>(json);
        var userSocialModel = new UserSocial()
        {
            Uid = userSocialDto!.Uid!,
            Email = userSocialDto.Email!,
            Network = userSocialDto.Network,
            FirstName = userSocialDto.First_name,
            LastName = userSocialDto.Last_name
        };

        if (_userSocialService.Check(userSocialModel) == false)
        {
            return false;
        }

        var userSocialRes = await _userSocialService.LoginOrRegister(userSocialModel!);
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

        await AuthenticateAsync(userSocialModel!, httpContext, role.Name);
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

    private User? GetAuthorizedUser(dynamic context, out IActionResult? error)
    {
        var userClaims = (UserClaims) _claimsService.GetClaims(context);
        var userSocial = _userSocialService.Get(userClaims).Result;
        if (userSocial == null)
        {
            error = new UnauthorizedObjectResult("UserSocial not found");
            return null;
        }

        var user = _userService.GetUserBySocialId(userSocial.Id).Result;
        if (user == null)
        {
            error = new UnauthorizedObjectResult("User not found");
            return null;
        }

        if (user.Role.Name != userClaims.Role)
        {
            error = new UnauthorizedObjectResult("Claims.Role and user.Role not match");
            return null;
        }

        error = null;
        return user;
    }

    public User? GetAuthorizedUser(AuthorizationHandlerContext context, out IActionResult? error)
    {
        var user = GetAuthorizedUser((object) context, out var error1);
        error = error1;
        return user;
    }

    public User? GetAuthorizedUser(HttpContext context, out IActionResult? error)
    {
        var user = GetAuthorizedUser((object) context, out var error1);
        error = error1;
        return user;
    }
}