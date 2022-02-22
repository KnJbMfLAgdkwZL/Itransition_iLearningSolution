using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class AccountController : Controller
{
    public IActionResult GoogleLogin()
    {
        //var redirectUrl = this.Url.Action("GoogleResponse", "Account", null, Request.Scheme);

        const string redirectUrl = "https://ilearningsolution.herokuapp.com/signin-google";
        
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl,
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var claims = result.Principal?.Identities.FirstOrDefault()?.Claims.Select(claim => new
        {
            claim.Issuer,
            claim.OriginalIssuer,
            claim.Type,
            claim.Value
        });
        return Json(claims);
    }

    public IActionResult FacebookLogin()
    {
        var redirectUrl = this.Url.Action("FacebookResponse", "Account", null, Request.Scheme);
        Console.WriteLine(redirectUrl);
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl,
        };
        return Challenge(properties, FacebookDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> FacebookResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var claims = result.Principal?.Identities.FirstOrDefault()?.Claims.Select(claim => new
        {
            claim.Issuer,
            claim.OriginalIssuer,
            claim.Type,
            claim.Value
        });
        return Json(claims);
    }
}