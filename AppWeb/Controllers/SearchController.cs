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

    public SearchController(IReviewService reviewService, IProductGroupService productGroupService,
        ICommentService commentService, ITagService tagService, IReviewTagService reviewTagService)
    {
        _reviewService = reviewService;
        _productGroupService = productGroupService;
        _commentService = commentService;
        _tagService = tagService;
        _reviewTagService = reviewTagService;
    }

    private async Task SearchReviews(string search, IDictionary<int, Review> preSearch)
    {
        const int maxSizeResult = 100;
        if (preSearch.Count >= maxSizeResult)
        {
            return;
        }

        var reviews = await _reviewService.FullTextSearchQuery(search);
        foreach (var review in reviews)
        {
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

    async Task SearchReviewsByComments(string search, Dictionary<int, Review> preSearch)
    {
        const int maxSizeResult = 100;
        if (preSearch.Count >= maxSizeResult)
        {
            return;
        }

        var comments = await _commentService.FullTextSearchQuery(search);
        foreach (var comment in comments)
        {
            var review = await _reviewService.GetOne(comment.ReviewId);
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

    async Task SearchReviewsByTags(string search, Dictionary<int, Review> preSearch)
    {
        const int maxSizeResult = 100;
        if (preSearch.Count >= maxSizeResult)
        {
            return;
        }

        var tags = await _tagService.FullTextSearchQuery(search);
        foreach (var tag in tags)
        {
            var reviewTags = await _reviewTagService.GetAllReviews(tag.Id);
            foreach (var rt in reviewTags)
            {
                var review = rt.Review;
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
    }

    async Task SearchReviewsByProductGroupService(string search, Dictionary<int, Review> preSearch)
    {
        const int maxSizeResult = 100;
        if (preSearch.Count >= maxSizeResult)
        {
            return;
        }

        var productGroups = await _productGroupService.FullTextSearchQuery(search);
        foreach (var productGroup in productGroups)
        {
            var reviewsFromGroup = await _reviewService.GetByProductId(productGroup.Id, 10);
            foreach (var review in reviewsFromGroup)
            {
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
    }

    [HttpGet]
    public async Task<IActionResult> FullTextSearch([FromQuery] string search)
    {
        var searchResult = new Dictionary<int, Review>();
        await SearchReviews(search, searchResult);
        await SearchReviewsByComments(search, searchResult);
        await SearchReviewsByTags(search, searchResult);
        await SearchReviewsByProductGroupService(search, searchResult);
        return View(searchResult.Values.ToList());
    }
}