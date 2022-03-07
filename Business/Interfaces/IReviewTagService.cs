namespace Business.Interfaces;

public interface IReviewTagService
{
    Task AddTagToReview(int reviewId, int tagId);
}