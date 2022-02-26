using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;

public class ReviewService : IReviewService
{
    private readonly IGeneralRepository<Review> _reviewRepository;
    private readonly ITagService _tagService;

    public ReviewService(IGeneralRepository<Review> reviewRepository, ITagService tagService)
    {
        _reviewRepository = reviewRepository;
        _tagService = tagService;
    }

    public async Task<Dictionary<string, object>> GetMainPageData()
    {
        return new Dictionary<string, object>
        {
            {"NewReviews", await GetNewReviews()},
            {"TopReviews", await GetTopReviews()},
            {"TopTags", await _tagService.GetTopTags()}
        };
    }

    public async Task<List<Review>> GetNewReviews()
    {
        var r = await _reviewRepository.GetAllAsyncDescending(r =>
                r.StatusId == 1,
            r =>
                r.CreationDate,
            CancellationToken.None
        );
        return r.Take(20).ToList();
    }

    public async Task<List<Review>> GetTopReviews()
    {
        var r = await _reviewRepository.GetAllAsyncDescending(r =>
                r.StatusId == 1,
            r =>
                r.AverageUserRating,
            CancellationToken.None
        );
        return r.Take(20).ToList();
    }
}