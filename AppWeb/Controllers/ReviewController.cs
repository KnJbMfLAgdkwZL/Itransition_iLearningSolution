using Business.Dto.Frontend.FromForm;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public IActionResult Index()
    {
        //return View();
        return Ok();
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ReviewForm reviewForm)
    {
        await _reviewService.Create(reviewForm);
        return Json(reviewForm);
    }
}