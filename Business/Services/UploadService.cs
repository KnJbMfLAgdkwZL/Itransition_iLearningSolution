using Business.Dto.Options;
using Business.Interfaces;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.Extensions.Options;

namespace Business.Services;

public class UploadService : IUploadService
{
    private readonly UploadOptions _uploadOptions;

    public UploadService(IOptions<UploadOptions> uploadOptions)
    {
        _uploadOptions = uploadOptions.Value;
    }

    public async Task<string> UploadToDropbox(Stream stream, string fileName)
    {
        using var dropBox = new DropboxClient(_uploadOptions.DropBoxAccessToken);
        var uploadFile = await dropBox.Files.UploadAsync(
            "/" + fileName,
            WriteMode.Overwrite.Instance,
            body: stream);
        var sharedLink = await dropBox.Sharing.CreateSharedLinkWithSettingsAsync(uploadFile.PathLower);
        return sharedLink.Url.Replace("?dl=0", "?raw=1");
    }
}