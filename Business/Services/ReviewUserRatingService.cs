using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;

public class ReviewUserRatingService : IReviewUserRatingService
{
    private readonly IGeneralRepository<ReviewUserRating> _reviewUserRatingRepository;

    public ReviewUserRatingService(IGeneralRepository<ReviewUserRating> reviewUserRatingRepository)
    {
        _reviewUserRatingRepository = reviewUserRatingRepository;
    }

    public async Task AddAssessment(int reviewId, int userId, int assessment)
    {
        await _reviewUserRatingRepository.AddIfNotExistAsync(r =>
                r.ReviewId == reviewId && r.UserId == userId,
            new ReviewUserRating()
            {
                ReviewId = reviewId,
                UserId = userId,
                Assessment = (byte) assessment
            }, CancellationToken.None);
    }
}