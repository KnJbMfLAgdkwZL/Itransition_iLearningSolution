using Business.Dto.Frontend.FromForm;
using Database.Models;
using Microsoft.AspNetCore.Http;

namespace Business.Interfaces;

public interface IReviewService
{
    Task<Dictionary<string, object>> GetMainPageData();
    Task<List<Review>> GetNewReviews();
    Task<List<Review>> GetTopReviews();
    Task<Review?> Create(ReviewForm reviewForm, HttpContext context);
    Task<Review?> Get(int id);
}