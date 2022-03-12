using Business.Interfaces.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWeb.Controllers;

public class TagController : Controller
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    public async Task<IActionResult> GetAll(string search)
    {
        var tags = await _tagService.GetTopTags(search);

        var settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = (sender, args) => { args.ErrorContext.Handled = true; },
        };
        var jsonResponse = JsonConvert.SerializeObject(tags, Formatting.Indented, settings);

        return Ok(jsonResponse);
    }
}