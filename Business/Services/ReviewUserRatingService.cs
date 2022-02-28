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
}
