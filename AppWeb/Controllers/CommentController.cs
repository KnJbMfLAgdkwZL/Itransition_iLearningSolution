using Business.Dto.Frontend.FromForm;
using Business.Interfaces;
using Business.Interfaces.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWeb.Controllers;

public class CommentController : Controller
{
    private readonly ICommentService _commentService;
    private readonly IAccountService _accountService;

    public CommentController(
        ICommentService commentService,
        IAccountService accountService
    )
    {
        _commentService = commentService;
        _accountService = accountService;
    }

    public async Task<IActionResult> GetComments(int reviewId, CancellationToken token)
    {
        var comments = await _commentService.GetComments(reviewId, token);

        var settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = (sender, args) => { args.ErrorContext.Handled = true; },
        };
        var jsonResponse = JsonConvert.SerializeObject(comments, Formatting.Indented, settings);

        return Ok(jsonResponse);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] Message message, CancellationToken token)
    {
        var user = _accountService.GetAuthorizedUser(HttpContext, out var error, token);
        if (user == null)
        {
            return error!;
        }

        if (message.Content.Length > 0 && message.Content.Length < 3000)
        {
            await _commentService.CreateComment(message.ReviewId, user.Id, message.Content, token);
            return Ok();
        }

        return BadRequest();
    }
}