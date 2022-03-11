using Business.Interfaces;
using Business.Interfaces.Model;
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

    [Authorize]
    public async Task<IActionResult> Add([FromQuery] int reviewId)
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

        await _reviewLikeService.Add(reviewId, user.Id);
        
        var count = await _reviewLikeService.GetLikesCount(reviewId);
        await _userService.UpdateReviewsLikes(user.Id, count);

        return Ok();
    }

    [Authorize]
    public async Task<IActionResult> Remove([FromQuery] int reviewId)
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

        await _reviewLikeService.Remove(reviewId, user.Id);
        
        var count = await _reviewLikeService.GetLikesCount(reviewId);
        await _userService.UpdateReviewsLikes(user.Id, count);

        return Ok();
    }
}