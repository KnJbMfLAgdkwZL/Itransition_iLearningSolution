using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWeb.Controllers;

public class AccountController : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoginResponse([FromForm] string token)
    {
        var httpHost = HttpContext.Request.Host.Value;
        var url = $"http://ulogin.ru/token.php?token={token}&host={httpHost}";

        var http = new HttpClient();
        var httpResponse = await http.GetAsync(url);
        
        var json = await httpResponse.Content.ReadAsStringAsync();
        var user = JsonConvert.DeserializeObject<UserSocial>(json);
        
        if (user != null)
        {
            Console.WriteLine(user.Network);
            Console.WriteLine(user.Uid);
            Console.WriteLine(user.Email);
            Console.WriteLine(user.First_name);
            Console.WriteLine(user.Last_name);
            Console.WriteLine();

            return Ok();
        }

        return BadRequest();
    }
}

class UserSocial
{
    public string Network { set; get; } = string.Empty;
    public string Uid { set; get; } = string.Empty;
    public string Email { set; get; } = string.Empty;
    public string First_name { set; get; } = string.Empty;
    public string Last_name { set; get; } = string.Empty;
}