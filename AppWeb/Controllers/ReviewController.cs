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

    public ReviewController(IReviewService reviewService, IProductGroupService productGroupService,
        IStatusReviewService statusReviewService)
    {
        _reviewService = reviewService;
        _productGroupService = productGroupService;
        _statusReviewService = statusReviewService;
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
        }

        var review = await _reviewService.Create(reviewForm, HttpContext);
        if (review == null)
        {
            return BadRequest();
        }

        var settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = (sender, args) => { args.ErrorContext.Handled = true; },
        };
        var strJson = JsonConvert.SerializeObject(review, Formatting.Indented, settings);
        //redirect review.Id 
        return Ok(strJson);
    }
}