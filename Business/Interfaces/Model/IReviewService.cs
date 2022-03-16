using Business.Dto.Frontend.FromForm;
using Database.Models;

namespace Business.Interfaces.Model;

public interface IReviewService
{
    Task<List<Review>> GetNewReviews();
    Task<List<Review>> GetTopReviews();
    Task<Review?> Create(ReviewForm reviewForm);
    Task<Review?> Update(ReviewForm reviewForm, Review review);
    Task<Review> Update(Review review);
    Task<Review?> Delete(Review review, int deleteStatusId);
    Task<Review?> GetOneIncludes(int id);
    Task<Review?> GetOne(int id);
    Task<List<Review>> GetAllByUserId(int userId);
    Task<List<Review>> GetByProductId(int productId, int takeNum);
    Task<List<Review>> GetAllIncludes(int userId);
    Task<int?> GetUserId(int reviewId);
    Task UpdateAverageUserRating(int id, float averageUserRating);
    Task<List<Review>> FullTextSearchQuery(string search);
}