using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppWeb.Models;

namespace AppWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }

    public IActionResult Test()
    {
        var str1 = Business.BusinessLayer.GetStr();
        var str2 = DataAccess.DataAccessLayer.GetStr();
        var str3 = Database.DatabaseLayer.GetStr();

        var strAll = Business.BusinessLayer.GetStrAll();

        var str = $"{str1} {str2} {str3} {strAll}";


        return Ok(str);
    }
}