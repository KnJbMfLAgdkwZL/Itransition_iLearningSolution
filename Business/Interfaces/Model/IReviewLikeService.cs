namespace Business.Interfaces.Model;

public interface IReviewLikeService
{
    Task AddAsync(int reviewId, int userId, CancellationToken token);
    Task RemoveAsync(int reviewId, int userId, CancellationToken token);
    Task<bool> IsUserLikeReviewAsync(int userId, int reviewId, CancellationToken token);
    Task<int> GetLikesCountAsync(int reviewId, CancellationToken token);
}