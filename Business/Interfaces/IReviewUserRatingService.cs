namespace Business.Interfaces;

public interface IReviewUserRatingService
{
    Task AddAssessment(int reviewId, int userId, int assessment);
}