using System.ComponentModel.DataAnnotations;

namespace Business.Dto.Frontend.Form;

public class Review
{
    [Required(ErrorMessage = "Required ProductType")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Required ProductName")]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required Content")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required AuthorAssessment")]
    public byte AuthorAssessment { get; set; }
}