using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class ReviewUserRatingController : Controller
{
    private readonly IReviewUserRatingService _reviewUserRatingService;
    private readonly IUserClaimsService _userClaimsService;
    private readonly IUserService _userService;
    private readonly IReviewService _reviewService;

    public ReviewUserRatingController(IReviewUserRatingService reviewUserRatingService,
        IUserClaimsService userClaimsService, IUserService userService, IReviewService reviewService)
    {
        _reviewUserRatingService = reviewUserRatingService;
        _userClaimsService = userClaimsService;
        _userService = userService;
        _reviewService = reviewService;
    }

    public async Task<IActionResult> Set([FromQuery] int reviewId, [FromQuery] int assessment)
    {
        var userClaims = _userClaimsService.GetClaims(HttpContext);
        var user = await _userService.GetUser(userClaims);
        if (user == null)
        {
            return BadRequest();
        }

        await _reviewUserRatingService.AddAssessment(reviewId, user.Id, assessment);
        await _reviewService.CalculateAverageUserAssessment(reviewId);
        return Ok();
    }
}