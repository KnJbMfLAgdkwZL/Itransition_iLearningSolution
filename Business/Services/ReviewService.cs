using Business.Dto.Frontend.FromForm;
using Business.Interfaces;
using DataAccess.Interfaces;
using Database.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Business.Services;

public class ReviewService : IReviewService
{
    private readonly IGeneralRepository<Review> _reviewRepository;
    private readonly ITagService _tagService;
    private readonly IReviewTagService _reviewTagService;
    private readonly IProductGroupService _productGroupService;
    private readonly IUserClaimsService _userClaimsService;
    private readonly IUserService _userService;
    private readonly IStatusReviewService _statusReviewService;
    private readonly IReviewUserRatingService _reviewUserRatingService;

    public ReviewService(IGeneralRepository<Review> reviewRepository, ITagService tagService,
        IProductGroupService productGroupService, IUserClaimsService userClaimsService, IUserService userService,
        IStatusReviewService statusReviewService, IReviewTagService reviewTagService,
        IReviewUserRatingService reviewUserRatingService)
    {
        _reviewRepository = reviewRepository;
        _tagService = tagService;
        _productGroupService = productGroupService;
        _userClaimsService = userClaimsService;
        _userService = userService;
        _statusReviewService = statusReviewService;
        _reviewTagService = reviewTagService;
        _reviewUserRatingService = reviewUserRatingService;
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

    public async Task<Review?> Create(ReviewForm reviewForm, HttpContext context)
    {
        if (!await _productGroupService.Check(reviewForm.ProductId))
        {
            return null;
        }

        var userClaims = _userClaimsService.GetClaims(context);
        var user = await _userService.GetUser(userClaims);
        if (user == null)
        {
            return null;
        }

        if (!await _statusReviewService.Check(reviewForm.StatusReviewId))
        {
            return null;
        }

        //add images <= 3
        var review = await _reviewRepository.AddAsync(new Review()
        {
            ProductName = reviewForm.ProductName,
            Title = reviewForm.Title,
            Content = reviewForm.Content,
            AuthorAssessment = reviewForm.AuthorAssessment,
            AverageUserRating = reviewForm.AuthorAssessment,
            ProductId = reviewForm.ProductId,
            AuthorId = user.Id,
            StatusId = reviewForm.StatusReviewId,
            CreationDate = DateTime.Now,
            RedactionDate = DateTime.Now,
            DeletionDate = DateTime.Now,
        }, CancellationToken.None);

        await _reviewUserRatingService.AddAssessment(review.Id, user.Id, reviewForm.AuthorAssessment);

        var tags = JsonConvert.DeserializeObject<List<string>>(reviewForm.TagsInput);
        if (tags != null)
        {
            foreach (var tagName in tags)
            {
                var tag = await _tagService.AddOrIncrement(tagName);
                await _reviewTagService.AddTagToReview(review.Id, tag.Id);
            }
        }

        return review;
    }
}