using Database.Models;

namespace Business.Interfaces.Model;

public interface IReviewTagService
{
    Task AddTagToReviewAsync(int reviewId, int tagId, CancellationToken token);
    Task DeleteTagsAsync(int reviewId, CancellationToken token);
    Task<List<ReviewTag>> GetTagsNamesAsync(int reviewId, CancellationToken token);
    Task<List<ReviewTag>> GetAllReviewsAsync(int tagId, CancellationToken token);
}