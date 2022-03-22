using Business.Interfaces.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWeb.Controllers;

public class TagController : Controller
{
    private readonly ITagService _tagService;

    public TagController(
        ITagService tagService
    )
    {
        _tagService = tagService;
    }

    public async Task<IActionResult> GetAllAsync(string search, CancellationToken token)
    {
        var tags = await _tagService.GetTopTagsAsync(search, token);

        var settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = (sender, args) => { args.ErrorContext.Handled = true; },
        };

        var jsonResponse = JsonConvert.SerializeObject(tags, Formatting.Indented, settings);

        return Ok(jsonResponse);
    }
}