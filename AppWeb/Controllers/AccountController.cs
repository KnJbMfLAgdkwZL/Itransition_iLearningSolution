using Business.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWeb.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(
        IAccountService accountService
    )
    {
        _accountService = accountService;
    }

    public async Task<IActionResult> AccessDeniedAsync(CancellationToken token)
    {
        var user = _accountService.GetAuthorizedUser(HttpContext, out var error, token);
        if (user == null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return error!;
        }

        return Ok("AccessDenied");
    }

    public IActionResult Login()
    {
        return RedirectToAction("LoginTmp", "Account");
        //return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoginResponseAsync([FromForm] string token, CancellationToken cancellationToken)
    {
        var httpHost = HttpContext.Request.Host.Value;
        
        var url = $"http://ulogin.ru/token.php?token={token}&host={httpHost}";

        var http = new HttpClient();
        
        var httpResponse = await http.GetAsync(url, cancellationToken);
        
        var json = await httpResponse.Content.ReadAsStringAsync(cancellationToken);

        if (await _accountService.LoginOrRegisterAsync(json, HttpContext, cancellationToken))
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

    public async Task<IActionResult> LoginTmpAsync(CancellationToken token) //TEMP LOGIN
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
        
        if (await _accountService.LoginOrRegisterAsync(json, HttpContext, token))
        {
            return RedirectToAction("Index", "Home");
        }

        return BadRequest();
    }
}