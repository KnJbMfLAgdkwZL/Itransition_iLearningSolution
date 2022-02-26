using Business.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class AccountController : Controller
{
    private IAccountServices _accountServices;

    public AccountController(IAccountServices accountServices)
    {
        _accountServices = accountServices;
    }

    [AllowAnonymous]
    [HttpGet("AccessDenied")]
    public async Task<IActionResult> AccessDenied()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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

        if (await _accountServices.LoginOrRegister(json, HttpContext, cancellationToken))
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