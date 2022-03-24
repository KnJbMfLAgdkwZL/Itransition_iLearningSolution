namespace Business.Dto.Frontend.FromForm;

public class UsersFilterForm
{
    public string OrderBy { get; set; } = "Id";
    public int Id { get; set; } = 0;
    public string Nickname { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Network { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}