namespace Business.Interfaces;

public interface IUploadService
{
    Task<string> UploadToDropbox(Stream stream, string fileName);
    Task<string> UploadToMega(Stream stream, string fileName);
    Task<string> UploadToAzureStorage(Stream stream, string fileName);
}