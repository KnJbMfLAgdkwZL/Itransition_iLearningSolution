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

    public async Task<IActionResult> IndexAsync(CancellationToken token)
    {
        var status = await _statusReviewService.GetAsync("Deleted", token);
        if (status == null)
        {
            return BadRequest("StatusReview Deleted not found");
        }

        ViewData["NewReviews"] = await _reviewService.GetNewReviewsAsync(token);

        var topReviews = await _reviewService.GetTopReviewsAsync(token);
        ViewData["TopReviews"] = topReviews.Where(review => review.StatusId != status.Id).Take(20).ToList();

        ViewData["TopTags"] = await _tagService.GetTopTagsAsync(token);

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}