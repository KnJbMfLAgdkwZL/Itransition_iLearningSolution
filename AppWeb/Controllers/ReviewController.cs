using Business.Dto.Frontend.FromForm;
using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWeb.Controllers;

public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly IProductGroupService _productGroupService;
    private readonly IStatusReviewService _statusReviewService;
    private readonly IReviewTagService _reviewTagService;

    public ReviewController(IReviewService reviewService, IProductGroupService productGroupService,
        IStatusReviewService statusReviewService, IReviewTagService reviewTagService)
    {
        _reviewService = reviewService;
        _productGroupService = productGroupService;
        _statusReviewService = statusReviewService;
        _reviewTagService = reviewTagService;
    }

    public IActionResult Index()
    {
        //return View();
        return Ok();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["productGroups"] = await _productGroupService.GetAll();
        ViewData["statusReviews"] = await _statusReviewService.GetAll();
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ReviewForm reviewForm)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var review = await _reviewService.Create(reviewForm, HttpContext);
        if (review == null)
        {
            return BadRequest();
        }

        return RedirectToAction("Get", "Review", new {id = review.Id});
    }

    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var review = await _reviewService.Get(id);
        if (review == null)
        {
            return BadRequest();
        }

        ViewData["review"] = review;
        ViewData["tags"] = await _reviewTagService.GetTagsNames(review.Id);
        return View();
    }
}