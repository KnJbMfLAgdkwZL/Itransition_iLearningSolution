using Business.Interfaces;
using Business.Interfaces.Model;
using Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUserSocialService _userSocialService;
    private readonly IUserClaimsService _userClaimsService;
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IReviewService _reviewService;
    private readonly IProductGroupService _productGroupService;

    public AdminController(IUserSocialService userSocialService, IUserClaimsService userClaimsService,
        IUserService userService, IRoleService roleService, IReviewService reviewService,
        IProductGroupService productGroupService)
    {
        _userSocialService = userSocialService;
        _userClaimsService = userClaimsService;
        _userService = userService;
        _roleService = roleService;
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

    public async Task<IActionResult> Users(int page = 1, int pageSize = 10)
    {
        var users = await _userService.GetAllInclude(user => user.Id > 0, page, pageSize);

        Console.WriteLine(users.Count);

        ViewData["users"] = users;

        return View();
    }

    public async Task<IActionResult> User([FromRoute] int id)
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

    [HttpPost]
    public async Task<IActionResult> User([FromForm] int id, [FromForm] string nickname, [FromForm] int roleId)
    {
        var user = await _userService.GetUserById(id);
        if (user == null)
        {
            return NotFound("User NotFound");
        }

        user.Nickname = nickname;
        user.RoleId = roleId;
        await _userService.Update(user);
        return RedirectToAction("User", "Admin");
    }

    public async Task<IActionResult> GetAllUserReviews([FromRoute] int id)
    {
        var user = await _userService.GetUserById(id);
        if (user == null)
        {
            return NotFound("User NotFound");
        }

        var reviews = await _reviewService.GetAllIncludes(user.Id);
        ViewData["reviews"] = reviews;

        var productGroups = await _productGroupService.GetAll();
        ViewData["productGroups"] = productGroups.ToDictionary(productGroup => productGroup.Id);

        return View();
    }
}