using Business.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IUserSocialService _userSocialService;

    public AccountController(IAccountService accountService, IUserSocialService userSocialService)
    {
        _accountService = accountService;
        _userSocialService = userSocialService;
    }

    public async Task<IActionResult> AccessDenied()
    {
        var claimUid = User.FindFirst("Uid");
        var uid = claimUid == null ? string.Empty : claimUid.Value;

        var claimEmail = User.FindFirst("Email");
        var email = claimEmail == null ? string.Empty : claimEmail.Value;

        var claimNetwork = User.FindFirst("Network");
        var network = claimNetwork == null ? string.Empty : claimNetwork.Value;

        if (uid != string.Empty && email != string.Empty && network != string.Empty)
        {
            if (await _userSocialService.Get(uid, email, network) == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok("AccessDenied Logout");
            }
        }

        return Ok("AccessDenied");
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoginResponse([FromForm] string token, CancellationToken cancellationToken)
    {
        var httpHost = HttpContext.Request.Host.Value;
        var url = $"http://ulogin.ru/token.php?token={token}&host={httpHost}";

        var http = new HttpClient();
        var httpResponse = await http.GetAsync(url);
        var json = await httpResponse.Content.ReadAsStringAsync();

        if (await _accountService.LoginOrRegister(json, HttpContext, cancellationToken))
        {
            return RedirectToAction("Index", "Home");
        }

        return BadRequest();
    }

    public async Task<IActionResult> LogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}