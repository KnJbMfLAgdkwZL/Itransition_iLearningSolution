using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWeb.Controllers;

public class Tag : Controller
{
    private readonly ITagService _tagService;

    public Tag(ITagService tagService)
    {
        _tagService = tagService;
    }

    public async Task<IActionResult> GetAll(string search)
    {
        var r = await _tagService.GetTopTags(search);

        var settings = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Error = (sender, args) => { args.ErrorContext.Handled = true; },
        };
        var jsonResponse = JsonConvert.SerializeObject(r, Formatting.Indented, settings);

        return Ok(jsonResponse);
    }
}