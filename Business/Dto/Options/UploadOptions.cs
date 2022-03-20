namespace Business.Dto.Options;

public class UploadOptions
{
    public string DropBoxAccessToken { get; set; } = string.Empty;
    public string MegaEmail { get; set; } = string.Empty;
    public string MegaPassword { get; set; } = string.Empty;
    public string AzureStorageConnectionString { get; set; } = string.Empty;
}