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

    public IActionResult Get([FromRoute] int id)
    {
        ViewData["tagId"] = id;
        return View();
    }

    public async Task<IActionResult> GetTopTags(CancellationToken token)
    {
        var tagsModels = await _tagService.GetTopTagsAsync(token);

        var tags = new List<object>();
        foreach (var tag in tagsModels)
        {
            tags.Add(new {tag.Id, tag.Amount, tag.Name});
        }

        var settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = (sender, args) => { args.ErrorContext.Handled = true; },
        };

        var json = JsonConvert.SerializeObject(tags, Formatting.Indented, settings);

        return Ok(json);
    }
}