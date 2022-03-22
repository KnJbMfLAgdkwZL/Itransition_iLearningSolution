namespace Business.Interfaces.Model;

public interface IReviewUserRatingService
{
    Task AddAssessmentAsync(int reviewId, int userId, int assessment, CancellationToken token);
    Task<float> GetAverageAssessmentAsync(int reviewId, CancellationToken token);
}