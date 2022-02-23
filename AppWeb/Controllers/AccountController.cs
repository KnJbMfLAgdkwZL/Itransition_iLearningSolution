using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class AccountController : Controller
{
    public IActionResult Login([FromQuery] string type = "Google")
    {
        var authenticationScheme = GoogleDefaults.AuthenticationScheme;
        if (type == "Facebook")
        {
            authenticationScheme = FacebookDefaults.AuthenticationScheme;
        }

        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("LoginResponse", "Account", null, "https")
            //RedirectUri = Url.Action("LoginResponse", "Account")
        };
        return Challenge(properties, authenticationScheme);
    }

    public async Task<IActionResult> LoginResponse()
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