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

    public async Task<IActionResult> SetAsync([FromQuery] int reviewId, [FromQuery] int assessment, CancellationToken token)
    {
        var user = _accountService.GetAuthorizedUser(HttpContext, out var error, token);
        if (user == null)
        {
            return error!;
        }

        var review = await _reviewService.GetOneAsync(reviewId, token);
        if (review == null)
        {
            return BadRequest("Review not found");
        }

        await _reviewUserRatingService.AddAssessmentAsync(reviewId, user.Id, assessment, token);

        var averageAssessment = await _reviewUserRatingService.GetAverageAssessmentAsync(reviewId, token);

        await _reviewService.UpdateAverageUserRatingAsync(reviewId, averageAssessment, token);

        return Ok();
    }
}