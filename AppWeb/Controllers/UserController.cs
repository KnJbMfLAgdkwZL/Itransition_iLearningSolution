using Business.Interfaces;
using Business.Interfaces.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class UserController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly IStatusReviewService _statusReviewService;
    private readonly IUserService _userService;
    private readonly IProductGroupService _productGroupService;
    private readonly IRoleService _roleService;
    private readonly IAccountService _accountService;

    public UserController(
        IUserService userService,
        IStatusReviewService statusReviewService,
        IReviewService reviewService,
        IProductGroupService productGroupService,
        IRoleService roleService,
        IAccountService accountService
    )
    {
        _userService = userService;
        _statusReviewService = statusReviewService;
        _reviewService = reviewService;
        _productGroupService = productGroupService;
        _roleService = roleService;
        _accountService = accountService;
    }

    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> GetUserReviewsAsync([FromRoute] int id, CancellationToken token)
    {
        var user = _accountService.GetAuthorizedUser(HttpContext, out var error, token);
        if (user == null)
        {
            return error!;
        }

        if (id > 0 && user.Role.Name == "Admin")
        {
            user = await _userService.GetUserByIdAsync(id, token);
            if (user == null)
            {
                return NotFound("User NotFound");
            }
        }

        var reviews = await _reviewService.GetAllIncludesAsync(user.Id, token);
        if (user.Role.Name != "Admin")
        {
            var statusReview = await _statusReviewService.GetAsync("Deleted", token);
            if (statusReview == null)
            {
                return BadRequest("StatusReview Deleted not found");
            }

            reviews = reviews.Where(review => review.StatusId != statusReview.Id).ToList();
        }

        ViewData["reviews"] = reviews;

        var productGroups = await _productGroupService.GetAllAsync(token);
        ViewData["productGroups"] = productGroups.ToDictionary(productGroup => productGroup.Id);
        ViewData["userId"] = id;

        return View();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsersAsync(CancellationToken token)
    {
        ViewData["users"] = await _userService.GetAllIncludeAsync(token);

        return View();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserAsync([FromRoute] int id, CancellationToken token)
    {
        var user = await _userService.GetIncludesForAdminAsync(id, token);
        if (user == null)
        {
            return NotFound("User NotFound");
        }

        ViewData["user"] = user;
        ViewData["roles"] = await _roleService.GetRoleAllAsync(token);

        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> UpdateUserAsync([FromForm] int id, [FromForm] string nickname,
        [FromForm] int roleId,
        CancellationToken token)
    {
        var user = await _userService.GetUserByIdAsync(id, token);
        if (user == null)
        {
            return NotFound("User NotFound");
        }

        user.Nickname = nickname;
        user.RoleId = roleId;

        await _userService.UpdateAsync(user, token);

        return RedirectToAction("GetUser", "User", new {id = user.Id});
    }
}