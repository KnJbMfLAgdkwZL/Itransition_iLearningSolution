using Business.Dto.Frontend.FromForm;
using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;

namespace Business.Services;

public class ReviewService : IReviewService
{
    private readonly IGeneralRepository<Review> _reviewRepository;
    private readonly ITagService _tagService;
    private readonly IProductGroupService _productGroupService;

    public ReviewService(IGeneralRepository<Review> reviewRepository, ITagService tagService,
        IProductGroupService productGroupService)
    {
        _reviewRepository = reviewRepository;
        _tagService = tagService;
        _productGroupService = productGroupService;
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

    public async Task<Review> Create(ReviewForm reviewForm)
    {
        //if (await _productGroupService.Check(reviewForm.ProductId))
        //{
        var r = new Review()
        {
            ProductName = reviewForm.ProductName,
            Title = reviewForm.Title,
            Content = reviewForm.Content,
            AuthorAssessment = reviewForm.AuthorAssessment,

            ProductId = reviewForm.ProductId,
            AuthorId = 0,
            StatusId = 0,

            CreationDate = DateTime.Now,
            RedactionDate = DateTime.Now,
            DeletionDate = DateTime.Now,

            AverageUserRating = 0,
        };
        //}

        //check AuthorId
        //check StatusId
        //add AuthorAssessment to AverageUserRating
        //add Tags
        //add images <= 3
        //add status from data

        //reviewForm.ProductId


        var res = await _reviewRepository.AddAsync(r, CancellationToken.None);


        return res;
    }
}