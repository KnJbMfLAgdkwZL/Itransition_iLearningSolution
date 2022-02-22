using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace AppWeb.Controllers;

[Route("account")]
public class AccountController : Controller
{
    [Route("googlelogin")]
    public async Task GoogleLogin()
    {
        var fullUrl = Url.Action("GoogleResponse", "Account", new { }, "https");
        //var url = Url.Action("GoogleResponse", "Account");

        Console.WriteLine();
        Console.WriteLine(fullUrl);
        //Console.WriteLine(url);
        Console.WriteLine();
        
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
        {
            RedirectUri = fullUrl
        });

        /*var redirectUrl = this.Url.Action("GoogleResponse", "Account");
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl,
        };*/
        //return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [Route("googleresponse")]
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
        var redirectUrl = this.Url.Action("FacebookResponse", "Account");
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