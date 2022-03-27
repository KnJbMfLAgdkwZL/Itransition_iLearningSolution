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
    public async Task<IActionResult> AddAsync([FromQuery] int reviewId, CancellationToken token)
    {
        return await AddOrRemoveAsync(reviewId, "Add", token);
    }

    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> RemoveAsync([FromQuery] int reviewId, CancellationToken token)
    {
        return await AddOrRemoveAsync(reviewId, "Remove", token);
    }

    private Review? GetOneReview(int reviewId, out IActionResult? error, CancellationToken token)
    {
        var review = _reviewService.GetOneAsync(reviewId, token).Result;
        if (review == null)
        {
            error = BadRequest("Review not found");
        }

        error = null;

        return review;
    }

    private async Task<IActionResult> AddOrRemoveAsync(int reviewId, string action, CancellationToken token)
    {
        var user = _accountService.GetAuthorizedUser(HttpContext, out var error, token);
        if (user == null)
        {
            return error!;
        }

        var review = GetOneReview(reviewId, out var errorReview, token);
        if (review == null)
        {
            return errorReview!;
        }

        var like = 0;
        if (action == "Add")
        {
            like++;
            await _reviewLikeService.AddAsync(reviewId, user.Id, token);
        }
        else if (action == "Remove")
        {
            like--;
            await _reviewLikeService.RemoveAsync(reviewId, user.Id, token);
        }
        else
        {
            return BadRequest();
        }

        var count = await _reviewLikeService.GetLikesCountAsync(reviewId, token);

        await _userService.UpdateReviewsLikesAsync(user.Id, like, token);

        return Ok(count);
    }
}