using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWeb.Controllers;

public class AccountController : Controller
{
    public IActionResult Login([FromQuery] string type = "Google")
    {
        ViewData["type"] = type;
        return View();
    }

    [HttpPost]
    public IActionResult LoginResponse([FromBody] [Required] ToolButtonAction data)
    {
        Console.WriteLine(data.Id);
        Console.WriteLine(data.Name);
        Console.WriteLine(data.Type);
        Console.WriteLine(data.ImageUrl);
        Console.WriteLine(data.Email);


        return Ok("sadfsdasa");
    }

    [HttpPost]
    public async Task<IActionResult> SomeResponse([FromForm] string token)
    {
        var httpHost = HttpContext.Request.Host.Value;
        var url = $"http://ulogin.ru/token.php?token={token}&host={httpHost}";

        var http = new HttpClient();
        var data = await http.GetAsync(url).Result.Content.ReadAsStringAsync();
        
        var request = JsonConvert.DeserializeObject<dynamic>(data);


        return Json(data);
    }
}

public class ToolButtonAction
{
    [Required] public string Id { get; set; } = string.Empty;
    [Required] public string Name { get; set; } = string.Empty;
    [Required] public string ImageUrl { get; set; } = string.Empty;
    [Required] public string Email { get; set; } = string.Empty;
    [Required] public string Type { get; set; } = string.Empty;
}