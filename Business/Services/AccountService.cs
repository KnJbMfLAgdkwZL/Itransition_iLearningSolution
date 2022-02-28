using System.Security.Claims;
using Business.Interfaces;
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

    public AccountService(IConverterService converterService, IUserSocialService userSocialService,
        IUserService userService)
    {
        _converterService = converterService;
        _userSocialService = userSocialService;
        _userService = userService;
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

        await AuthenticateAsync(userSocial!, httpContext);
        return true;
    }

    private async Task AuthenticateAsync(UserSocial userSocial, HttpContext httpContext)
    {
        var claims = new List<Claim>
        {
            new("Uid", userSocial.Uid),
            new("Email", userSocial.Email),
            new("Network", userSocial.Network)
        };
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
    }
}