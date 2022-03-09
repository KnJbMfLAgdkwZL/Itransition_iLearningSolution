using Business.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWeb.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IUserSocialService _userSocialService;
    private readonly IUserClaimsService _userClaimsService;

    public AccountController(IAccountService accountService, IUserSocialService userSocialService,
        IUserClaimsService userClaimsService)
    {
        _accountService = accountService;
        _userSocialService = userSocialService;
        _userClaimsService = userClaimsService;
    }

    public async Task<IActionResult> AccessDenied()
    {
        var userClaims = _userClaimsService.GetClaims(HttpContext);

        if (userClaims.Uid != string.Empty && userClaims.Email != string.Empty && userClaims.Network != string.Empty)
        {
            if (await _userSocialService.Get(userClaims) == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok("AccessDenied Logout");
            }
        }

        return Ok("AccessDenied");
    }

    public IActionResult Login()
    {
        return RedirectToAction("LoginTmp", "Account");
        //return View();
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

    public async Task<IActionResult> LoginTmp(CancellationToken cancellationToken) //TEMP LOGIN
    {
        var userLogin = new
        {
            uid = "102323882587819167548",
            email = "zipocat@gmail.com",
            network = "google",
            first_name = "Zippo",
            last_name = "Cat"
        };
        var json = JsonConvert.SerializeObject(userLogin);
        if (await _accountService.LoginOrRegister(json, HttpContext, cancellationToken))
        {
            return RedirectToAction("Index", "Home");
        }

        return BadRequest();
    }
}