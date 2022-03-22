namespace Business.Interfaces;

public interface IUploadService
{
    Task<string> UploadToDropboxAsync(Stream stream, string fileName, CancellationToken token);
    Task<string> UploadToMegaAsync(Stream stream, string fileName, CancellationToken token);
    Task<string> UploadToAzureStorageAsync(Stream stream, string fileName, CancellationToken token);
}