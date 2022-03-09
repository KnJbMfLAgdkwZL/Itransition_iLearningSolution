using Database.Models;

namespace Business.Interfaces;

public interface IReviewTagService
{
    Task AddTagToReview(int reviewId, int tagId);
    Task<List<ReviewTag>> GetTagsNames(int reviewId);
}