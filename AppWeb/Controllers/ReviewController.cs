using Business.Dto.Frontend.Form;
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
    public IActionResult Create([FromForm] Review review)
    {
        
        return Json(review);
    }
}