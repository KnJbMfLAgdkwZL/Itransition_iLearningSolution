using Business.Interfaces;
using Business.Interfaces.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class ReviewUserRatingController : Controller
{
    private readonly IReviewUserRatingService _reviewUserRatingService;
    private readonly IUserClaimsService _userClaimsService;
    private readonly IUserService _userService;
    private readonly IReviewService _reviewService;
    private readonly IUserSocialService _userSocialService;

    public ReviewUserRatingController(IReviewUserRatingService reviewUserRatingService,
        IUserClaimsService userClaimsService, IUserService userService, IReviewService reviewService,
        IUserSocialService userSocialService)
    {
        _reviewUserRatingService = reviewUserRatingService;
        _userClaimsService = userClaimsService;
        _userService = userService;
        _reviewService = reviewService;
        _userSocialService = userSocialService;
    }

    [Authorize]
    public async Task<IActionResult> Set([FromQuery] int reviewId, [FromQuery] int assessment)
    {
        var userClaims = _userClaimsService.GetClaims(HttpContext);
        var userSocial = await _userSocialService.Get(userClaims);
        if (userSocial == null)
        {
            return BadRequest("UserSocial not found");
        }

        var user = await _userService.GetUserBySocialId(userSocial.Id);
        if (user == null)
        {
            return BadRequest("User not found");
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