using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AppWeb.Models;
using Business.Interfaces.Model;
using Database.Models;
using Newtonsoft.Json;

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

        ViewData["TopReviews"] = await _reviewService.GetTopReviewsAsync(token);
        
        var tags = await _tagService.GetTopTagsAsync(token);
        
        var settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = (sender, args) => { args.ErrorContext.Handled = true; },
        };
        ViewData["TopTags"] = JsonConvert.SerializeObject(tags, Formatting.Indented, settings);
        

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}