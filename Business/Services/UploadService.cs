using Azure.Storage.Blobs;
using Business.Dto.Options;
using Business.Interfaces;
using CG.Web.MegaApiClient;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.Extensions.Options;

namespace Business.Services;

public class UploadService : IUploadService
{
    private readonly UploadOptions _uploadOptions;

    public UploadService(
        IOptions<UploadOptions> uploadOptions
    )
    {
        _uploadOptions = uploadOptions.Value;
    }

    public async Task<string> UploadToDropboxAsync(Stream stream, string fileName, CancellationToken token)
    {
        using var dropboxClient = new DropboxClient(_uploadOptions.DropBoxAccessToken);

        var uploadFile = await dropboxClient.Files.UploadAsync(
            "/" + fileName,
            WriteMode.Overwrite.Instance,
            body: stream);

        var sharedLink = await dropboxClient.Sharing.CreateSharedLinkWithSettingsAsync(uploadFile.PathLower);

        return sharedLink.Url.Replace("?dl=0", "?raw=1");
    }

    public async Task<string> UploadToMegaAsync(Stream stream, string fileName, CancellationToken token)
    {
        var megaApiClient = new MegaApiClient();

        megaApiClient.Login(_uploadOptions.MegaEmail, _uploadOptions.MegaPassword);

        const string folderName = "img";

        IEnumerable<INode> nodes = await megaApiClient.GetNodesAsync();

        var folders = nodes.Where(n => n.Type == NodeType.Directory).ToList();

        var folder = folders.FirstOrDefault(f => f.Name == folderName);

        var uploadFile = await megaApiClient.UploadAsync(stream, fileName, folder);

        var downloadLink = await megaApiClient.GetDownloadLinkAsync(uploadFile);

        await megaApiClient.LogoutAsync();

        return downloadLink.AbsoluteUri;
    }

    public async Task<string> UploadToAzureStorageAsync(Stream stream, string fileName, CancellationToken token)
    {
        var blobClient = new BlobClient(
            connectionString: _uploadOptions.AzureStorageConnectionString,
            blobContainerName: "img",
            blobName: fileName);

        await blobClient.UploadAsync(stream, token);

        return blobClient.Uri.AbsoluteUri;
    }
}