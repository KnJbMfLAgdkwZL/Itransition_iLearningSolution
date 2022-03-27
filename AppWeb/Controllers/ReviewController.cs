using Business.Dto.Frontend.FromForm;
using Business.Interfaces;
using Business.Interfaces.Model;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWeb.Controllers;

public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly IProductGroupService _productGroupService;
    private readonly IStatusReviewService _statusReviewService;
    private readonly IReviewTagService _reviewTagService;
    private readonly IReviewLikeService _reviewLikeService;
    private readonly IReviewUserRatingService _reviewUserRatingService;
    private readonly ITagService _tagService;
    private readonly IUserService _userService;
    private readonly IAccountService _accountService;

    public ReviewController(
        IReviewService reviewService,
        IProductGroupService productGroupService,
        IStatusReviewService statusReviewService,
        IReviewTagService reviewTagService,
        IReviewLikeService reviewLikeService,
        IUserService userService,
        IReviewUserRatingService reviewUserRatingService,
        ITagService tagService,
        IAccountService accountService
    )
    {
        _reviewService = reviewService;
        _productGroupService = productGroupService;
        _statusReviewService = statusReviewService;
        _reviewTagService = reviewTagService;
        _reviewLikeService = reviewLikeService;
        _userService = userService;
        _reviewUserRatingService = reviewUserRatingService;
        _tagService = tagService;
        _accountService = accountService;
    }

    private async Task SetReviews(List<Review> reviews, CancellationToken token)
    {
        ViewData["reviewsImage"] = _reviewService.GetReviewsImage(reviews);
        foreach (var review in reviews)
        {
            review.ReviewTag = await _reviewTagService.GetTagsNamesAsync(review.Id, token);
            review.ReviewLike = review.ReviewLike.Where(like => like.IsSet).ToList();
        }

        _reviewService.CLearContent(reviews);
        ViewData["reviews"] = reviews;
    }

    public async Task<IActionResult> GetNewReviewsAsync(CancellationToken token)
    {
        var reviews = await _reviewService.GetNewReviewsAsync(token);
        await SetReviews(reviews, token);
        return PartialView("_GetReviews");
    }

    public async Task<IActionResult> GetTopReviewsAsync(CancellationToken token)
    {
        var reviews = await _reviewService.GetTopReviewsAsync(token);
        await SetReviews(reviews, token);
        return PartialView("_GetReviews");
    }

    public async Task<IActionResult> GetReviewsByIdAsync(string json, CancellationToken token)
    {
        var reviewsId = JsonConvert.DeserializeObject<List<int>>(json);
        if (reviewsId == null)
        {
            return BadRequest("Wrong json, reviewsId is null");
        }

        var reviews = await _reviewService.GetReviewsByIdAsync(reviewsId, token);
        await SetReviews(reviews, token);
        return PartialView("_GetReviews");
    }

    public async Task<IActionResult> GetReviewsByTagAsync(int id, CancellationToken token)
    {
        var reviews = await _reviewService.GetReviewsByTagAsync(id, token);
        await SetReviews(reviews, token);
        return PartialView("_GetReviews");
    }

    public async Task<IActionResult> GetReviewsByAuthorAsync(int id, CancellationToken token)
    {
        var reviews = await _reviewService.GetReviewsByAuthorAsync(id, token);
        await SetReviews(reviews, token);
        return PartialView("_GetReviews");
    }

    public async Task<IActionResult> GetReviewsByProductAsync(int id, CancellationToken token)
    {
        var reviews = await _reviewService.GetReviewsByProductAsync(id, token);
        await SetReviews(reviews, token);
        return PartialView("_GetReviews");
    }

    [Authorize(Roles = "Admin, User")]
    [HttpGet]
    public async Task<IActionResult> CreateOrUpdateAsync([FromQuery] int userId, [FromQuery] int reviewId,
        CancellationToken token)
    {
        var user = _accountService.GetAuthorizedUser(HttpContext, out var error, token);
        if (user == null)
        {
            return error!;
        }

        if (userId > 0 && user.Role.Name == "Admin" && user.Id != userId)
        {
            user = await _userService.GetUserByIdAsync(userId, token);
            if (user == null)
            {
                return NotFound("User NotFound");
            }
        }

        ViewData["userId"] = userId;
        ViewData["reviewId"] = reviewId;
        ViewData["Role"] = user.Role.Name;

        ViewData["productGroups"] = await _productGroupService.GetAllAsync(token);

        var statusReviews = await _statusReviewService.GetAllAsync(token);
        ViewData["statusReviews"] = statusReviews.Where(status => status.Name != "Deleted").ToList();

        ViewData["review"] = null;
        ViewData["tags"] = JsonConvert.SerializeObject(new List<string>());

        if (reviewId > 0) // Update
        {
            var review = await _reviewService.GetOneIncludesAsync(reviewId, token);
            if (review == null)
            {
                return NotFound("Review NotFound");
            }

            if (review.AuthorId != user.Id && user.Role.Name != "Admin")
            {
                return BadRequest("You are not the author of this review");
            }

            var statusReview = await _statusReviewService.GetAsync("Deleted", token);
            if (statusReview == null)
            {
                return BadRequest("StatusReview Deleted not found");
            }

            if (review.StatusId == statusReview.Id && user.Role.Name != "Admin")
            {
                return BadRequest("Review Deleted");
            }

            ViewData["review"] = review;

            var tags = await _reviewTagService.GetTagsNamesAsync(review.Id, token);
            var tagsNames = tags.Select(tag => tag.Tag.Name).ToList();
            ViewData["tags"] = JsonConvert.SerializeObject(tagsNames);
        }

        return View();
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateAsync([FromForm] ReviewForm reviewForm, CancellationToken token)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Form not Valid");
        }

        if (!await _productGroupService.CheckAsync(reviewForm.ProductId, token))
        {
            return BadRequest("Wrong ProductGroup");
        }

        if (!await _statusReviewService.CheckAsync(reviewForm.StatusReviewId, token))
        {
            return BadRequest("Wrong StatusReview");
        }

        var currentUser = _accountService.GetAuthorizedUser(HttpContext, out var error, token);
        if (currentUser == null)
        {
            return error!;
        }

        var statusReviewDeleted = await _statusReviewService.GetAsync("Deleted", token);
        if (statusReviewDeleted != null && statusReviewDeleted.Id == reviewForm.StatusReviewId &&
            currentUser.Role.Name != "Admin")
        {
            return BadRequest("Wrong StatusReview");
        }

        Review? review = null;

        if (reviewForm.AuthorId <= 0 && reviewForm.Id <= 0) // Create
        {
            if (currentUser.Role.Name == "Admin" && reviewForm.UserId > 0) // Admin Created on behalf User
            {
                var userAuthor = await _userService.GetUserByIdAsync(reviewForm.UserId, token);
                if (userAuthor == null)
                {
                    return NotFound("UserAuthor NotFound");
                }

                reviewForm.AuthorId = userAuthor.Id; // Create review by Admin on behalf of another user
            }
            else
            {
                reviewForm.AuthorId = currentUser.Id; // Admin as user or user, created his review
            }

            review = await _reviewService.CreateAsync(reviewForm, token);
            if (review == null)
            {
                return BadRequest("Error reviewService.Create");
            }
        }
        else if (reviewForm.AuthorId > 0 && reviewForm.Id > 0) // Update
        {
            var reviewUpdate = await _reviewService.GetOneIncludesAsync(reviewForm.Id, token);
            if (reviewUpdate == null)
            {
                return NotFound("Wrong reviewForm.Id");
            }

            if (currentUser.Role.Name != "Admin" && reviewUpdate.AuthorId != currentUser.Id)
            {
                return BadRequest("You are not the author of this review");
            }

            if (reviewUpdate!.StatusId == statusReviewDeleted!.Id && currentUser.Role.Name != "Admin")
            {
                return BadRequest("Review Deleted");
            }

            review = await _reviewService.UpdateAsync(reviewForm, reviewUpdate, token);
            if (review == null)
            {
                return BadRequest("Error reviewService.Update");
            }
        }

        if (review != null)
        {
            await _reviewUserRatingService.AddAssessmentAsync(review.Id, review.AuthorId, review.AuthorAssessment,
                token);

            await _reviewTagService.DeleteTagsAsync(review.Id, token);

            var tags = JsonConvert.DeserializeObject<List<string>>(reviewForm.TagsInput);
            if (tags != null)
            {
                foreach (var tagName in tags)
                {
                    var tag = await _tagService.AddOrIncrementAsync(tagName, token);

                    await _reviewTagService.AddTagToReviewAsync(review.Id, tag.Id, token);
                }
            }

            return RedirectToAction("Get", "Review", new {id = review.Id});
        }

        return BadRequest();
    }

    public async Task<IActionResult> GetAsync([FromRoute] int id, CancellationToken token)
    {
        var review = await _reviewService.GetOneIncludesAsync(id, token);
        if (review == null)
        {
            return BadRequest("Wrong reviewId");
        }

        ViewData["IsUserLike"] = false;
        ViewData["Assessment"] = null;
        var user = _accountService.GetAuthorizedUser(HttpContext, out var error, token);
        if (user != null)
        {
            ViewData["IsUserLike"] = await _reviewLikeService.IsUserLikeReviewAsync(user.Id, id, token);

            var reviewUserRating = await _reviewUserRatingService.GetAsync(review.Id, user.Id, token);
            if (reviewUserRating != null)
            {
                ViewData["Assessment"] = reviewUserRating.Assessment;
            }
        }

        var statusReview = await _statusReviewService.GetAsync("Deleted", token);
        if (statusReview == null)
        {
            return BadRequest("StatusReview Deleted not found");
        }

        if (review.StatusId == statusReview.Id)
        {
            if (user == null || user.Role.Name != "Admin")
            {
                return BadRequest("Review Deleted");
            }
        }

        review.ReviewTag = await _reviewTagService.GetTagsNamesAsync(review.Id, token);
        review.ReviewLike = review.ReviewLike.Where(like => like.IsSet).ToList();
        ViewData["review"] = review;
        return View();
    }

    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id, CancellationToken token)
    {
        var user = _accountService.GetAuthorizedUser(HttpContext, out var error, token);
        if (user == null)
        {
            return error!;
        }

        var review = await _reviewService.GetOneIncludesAsync(id, token);
        if (review == null)
        {
            return BadRequest("Wrong reviewId");
        }

        if (review.AuthorId != user.Id && user.Role.Name != "Admin")
        {
            return BadRequest("You are not the author of this review");
        }

        var statusReview = await _statusReviewService.GetAsync("Deleted", token);
        if (statusReview == null)
        {
            return BadRequest("StatusReview Deleted not found");
        }

        if (review.StatusId == statusReview.Id)
        {
            return BadRequest("Review Deleted");
        }

        review.StatusId = statusReview.Id;

        await _reviewService.UpdateAsync(review, token);

        return View();
    }
}