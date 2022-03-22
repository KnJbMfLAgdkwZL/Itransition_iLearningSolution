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

    public async Task AddAsync(int reviewId, int userId, CancellationToken token)
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
            token);
    }

    public async Task RemoveAsync(int reviewId, int userId, CancellationToken token)
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
            token);
    }

    public async Task<bool> IsUserLikeReviewAsync(int userId, int reviewId, CancellationToken token)
    {
        var reviewLike = await _reviewLikeRepository.GetOneAsync(
            like =>
                like.ReviewId == reviewId &&
                like.UserId == userId,
            token);

        return reviewLike != null && reviewLike.IsSet;
    }

    public async Task<int> GetLikesCountAsync(int reviewId, CancellationToken token)
    {
        var likes = await _reviewLikeRepository.GetAllAsync(
            like =>
                like.ReviewId == reviewId &&
                like.IsSet == true,
            token);

        return likes.Count;
    }
}