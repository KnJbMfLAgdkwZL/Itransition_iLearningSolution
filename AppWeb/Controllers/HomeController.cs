using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppWeb.Models;
using Business.Interfaces.Model;

namespace AppWeb.Controllers;

public class HomeController : Controller
{
    private readonly IReviewService _reviewService;

    public HomeController(
        IReviewService reviewService
    )
    {
        _reviewService = reviewService;
    }

    public async Task<IActionResult> IndexAsync(CancellationToken token)
    {
        var newReviews = await _reviewService.GetNewReviewsAsync(token);
        ViewData["newReviewsImage"] = _reviewService.GetReviewsImage(newReviews);
        _reviewService.CLearContent(newReviews);
        ViewData["NewReviews"] = newReviews;

        var topReviews = await _reviewService.GetTopReviewsAsync(token);
        ViewData["topReviewsImage"] = _reviewService.GetReviewsImage(topReviews);
        _reviewService.CLearContent(topReviews);
        ViewData["TopReviews"] = topReviews;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}