using Business.Dto.Frontend.FromForm;
using Business.Interfaces;
using Business.Interfaces.Model;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWeb.Controllers;

[Authorize]
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
    public async Task<IActionResult> CreateOrUpdate([FromQuery] int userId, [FromQuery] int reviewId)
    {
        var user = GetAuthorizedUser(out var error);
        if (user == null)
        {
            return error!;
        }

        if (userId > 0 && user.Role.Name == "Admin" && user.Id != userId)
        {
            user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound("User NotFound");
            }
        }

        ViewData["userId"] = userId;
        ViewData["reviewId"] = reviewId;
        ViewData["Role"] = user.Role.Name;

        ViewData["productGroups"] = await _productGroupService.GetAll();

        var statusReviews = await _statusReviewService.GetAll();
        ViewData["statusReviews"] = statusReviews.Where(status => status.Name != "Deleted").ToList();

        ViewData["review"] = null;
        ViewData["tags"] = JsonConvert.SerializeObject(new List<string>());

        if (reviewId > 0) // Update
        {
            var review = await _reviewService.GetOneIncludes(reviewId);
            if (review == null)
            {
                return NotFound("Review NotFound");
            }

            if (review.AuthorId != user.Id && user.Role.Name != "Admin")
            {
                return BadRequest("You are not the author of this review");
            }

            var statusReview = await _statusReviewService.Get("Deleted");
            if (statusReview == null)
            {
                return BadRequest("StatusReview Deleted not found");
            }

            if (review.StatusId == statusReview.Id && user.Role.Name != "Admin")
            {
                return BadRequest("Review Deleted");
            }

            ViewData["review"] = review;
            var tags = await _reviewTagService.GetTagsNames(review.Id);
            var tagsNames = tags.Select(tag => tag.Tag.Name).ToList();
            ViewData["tags"] = JsonConvert.SerializeObject(tagsNames);
        }

        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateOrUpdate([FromForm] ReviewForm reviewForm)
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

        var currentUser = GetAuthorizedUser(out var error);
        if (currentUser == null)
        {
            return error!;
        }

        var statusReviewDeleted = await _statusReviewService.Get("Deleted");
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
                var userAuthor = await _userService.GetUserById(reviewForm.UserId);
                if (userAuthor == null)
                {
                    return NotFound("UserAuthor NotFound");
                }

                reviewForm.AuthorId = userAuthor.Id; // Создать обзор Админом от лица другого пользователя
            }
            else
            {
                reviewForm.AuthorId = currentUser.Id; // Админ как пользователь или пользователь, создал свой обзор
            }

            review = await _reviewService.Create(reviewForm);
            if (review == null)
            {
                return BadRequest("Error reviewService.Create");
            }
        }
        else if (reviewForm.AuthorId > 0 && reviewForm.Id > 0) // Update
        {
            var reviewUpdate = await _reviewService.GetOneIncludes(reviewForm.Id);
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

            review = await _reviewService.Update(reviewForm, reviewUpdate);
            if (review == null)
            {
                return BadRequest("Error reviewService.Update");
            }
        }

        if (review != null)
        {
            await _reviewUserRatingService.AddAssessment(review.Id, review.AuthorId, review.AuthorAssessment);
            await _reviewTagService.DeleteTags(review.Id);

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

        return BadRequest();
    }

    public async Task<IActionResult> Get([FromRoute] int id)
    {
        var review = await _reviewService.GetOneIncludes(id);
        if (review == null)
        {
            return BadRequest("Wrong reviewId");
        }

        var user = GetAuthorizedUser(out var error);
        if (user != null)
        {
            ViewData["IsUserLike"] = await _reviewLikeService.IsUserLikeReview(user.Id, id);
        }

        var statusReview = await _statusReviewService.Get("Deleted");
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

        ViewData["review"] = review;
        ViewData["tags"] = await _reviewTagService.GetTagsNames(review.Id);
        ViewData["IsUserLike"] = false;

        return View();
    }

    [Authorize]
    public async Task<IActionResult> Delete([FromRoute] int id)
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

        if (review.AuthorId != user.Id && user.Role.Name != "Admin")
        {
            return BadRequest("You are not the author of this review");
        }

        var statusReview = await _statusReviewService.Get("Deleted");
        if (statusReview == null)
        {
            return BadRequest("StatusReview Deleted not found");
        }

        if (review.StatusId == statusReview.Id)
        {
            return BadRequest("Review Deleted");
        }

        review.StatusId = statusReview.Id;
        var updatedReview = await _reviewService.Update(review);
        
        return View();
    }
}