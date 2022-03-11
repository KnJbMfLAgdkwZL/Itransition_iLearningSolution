using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppWeb.Models;
using Business.Interfaces.Model;

namespace AppWeb.Controllers;

public class HomeController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly ITagService _tagService;

    public HomeController(IReviewService reviewService, ITagService tagService)
    {
        _reviewService = reviewService;
        _tagService = tagService;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["NewReviews"] = await _reviewService.GetNewReviews();
        ViewData["TopReviews"] = await _reviewService.GetTopReviews();
        ViewData["TopTags"] = await _tagService.GetTopTags();
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}