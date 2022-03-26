using Business.Interfaces.Model;
using Database.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWeb.Controllers;

public class SearchController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly ICommentService _commentService;
    private readonly ITagService _tagService;
    private readonly IReviewTagService _reviewTagService;
    private readonly IProductGroupService _productGroupService;

    public SearchController(
        IReviewService reviewService,
        IProductGroupService productGroupService,
        ICommentService commentService,
        ITagService tagService,
        IReviewTagService reviewTagService
    )
    {
        _reviewService = reviewService;
        _productGroupService = productGroupService;
        _commentService = commentService;
        _tagService = tagService;
        _reviewTagService = reviewTagService;
    }

    private bool AddReviews(Review review, IDictionary<int, Review> preSearch)
    {
        const int maxSizeResult = 10;
        if (preSearch.Count >= maxSizeResult)
        {
            return false;
        }

        if (!preSearch.ContainsKey(review.Id))
        {
            preSearch.Add(review.Id, review);
        }

        return true;
    }

    private async Task SearchReviewsAsync(string search, IDictionary<int, Review> preSearch, CancellationToken token)
    {
        var reviews = await _reviewService.FullTextSearchQueryAsync(search, token);
        foreach (var review in reviews)
        {
            if (!AddReviews(review, preSearch))
            {
                return;
            }
        }
    }

    private async Task SearchReviewsByCommentsAsync(string search, IDictionary<int, Review> preSearch,
        CancellationToken token)
    {
        var comments = await _commentService.FullTextSearchQueryAsync(search, token);
        foreach (var comment in comments)
        {
            var review = await _reviewService.GetOneAsync(comment.ReviewId, token);
            if (review != null)
            {
                if (!AddReviews(review, preSearch))
                {
                    return;
                }
            }
        }
    }

    private async Task SearchReviewsByTagsAsync(string search, IDictionary<int, Review> preSearch,
        CancellationToken token)
    {
        var tags = await _tagService.FullTextSearchQueryAsync(search, token);
        foreach (var tag in tags)
        {
            var reviewTags = await _reviewTagService.GetAllReviewsAsync(tag.Id, token);
            foreach (var rt in reviewTags)
            {
                if (!AddReviews(rt.Review, preSearch))
                {
                    return;
                }
            }
        }
    }

    private async Task SearchReviewsByProductGroupServiceAsync(string search, IDictionary<int, Review> preSearch,
        CancellationToken token)
    {
        var productGroups = await _productGroupService.FullTextSearchQueryAsync(search, token);
        foreach (var productGroup in productGroups)
        {
            var reviewsFromGroup = await _reviewService.GetByProductIdAsync(productGroup.Id, 10, token);
            foreach (var review in reviewsFromGroup)
            {
                if (!AddReviews(review, preSearch))
                {
                    return;
                }
            }
        }
    }

    [HttpGet]
    public async Task<IActionResult> FullTextSearchAsync([FromQuery] string search, CancellationToken token)
    {
        var searchResult = new Dictionary<int, Review>();

        await SearchReviewsAsync(search, searchResult, token);
        await SearchReviewsByCommentsAsync(search, searchResult, token);
        await SearchReviewsByTagsAsync(search, searchResult, token);
        await SearchReviewsByProductGroupServiceAsync(search, searchResult, token);

        var reviewsId = searchResult.Values.Select(review => review.Id).ToList();

        var settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = (sender, args) => { args.ErrorContext.Handled = true; },
        };
        ViewData["reviewsId"] = JsonConvert.SerializeObject(reviewsId, Formatting.Indented, settings);
        return View();
    }
}