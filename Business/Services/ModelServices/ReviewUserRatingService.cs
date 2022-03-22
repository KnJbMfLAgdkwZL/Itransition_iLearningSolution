using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class ReviewUserRatingService : IReviewUserRatingService
{
    private readonly IGeneralRepository<ReviewUserRating> _reviewUserRatingRepository;

    public ReviewUserRatingService(
        IGeneralRepository<ReviewUserRating> reviewUserRatingRepository
    )
    {
        _reviewUserRatingRepository = reviewUserRatingRepository;
    }

    public async Task AddAssessmentAsync(int reviewId, int userId, int assessment, CancellationToken token)
    {
        await _reviewUserRatingRepository.AddOrUpdateAsync(
            rating => rating.ReviewId == reviewId && rating.UserId == userId,
            new ReviewUserRating()
            {
                ReviewId = reviewId,
                UserId = userId,
                Assessment = (byte) assessment
            }, token);
    }

    public async Task<float> GetAverageAssessmentAsync(int reviewId, CancellationToken token)
    {
        var ratings = await _reviewUserRatingRepository.GetAllAsync(
            rating => rating.ReviewId == reviewId, token);
        
        return (float) ratings.Average(v => v.Assessment);
    }
}