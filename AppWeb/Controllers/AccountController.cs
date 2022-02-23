using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Reflection;
using System.ComponentModel.Design;

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
        var json = await http.GetAsync(url).Result.Content.ReadAsStringAsync();

        var data = JsonConvert.DeserializeObject<dynamic>(json);

        foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(data))
        {
            var name = prop.Name;
            var val = prop.GetValue(data);
            Console.WriteLine($"{name}: {val}");
        }
        Console.WriteLine();
        Console.WriteLine();

        return Json(json);
    }
}