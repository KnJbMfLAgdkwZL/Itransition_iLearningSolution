using Business.Interfaces;
using Business.Interfaces.Model;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class LikeController : Controller
{
    private readonly IReviewLikeService _reviewLikeService;
    private readonly IUserService _userService;
    private readonly IReviewService _reviewService;
    private readonly IUserClaimsService _userClaimsService;
    private readonly IUserSocialService _userSocialService;

    public LikeController(IReviewLikeService reviewLikeService, IUserService userService, IReviewService reviewService,
        IUserClaimsService userClaimsService, IUserSocialService userSocialService)
    {
        _reviewLikeService = reviewLikeService;
        _userService = userService;
        _reviewService = reviewService;
        _userClaimsService = userClaimsService;
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

    private Review? GetOneReview(int reviewId, out IActionResult? error)
    {
        var review = _reviewService.GetOne(reviewId).Result;
        if (review == null)
        {
            error = BadRequest("Review not found");
        }

        error = null;
        return review;
    }

    private async Task<IActionResult> AddOrRemove(int reviewId, string action)
    {
        var user = GetAuthorizedUser(out var error);
        if (user == null)
        {
            return error!;
        }

        var review = GetOneReview(reviewId, out var errorReview);
        if (review == null)
        {
            return errorReview!;
        }

        if (action == "Add")
        {
            await _reviewLikeService.Add(reviewId, user.Id);
        }
        else if (action == "Remove")
        {
            await _reviewLikeService.Remove(reviewId, user.Id);
        }
        else
        {
            return BadRequest();
        }

        var count = await _reviewLikeService.GetLikesCount(reviewId);
        await _userService.UpdateReviewsLikes(user.Id, count);

        return Ok();
    }

    [Authorize]
    public async Task<IActionResult> Add([FromQuery] int reviewId)
    {
        return await AddOrRemove(reviewId, "Add");
    }

    [Authorize]
    public async Task<IActionResult> Remove([FromQuery] int reviewId)
    {
        return await AddOrRemove(reviewId, "Remove");
    }
}