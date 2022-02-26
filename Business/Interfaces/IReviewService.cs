using Database.Models;

namespace Business.Interfaces;

public interface IReviewService
{
    Task<Dictionary<string, object>> GetMainPageData();
    Task<List<Review>> GetNewReviews();
    Task<List<Review>> GetTopReviews();
}