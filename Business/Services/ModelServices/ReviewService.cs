using System.Linq.Expressions;
using Business.Dto.Frontend.FromForm;
using Business.Interfaces.Model;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services.ModelServices;

public class ReviewService : IReviewService
{
    private readonly IGeneralRepository<Review> _reviewRepository;

    public ReviewService(
        IGeneralRepository<Review> reviewRepository
    )
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<List<Review>> GetNewReviewsAsync(CancellationToken token)
    {
        var reviews = await _reviewRepository.GetAllDescendingAsync(
            review => review.Id > 0,
            review => review.CreationDate,
            token);

        var includes = new List<Expression<Func<Review, object>>>()
        {
            //review => review.Author,
            review => review.Product,
            //review => review.Status,
            //review => review.ReviewLike,
            //review => review.ReviewTag,
            //review => review.ReviewUserRating
        };

        await _reviewRepository.GetAllIncludeManyDescendingAsync(
            review => review.Status.Name != "Deleted",
            includes,
            review => review.CreationDate,
            token);
        
        return reviews.Take(20).ToList();
    }

    public async Task<List<Review>> GetTopReviewsAsync(CancellationToken token)
    {
        var reviews = await _reviewRepository.GetAllDescendingAsync(
            review => review.Id > 0,
            review => review.AverageUserRating,
            token);

        return reviews.Take(30).ToList();
    }

    public async Task<Review?> CreateAsync(ReviewForm reviewForm, CancellationToken token)
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

        return await _reviewRepository.AddAsync(review, token);
    }

    public async Task<Review?> UpdateAsync(ReviewForm reviewForm, Review review, CancellationToken token)
    {
        review.ProductName = reviewForm.ProductName;
        review.Title = reviewForm.Title;
        review.Content = reviewForm.Content;
        review.AuthorAssessment = reviewForm.AuthorAssessment;
        review.ProductId = reviewForm.ProductId;
        review.StatusId = reviewForm.StatusReviewId;
        review.RedactionDate = DateTime.Now;

        return await _reviewRepository.UpdateAsync(review, token);
    }

    public async Task<Review> UpdateAsync(Review review, CancellationToken token)
    {
        return await _reviewRepository.UpdateAsync(review, token);
    }

    public async Task<Review?> GetOneIncludesAsync(int id, CancellationToken token)
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

        return await _reviewRepository.GetOneIncludeManyAsync(review => review.Id == id, includes, token);
    }

    public async Task<Review?> GetOneAsync(int id, CancellationToken token)
    {
        return await _reviewRepository.GetOneAsync(review => review.Id == id, token);
    }

    public async Task<List<Review>> GetByProductIdAsync(int productId, int takeNum, CancellationToken token)
    {
        var reviews = await _reviewRepository.GetAllAsync(review => review.ProductId == productId, token);

        var orderedEnumerable = reviews.OrderBy(review => review.AverageUserRating);

        return orderedEnumerable.Take(takeNum).ToList();
    }

    public async Task<List<Review>> GetAllIncludesAsync(int userId, CancellationToken token)
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

        return await _reviewRepository.GetAllIncludeManyDescendingAsync(review => review.AuthorId == userId, includes,
            review => review.CreationDate, token);
    }

    public async Task UpdateAverageUserRatingAsync(int id, float averageUserRating, CancellationToken token)
    {
        var review = await GetOneAsync(id, token);
        if (review != null)
        {
            review.AverageUserRating = averageUserRating;
            await _reviewRepository.UpdateAsync(review, token);
        }
    }

    public async Task<List<Review>> FullTextSearchQueryAsync(string search, CancellationToken token)
    {
        return await _reviewRepository.FullTextSearchQueryAsync(search, token);
    }
}