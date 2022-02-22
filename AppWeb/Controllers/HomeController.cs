using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppWeb.Models;
using Database.DbContexts;

namespace AppWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MasterContext _db;

    public HomeController(ILogger<HomeController> logger, MasterContext db)
    {
        _logger = logger;
        this._db = db;
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
        var str = _db.Test.FirstOrDefault(test => test.Id == 1)?.Name;
        return Ok(str);
    }
}