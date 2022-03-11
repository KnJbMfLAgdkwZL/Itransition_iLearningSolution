namespace Business.Interfaces.Model;

public interface IReviewLikeService
{
    Task Add(int reviewId, int userId);
    Task Remove(int reviewId, int userId);
    Task<bool> IsUserLikeReview(int userId, int reviewId);
    Task<int> GetLikesCount(int reviewId);
}