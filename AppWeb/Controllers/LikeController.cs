using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class LikeController : Controller
{
    private readonly IReviewLikeService _reviewLikeService;

    public LikeController(IReviewLikeService reviewLikeService)
    {
        _reviewLikeService = reviewLikeService;
    }

    public async Task<IActionResult> Add([FromQuery] int reviewId)
    {
        if (await _reviewLikeService.Add(reviewId, HttpContext))
        {
            return Ok();
        }

        return BadRequest();
    }

    public async Task<IActionResult> Remove([FromQuery] int reviewId)
    {
        if (await _reviewLikeService.Remove(reviewId, HttpContext))
        {
            return Ok();
        }

        return BadRequest();
    }
}