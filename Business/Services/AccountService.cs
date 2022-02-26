using System.Security.Claims;
using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Business.Services;

public class AccountService : IAccountService
{
    private readonly IConverterService _converterService;
    private readonly IGeneralRepository<UserSocial> _userSocialRepository;

    public AccountService(IConverterService converterService, IGeneralRepository<UserSocial> userSocialRepository)
    {
        _converterService = converterService;
        _userSocialRepository = userSocialRepository;
    }

    public async Task<bool> LoginOrRegister(string json, HttpContext httpContext, CancellationToken token)
    {
        var userSocial = _converterService.UserSociaModel(json);
        if (!Check(userSocial))
        {
            return false;
        }

        await _userSocialRepository.AddIfNotExistAsync(us =>
            us.Uid == userSocial!.Uid &&
            us.Email == userSocial.Email &&
            us.Network == userSocial.Network, userSocial!, token);

        await AuthenticateAsync(userSocial!, httpContext);

        return true;
    }

    public async Task<bool> GetUserSocial(string uid, string email, string network)
    {
        var res = await _userSocialRepository.GetOneAsync(us =>
                us.Uid == uid &&
                us.Email == email &&
                us.Network == network
            , CancellationToken.None);
        return res != null;
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

    private bool Check(UserSocial? userSocial)
    {
        if (userSocial == null)
        {
            return false;
        }

        return userSocial.Uid != string.Empty && userSocial.Email != string.Empty;
    }
}