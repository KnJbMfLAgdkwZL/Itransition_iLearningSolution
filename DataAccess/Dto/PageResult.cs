namespace DataAccess.Dto;

public class PageResult<T>
{
    public List<T>? Result { get; set; }
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 0;
    public int TotalPages { get; set; } = 0;
    public int TotalRecords { get; set; } = 0;
}