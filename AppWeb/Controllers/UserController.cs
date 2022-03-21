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
    public async Task<IActionResult> GetUserReviews([FromRoute] int id)
    {
        var user = _accountService.GetAuthorizedUser(HttpContext, out var error);
        if (user == null)
        {
            return error!;
        }

        if (id > 0 && user.Role.Name == "Admin")
        {
            user = await _userService.GetUserById(id);
            if (user == null)
            {
                return NotFound("User NotFound");
            }
        }

        var reviews = await _reviewService.GetAllIncludes(user.Id);
        if (user.Role.Name != "Admin")
        {
            var statusReview = await _statusReviewService.Get("Deleted");
            if (statusReview == null)
            {
                return BadRequest("StatusReview Deleted not found");
            }

            reviews = reviews.Where(review => review.StatusId != statusReview.Id).ToList();
        }

        ViewData["reviews"] = reviews;

        var productGroups = await _productGroupService.GetAll();
        ViewData["productGroups"] = productGroups.ToDictionary(productGroup => productGroup.Id);
        ViewData["userId"] = id;

        return View();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetAllInclude();
        ViewData["users"] = users;
        return View();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUser([FromRoute] int id)
    {
        var user = await _userService.GetIncludesForAdmin(id);
        if (user == null)
        {
            return NotFound("User NotFound");
        }

        ViewData["user"] = user;
        ViewData["roles"] = await _roleService.GetRoleAll();
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> UpdateUser([FromForm] int id, [FromForm] string nickname, [FromForm] int roleId)
    {
        var user = await _userService.GetUserById(id);
        if (user == null)
        {
            return NotFound("User NotFound");
        }

        user.Nickname = nickname;
        user.RoleId = roleId;
        await _userService.Update(user);
        return RedirectToAction("GetUser", "User", new {id = user.Id});
    }
}