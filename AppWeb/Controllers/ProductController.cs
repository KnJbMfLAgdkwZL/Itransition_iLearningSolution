using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class ProductController : Controller
{
    public IActionResult GetAsync([FromRoute] int id, CancellationToken token)
    {
        return Ok();
    }
}