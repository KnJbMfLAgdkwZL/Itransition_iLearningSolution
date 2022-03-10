using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Http;

namespace Business.Services;

public class ReviewLikeService : IReviewLikeService
{
    private readonly IGeneralRepository<ReviewLike> _reviewLikeRepository;
    private readonly IUserClaimsService _userClaimsService;
    private readonly IUserService _userService;

    public ReviewLikeService(IGeneralRepository<ReviewLike> reviewLikeRepository, IUserClaimsService userClaimsService,
        IUserService userService)
    {
        _reviewLikeRepository = reviewLikeRepository;
        _userClaimsService = userClaimsService;
        _userService = userService;
    }

    public async Task<bool> Add(int reviewId, HttpContext context)
    {
        var userClaims = _userClaimsService.GetClaims(context);
        var user = await _userService.GetUser(userClaims);
        if (user == null)
        {
            return false;
        }

        await _reviewLikeRepository.AddOrUpdateAsync(
            v => v.ReviewId == reviewId && v.UserId == user.Id,
            new ReviewLike()
            {
                ReviewId = reviewId,
                UserId = user.Id,
                IsSet = true
            },
            CancellationToken.None);

        return true;
    }

    public async Task<bool> Remove(int reviewId, HttpContext context)
    {
        var userClaims = _userClaimsService.GetClaims(context);
        var user = await _userService.GetUser(userClaims);
        if (user == null)
        {
            return false;
        }

        await _reviewLikeRepository.AddOrUpdateAsync(
            v => v.ReviewId == reviewId && v.UserId == user.Id,
            new ReviewLike()
            {
                ReviewId = reviewId,
                UserId = user.Id,
                IsSet = false
            },
            CancellationToken.None);

        return true;
    }

    public async Task<bool> IsTrue(int reviewId, HttpContext context)
    {
        var userClaims = _userClaimsService.GetClaims(context);
        var user = await _userService.GetUser(userClaims);
        if (user == null)
        {
            return false;
        }

        var reviewLike = await _reviewLikeRepository.GetOneAsync(v => v.ReviewId == reviewId && v.UserId == user.Id,
            CancellationToken.None);
        if (reviewLike == null)
        {
            return false;
        }

        return reviewLike.IsSet;
    }
}