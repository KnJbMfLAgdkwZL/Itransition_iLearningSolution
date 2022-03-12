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

    private readonly IUserSocialService _userSocialService;
    private readonly IUserClaimsService _userClaimsService;
    private readonly IUserService _userService;

    public ReviewController(IReviewService reviewService, IProductGroupService productGroupService,
        IStatusReviewService statusReviewService, IReviewTagService reviewTagService,
        IReviewLikeService reviewLikeService, IUserClaimsService userClaimsService, IUserService userService,
        IUserSocialService userSocialService, IReviewUserRatingService reviewUserRatingService, ITagService tagService)
    {
        _reviewService = reviewService;
        _productGroupService = productGroupService;
        _statusReviewService = statusReviewService;
        _reviewTagService = reviewTagService;
        _reviewLikeService = reviewLikeService;
        _userClaimsService = userClaimsService;
        _userService = userService;
        _userSocialService = userSocialService;
        _reviewUserRatingService = reviewUserRatingService;
        _tagService = tagService;
    }

    private User? GetAuthorizedUser(out IActionResult? error)
    {
        var userClaims = _userClaimsService.GetClaims(HttpContext);
        var userSocial = _userSocialService.Get(userClaims).Result;
        if (userSocial == null)
        {
            error = BadRequest("UserSocial not found");
            return null;
        }

        var user = _userService.GetUserBySocialId(userSocial.Id).Result;
        if (user == null)
        {
            error = BadRequest("User not found");
            return null;
        }

        error = null;
        return user;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["productGroups"] = await _productGroupService.GetAll();
        ViewData["statusReviews"] = await _statusReviewService.GetAll();
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ReviewForm reviewForm)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Form not Valid");
        }

        if (!await _productGroupService.Check(reviewForm.ProductId))
        {
            return BadRequest("Wrong ProductGroup");
        }

        if (!await _statusReviewService.Check(reviewForm.StatusReviewId))
        {
            return BadRequest("Wrong StatusReview");
        }

        var status = await _statusReviewService.Get("Deleted");
        if (status != null && status.Id == reviewForm.StatusReviewId)
        {
            return BadRequest("Wrong StatusReview");
        }

        var user = GetAuthorizedUser(out var error);
        if (user == null)
        {
            return error!;
        }

        reviewForm.AuthorId = user.Id;
        var review = await _reviewService.Create(reviewForm);
        if (review == null)
        {
            return BadRequest();
        }

        await _reviewUserRatingService.AddAssessment(review.Id, review.AuthorId, review.AuthorAssessment);

        var tags = JsonConvert.DeserializeObject<List<string>>(reviewForm.TagsInput);
        if (tags != null)
        {
            foreach (var tagName in tags)
            {
                var tag = await _tagService.AddOrIncrement(tagName);
                await _reviewTagService.AddTagToReview(review.Id, tag.Id);
            }
        }

        return RedirectToAction("Get", "Review", new {id = review.Id});
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit([FromRoute] int id)
    {
        var user = GetAuthorizedUser(out var error);
        if (user == null)
        {
            return error!;
        }

        var review = await _reviewService.GetOneIncludes(id);
        if (review == null)
        {
            return BadRequest("Wrong reviewId");
        }

        if (review.AuthorId != user.Id)
        {
            return BadRequest("You are not the author of this review");
        }

        ViewData["productGroups"] = await _productGroupService.GetAll();
        ViewData["statusReviews"] = await _statusReviewService.GetAll();
        ViewData["review"] = review;

        var tags = await _reviewTagService.GetTagsNames(review.Id);
        var tagsNames = tags.Select(tag => tag.Tag.Name).ToList();
        ViewData["tags"] = JsonConvert.SerializeObject(tagsNames);

        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Edit([FromForm] ReviewForm reviewForm)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Form not Valid");
        }

        if (!await _productGroupService.Check(reviewForm.ProductId))
        {
            return BadRequest("Wrong ProductGroup");
        }

        if (!await _statusReviewService.Check(reviewForm.StatusReviewId))
        {
            return BadRequest("Wrong StatusReview");
        }

        var status = await _statusReviewService.Get("Deleted");
        if (status != null && status.Id == reviewForm.StatusReviewId)
        {
            return BadRequest("Wrong StatusReview");
        }

        var user = GetAuthorizedUser(out var error);
        if (user == null)
        {
            return error!;
        }

        var review = await _reviewService.GetOneIncludes(reviewForm.Id);
        if (review == null)
        {
            return BadRequest("Wrong reviewId");
        }

        if (!(reviewForm.AuthorId == user.Id && user.Id == review.AuthorId))
        {
            return BadRequest("You are not the author of this review");
        }

        var updatedReview = await _reviewService.Update(reviewForm, review);
        if (updatedReview == null)
        {
            return BadRequest();
        }


        await _reviewUserRatingService.AddAssessment(updatedReview.Id, updatedReview.AuthorId,
            updatedReview.AuthorAssessment);

        await _reviewTagService.DeleteTags(updatedReview.Id);

        var tags = JsonConvert.DeserializeObject<List<string>>(reviewForm.TagsInput);
        if (tags != null)
        {
            foreach (var tagName in tags)
            {
                var tag = await _tagService.AddOrIncrement(tagName);

                await _reviewTagService.AddTagToReview(review.Id, tag.Id);
            }
        }

        return RedirectToAction("Get", "Review", new {id = review.Id});
    }

    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var review = await _reviewService.GetOneIncludes(id);
        if (review == null)
        {
            return BadRequest("Wrong reviewId");
        }

        ViewData["review"] = review;
        ViewData["tags"] = await _reviewTagService.GetTagsNames(review.Id);
        ViewData["IsUserLike"] = false;

        var user = GetAuthorizedUser(out var error);
        if (user != null)
        {
            ViewData["IsUserLike"] = await _reviewLikeService.IsUserLikeReview(user.Id, id);
        }

        return View();
    }
}