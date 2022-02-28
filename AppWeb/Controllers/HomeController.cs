using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppWeb.Models;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace AppWeb.Controllers;

public class HomeController : Controller
{
    private readonly IReviewService _reviewService;

    public HomeController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["MainPageData"] = await _reviewService.GetMainPageData();
        return View();
    }

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}