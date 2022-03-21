using Business.Interfaces;
using Business.Interfaces.Model;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

[Authorize(Roles = "Admin, User")]
public class LikeController : Controller
{
    private readonly IReviewLikeService _reviewLikeService;
    private readonly IUserService _userService;
    private readonly IReviewService _reviewService;
    private readonly IAccountService _accountService;

    public LikeController(
        IReviewLikeService reviewLikeService,
        IUserService userService,
        IReviewService reviewService,
        IAccountService accountService
    )
    {
        _reviewLikeService = reviewLikeService;
        _userService = userService;
        _reviewService = reviewService;
        _accountService = accountService;
    }

    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> Add([FromQuery] int reviewId)
    {
        return await AddOrRemove(reviewId, "Add");
    }

    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> Remove([FromQuery] int reviewId)
    {
        return await AddOrRemove(reviewId, "Remove");
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
        var user = _accountService.GetAuthorizedUser(HttpContext, out var error);
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
}