using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppWeb.Controllers;

[Authorize(Roles = "Admin, User")]
public class ImagesController : Controller
{
    private readonly IUploadService _uploadService;

    public ImagesController(
        IUploadService uploadService
    )
    {
        _uploadService = uploadService;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var fileName = Path.GetRandomFileName().Split(".")[0] + Path.GetExtension(file.FileName);

        await using var stream = file.OpenReadStream();

        return Ok(new
        {
            location = await _uploadService.UploadToAzureStorage(stream, fileName)
        });
    }
}