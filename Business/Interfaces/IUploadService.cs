namespace Business.Interfaces;

public interface IUploadService
{
    Task<string> UploadToDropbox(Stream stream, string fileName);
}