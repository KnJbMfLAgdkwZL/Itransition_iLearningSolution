using Microsoft.AspNetCore.Http;

namespace Business.Interfaces;

public interface IReviewLikeService
{
    Task<bool> Add(int reviewId, HttpContext context);
    Task<bool> Remove(int reviewId, HttpContext context);
    Task<bool> IsTrue(int reviewId, HttpContext context);
}