using Business.Interfaces;
using Business.Interfaces.Model;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class UserController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly IStatusReviewService _statusReviewService;
    private readonly IUserClaimsService _userClaimsService;
    private readonly IUserSocialService _userSocialService;
    private readonly IUserService _userService;
    private readonly IProductGroupService _productGroupService;


    public UserController(IUserClaimsService userClaimsService, IUserSocialService userSocialService,
        IUserService userService, IStatusReviewService statusReviewService, IReviewService reviewService,
        IProductGroupService productGroupService)
    {
        _userClaimsService = userClaimsService;
        _userSocialService = userSocialService;
        _userService = userService;
        _statusReviewService = statusReviewService;
        _reviewService = reviewService;
        _productGroupService = productGroupService;
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
    public async Task<IActionResult> PersonalPage()
    {
        var user = GetAuthorizedUser(out var error);
        if (user == null)
        {
            return error!;
        }

        var statusReview = await _statusReviewService.Get("Deleted");
        if (statusReview == null)
        {
            return BadRequest("StatusReview Deleted not found");
        }

        var reviews = await _reviewService.GetAllIncludes(user.Id);
        ViewData["reviews"] = reviews.Where(review => review.StatusId != statusReview.Id).ToList();

        var productGroups = await _productGroupService.GetAll();
        ViewData["productGroups"] = productGroups.ToDictionary(productGroup => productGroup.Id);

        return View();
    }
}