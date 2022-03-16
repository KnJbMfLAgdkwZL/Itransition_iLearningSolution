using Business.Interfaces;
using Business.Interfaces.Model;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

[Authorize]
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

    private User? GetAuthorizedUser(out IActionResult? error)
    {
        var userClaims = _userClaimsService.GetClaims(HttpContext);
        var userSocial = _userSocialService.Get(userClaims).Result;
        if (userSocial == null)
        {
            error = BadRequest("UserSocial not found");
            return null;
        }

        var user = _userService.GetUserBySocialId(userSocial.Id).Result;
        if (user == null)
        {
            error = BadRequest("User not found");
            return null;
        }

        error = null;
        return user;
    }

    [Authorize]
    public async Task<IActionResult> Set([FromQuery] int reviewId, [FromQuery] int assessment)
    {
        var user = GetAuthorizedUser(out var error);
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