using Business.Dto.Frontend.FromForm;
using Database.Models;

namespace Business.Interfaces.Model;

public interface IReviewService
{
    void CLearContent(List<Review> reviews);
    Dictionary<int, string> GetReviewsImage(List<Review> reviews);
    string? GetImage(string html);
    string RemoveHtmlTags(string html);
    string CropStr(string str, int size);
    Task<List<Review>> GetNewReviewsAsync(CancellationToken token);
    Task<List<Review>> GetTopReviewsAsync(CancellationToken token);
    Task<List<Review>> GetReviewsByIdAsync(List<int> reviewsId, CancellationToken token);
    Task<Review?> CreateAsync(ReviewForm reviewForm, CancellationToken token);
    Task<Review?> UpdateAsync(ReviewForm reviewForm, Review review, CancellationToken token);
    Task<Review> UpdateAsync(Review review, CancellationToken token);
    Task<Review?> GetOneIncludesAsync(int id, CancellationToken token);
    Task<Review?> GetOneAsync(int id, CancellationToken token);
    Task<List<Review>> GetByProductIdAsync(int productId, int takeNum, CancellationToken token);
    Task<List<Review>> GetAllIncludesAsync(int userId, CancellationToken token);
    Task UpdateAverageUserRatingAsync(int id, float averageUserRating, CancellationToken token);
    Task<List<Review>> FullTextSearchQueryAsync(string search, CancellationToken token);
}