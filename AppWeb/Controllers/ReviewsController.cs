using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class ReviewsController : Controller
{
    private IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public IActionResult Index()
    {
        return Ok();
    }
}