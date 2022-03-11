using System.Linq.Expressions;
using Business.Dto.Frontend.FromForm;
using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class ReviewService : IReviewService
{
    private readonly IGeneralRepository<Review> _reviewRepository;

    public ReviewService(IGeneralRepository<Review> reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<List<Review>> GetNewReviews()
    {
        var reviews = await _reviewRepository.GetAllAsyncDescending(
            review => review.Id > 0,
            review => review.CreationDate,
            CancellationToken.None);
        return reviews.Take(20).ToList();
    }

    public async Task<List<Review>> GetTopReviews()
    {
        var reviews = await _reviewRepository.GetAllAsyncDescending(
            review => review.Id > 0,
            review => review.AverageUserRating,
            CancellationToken.None
        );
        return reviews.Take(20).ToList();
    }

    public async Task<Review?> Create(ReviewForm reviewForm)
    {
        var review = new Review()
        {
            ProductName = reviewForm.ProductName,
            Title = reviewForm.Title,
            Content = reviewForm.Content,
            AuthorAssessment = reviewForm.AuthorAssessment,
            AverageUserRating = reviewForm.AuthorAssessment,
            ProductId = reviewForm.ProductId,
            AuthorId = reviewForm.AuthorId,
            StatusId = reviewForm.StatusReviewId,
            CreationDate = DateTime.Now,
            RedactionDate = DateTime.Now,
            DeletionDate = DateTime.Now,
        };

        //add images <= 3
        return await _reviewRepository.AddAsync(review, CancellationToken.None);
    }

    public async Task<Review?> GetOneIncludes(int id)
    {
        var includes = new List<Expression<Func<Review, object>>>()
        {
            review => review.Author,
            review => review.Product,
            review => review.Status,
            review => review.Comment,
            review => review.ReviewLike,
            review => review.ReviewTag,
            review => review.ReviewUserRating
        };

        return await _reviewRepository.GetOneIncludeManyAsync(
            review => review.Id == id,
            includes,
            CancellationToken.None);
    }

    public async Task<Review?> GetOne(int id)
    {
        return await _reviewRepository.GetOneAsync(review => review.Id == id, CancellationToken.None);
    }

    public async Task<List<Review>> GetAll(int userId)
    {
        return await _reviewRepository.GetAllAsync(review => review.AuthorId == userId, CancellationToken.None);
    }

    public async Task<int?> GetUserId(int reviewId)
    {
        var reviewModel = await _reviewRepository.GetOneAsync(review => review.Id == reviewId, CancellationToken.None);

        return reviewModel?.AuthorId;
    }

    public async Task UpdateAverageUserRating(int id, float averageUserRating)
    {
        var review = await GetOne(id);
        if (review != null)
        {
            review.AverageUserRating = averageUserRating;
            await _reviewRepository.UpdateAsync(review, CancellationToken.None);
        }
    }
}