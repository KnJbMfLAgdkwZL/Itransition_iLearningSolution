using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

public class ProductController : Controller
{
    public IActionResult GetAsync([FromRoute] int id)
    {
        ViewData["id"] = id;
        return View();
    }
}