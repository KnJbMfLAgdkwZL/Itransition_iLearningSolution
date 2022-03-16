using Database.Models;

namespace Business.Interfaces.Model;

public interface IReviewTagService
{
    Task AddTagToReview(int reviewId, int tagId);
    Task DeleteTags(int reviewId);
    Task<List<ReviewTag>> GetTagsNames(int reviewId);
    Task<List<ReviewTag>> GetAllReviews(int tagId);
}