using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class ReviewUserRatingService : IReviewUserRatingService
{
    private readonly IGeneralRepository<ReviewUserRating> _reviewUserRatingRepository;

    public ReviewUserRatingService(IGeneralRepository<ReviewUserRating> reviewUserRatingRepository)
    {
        _reviewUserRatingRepository = reviewUserRatingRepository;
    }

    public async Task AddAssessment(int reviewId, int userId, int assessment)
    {
        await _reviewUserRatingRepository.AddOrUpdateAsync(
            rating => rating.ReviewId == reviewId && rating.UserId == userId,
            new ReviewUserRating()
            {
                ReviewId = reviewId,
                UserId = userId,
                Assessment = (byte) assessment
            }, CancellationToken.None);
    }

    public async Task<float> GetAverageAssessment(int reviewId)
    {
        var ratings = await _reviewUserRatingRepository.GetAllAsync(
            rating => rating.ReviewId == reviewId, CancellationToken.None);
        return (float) ratings.Average(v => v.Assessment);
    }
}