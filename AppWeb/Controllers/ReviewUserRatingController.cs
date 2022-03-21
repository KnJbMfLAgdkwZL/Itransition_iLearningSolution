using Business.Interfaces;
using Business.Interfaces.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

[Authorize(Roles = "Admin, User")]
public class ReviewUserRatingController : Controller
{
    private readonly IReviewUserRatingService _reviewUserRatingService;
    private readonly IReviewService _reviewService;
    private readonly IAccountService _accountService;

    public ReviewUserRatingController(
        IReviewUserRatingService reviewUserRatingService,
        IReviewService reviewService,
        IAccountService accountService
    )
    {
        _reviewUserRatingService = reviewUserRatingService;
        _reviewService = reviewService;
        _accountService = accountService;
    }

    public async Task<IActionResult> Set([FromQuery] int reviewId, [FromQuery] int assessment)
    {
        var user = _accountService.GetAuthorizedUser(HttpContext, out var error);
        if (user == null)
        {
            return error!;
        }

        var review = await _reviewService.GetOne(reviewId);
        if (review == null)
        {
            return BadRequest("Review not found");
        }

        await _reviewUserRatingService.AddAssessment(reviewId, user.Id, assessment);

        var averageAssessment = await _reviewUserRatingService.GetAverageAssessment(reviewId);

        await _reviewService.UpdateAverageUserRating(reviewId, averageAssessment);

        return Ok();
    }
}