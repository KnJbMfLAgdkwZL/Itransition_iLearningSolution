using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppWeb.Models;
using Business.Interfaces.Model;

namespace AppWeb.Controllers;

public class HomeController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly ITagService _tagService;
    private readonly IStatusReviewService _statusReviewService;

    public HomeController(
        IReviewService reviewService,
        ITagService tagService,
        IStatusReviewService statusReviewService
    )
    {
        _reviewService = reviewService;
        _tagService = tagService;
        _statusReviewService = statusReviewService;
    }

    public async Task<IActionResult> Index()
    {
        var status = await _statusReviewService.Get("Deleted");
        if (status == null)
        {
            return BadRequest("StatusReview Deleted not found");
        }

        var newReviews = await _reviewService.GetNewReviews();
        ViewData["NewReviews"] = newReviews.Where(review => review.StatusId != status.Id).ToList();

        var topReviews = await _reviewService.GetTopReviews();
        ViewData["TopReviews"] = topReviews.Where(review => review.StatusId != status.Id).ToList();

        ViewData["TopTags"] = await _tagService.GetTopTags();

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}