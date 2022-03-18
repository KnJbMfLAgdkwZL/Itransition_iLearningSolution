namespace Business.Dto;

public class UserClaims
{
    public string Uid { set; get; } = string.Empty;
    public string Email { set; get; } = string.Empty;
    public string Network { set; get; } = string.Empty;
    public string Role { set; get; } = string.Empty;
    public bool IsEmpty { set; get; } = false;
}