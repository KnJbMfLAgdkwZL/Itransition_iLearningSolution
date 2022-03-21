using System.ComponentModel.DataAnnotations;

namespace Business.Dto.Frontend.FromForm;

public class ReviewForm
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Required StatusReviewId")]
    public int StatusReviewId { get; set; }

    [Required(ErrorMessage = "Required ProductId")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Required ProductName")]
    [StringLength(50)]
    public string ProductName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required Title")]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required Content")]
    [StringLength(30000)]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required AuthorAssessment")]
    [Range(1, 5)]
    public byte AuthorAssessment { get; set; }

    public string TagsInput { get; set; } = string.Empty;
    public int AuthorId { get; set; } = 0;
    public int UserId { get; set; } = 0;
    public int ReviewId { get; set; } = 0;
}