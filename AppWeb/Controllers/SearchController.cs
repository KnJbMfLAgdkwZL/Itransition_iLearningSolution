using Business.Interfaces.Model;
using Database.Models;
using Microsoft.AspNetCore.Mvc;

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

    private async Task SearchReviewsAsync(string search, IDictionary<int, Review> preSearch, CancellationToken token)
    {
        const int maxSizeResult = 100;
        if (preSearch.Count >= maxSizeResult)
        {
            return;
        }

        var reviews = await _reviewService.FullTextSearchQueryAsync(search, token);
        foreach (var review in reviews)
        {
            if (preSearch.ContainsKey(review.Id))
            {
                continue;
            }

            if (preSearch.Count >= maxSizeResult)
            {
                return;
            }

            preSearch.Add(review.Id, review);
        }
    }

    private async Task SearchReviewsByCommentsAsync(string search, IDictionary<int, Review> preSearch,
        CancellationToken token)
    {
        const int maxSizeResult = 100;
        if (preSearch.Count >= maxSizeResult)
        {
            return;
        }

        var comments = await _commentService.FullTextSearchQueryAsync(search, token);
        foreach (var comment in comments)
        {
            var review = await _reviewService.GetOneAsync(comment.ReviewId, token);
            if (review == null || preSearch.ContainsKey(review.Id))
            {
                continue;
            }

            if (preSearch.Count >= maxSizeResult)
            {
                return;
            }

            preSearch.Add(review.Id, review);
        }
    }

    private async Task SearchReviewsByTagsAsync(string search, IDictionary<int, Review> preSearch,
        CancellationToken token)
    {
        const int maxSizeResult = 100;
        if (preSearch.Count >= maxSizeResult)
        {
            return;
        }

        var tags = await _tagService.FullTextSearchQueryAsync(search, token);
        foreach (var tag in tags)
        {
            var reviewTags = await _reviewTagService.GetAllReviewsAsync(tag.Id, token);
            foreach (var rt in reviewTags)
            {
                var review = rt.Review;
                if (preSearch.ContainsKey(review.Id))
                {
                    continue;
                }

                if (preSearch.Count >= maxSizeResult)
                {
                    return;
                }

                preSearch.Add(review.Id, review);
            }
        }
    }

    private async Task SearchReviewsByProductGroupServiceAsync(string search, IDictionary<int, Review> preSearch,
        CancellationToken token)
    {
        const int maxSizeResult = 100;
        if (preSearch.Count >= maxSizeResult)
        {
            return;
        }

        var productGroups = await _productGroupService.FullTextSearchQueryAsync(search, token);
        foreach (var productGroup in productGroups)
        {
            var reviewsFromGroup = await _reviewService.GetByProductIdAsync(productGroup.Id, 10, token);
            foreach (var review in reviewsFromGroup)
            {
                if (preSearch.ContainsKey(review.Id))
                {
                    continue;
                }

                if (preSearch.Count >= maxSizeResult)
                {
                    return;
                }

                preSearch.Add(review.Id, review);
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

        return View(searchResult.Values.ToList());
    }
}