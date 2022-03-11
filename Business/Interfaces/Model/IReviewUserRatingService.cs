namespace Business.Interfaces.Model;

public interface IReviewUserRatingService
{
    Task AddAssessment(int reviewId, int userId, int assessment);
    Task<float> GetAverageAssessment(int reviewId);
}