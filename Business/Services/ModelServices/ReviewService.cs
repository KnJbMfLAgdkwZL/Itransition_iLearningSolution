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

        return await _reviewRepository.AddAsync(review, CancellationToken.None);
    }

    public async Task<Review?> Update(ReviewForm reviewForm, Review review)
    {
        review.ProductName = reviewForm.ProductName;
        review.Title = reviewForm.Title;
        review.Content = reviewForm.Content;
        review.AuthorAssessment = reviewForm.AuthorAssessment;
        review.ProductId = reviewForm.ProductId;
        review.StatusId = reviewForm.StatusReviewId;
        review.RedactionDate = DateTime.Now;

        return await _reviewRepository.UpdateAsync(review, CancellationToken.None);
    }

    public async Task<Review> Update(Review review)
    {
        return await _reviewRepository.UpdateAsync(review, CancellationToken.None);
    }

    public async Task<Review?> Delete(Review review, int deleteStatusId)
    {
        review.StatusId = deleteStatusId;
        review.DeletionDate = DateTime.Now;

        return await _reviewRepository.UpdateAsync(review, CancellationToken.None);
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

    public async Task<List<Review>> GetAllByUserId(int userId)
    {
        return await _reviewRepository.GetAllAsync(review => review.AuthorId == userId, CancellationToken.None);
    }

    public async Task<List<Review>> GetByProductId(int productId, int takeNum)
    {
        var reviews = await _reviewRepository.GetAllAsync(review => review.ProductId == productId,
            CancellationToken.None);
        var orderedEnumerable = reviews.OrderBy(review => review.AverageUserRating);
        return orderedEnumerable.Take(takeNum).ToList();
    }

    public async Task<List<Review>> GetAllIncludes(int userId)
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

        return await _reviewRepository.GetAllIncludeManyDescendingAsync(
            review => review.AuthorId == userId,
            includes,
            review => review.CreationDate,
            CancellationToken.None);
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

    public async Task<List<Review>> FullTextSearchQuery(string search)
    {
        return await _reviewRepository.FullTextSearchQueryAsync(search, CancellationToken.None);
    }
}