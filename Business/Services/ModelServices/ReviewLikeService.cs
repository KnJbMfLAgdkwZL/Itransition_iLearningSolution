using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class ReviewLikeService : IReviewLikeService
{
    private readonly IGeneralRepository<ReviewLike> _reviewLikeRepository;

    public ReviewLikeService(
        IGeneralRepository<ReviewLike> reviewLikeRepository
    )
    {
        _reviewLikeRepository = reviewLikeRepository;
    }

    public async Task Add(int reviewId, int userId)
    {
        await _reviewLikeRepository.AddOrUpdateAsync(
            like =>
                like.ReviewId == reviewId &&
                like.UserId == userId,
            new ReviewLike()
            {
                ReviewId = reviewId,
                UserId = userId,
                IsSet = true
            },
            CancellationToken.None);
    }

    public async Task Remove(int reviewId, int userId)
    {
        await _reviewLikeRepository.AddOrUpdateAsync(
            like =>
                like.ReviewId == reviewId &&
                like.UserId == userId,
            new ReviewLike()
            {
                ReviewId = reviewId,
                UserId = userId,
                IsSet = false
            },
            CancellationToken.None);
    }

    public async Task<bool> IsUserLikeReview(int userId, int reviewId)
    {
        var reviewLike = await _reviewLikeRepository.GetOneAsync(
            like =>
                like.ReviewId == reviewId &&
                like.UserId == userId,
            CancellationToken.None);
        return reviewLike != null && reviewLike.IsSet;
    }

    public async Task<int> GetLikesCount(int reviewId)
    {
        var likes = await _reviewLikeRepository.GetAllAsync(
            like =>
                like.ReviewId == reviewId &&
                like.IsSet == true,
            CancellationToken.None);
        return likes.Count;
    }
}