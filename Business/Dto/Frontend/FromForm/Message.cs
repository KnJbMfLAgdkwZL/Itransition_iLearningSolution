using System.ComponentModel.DataAnnotations;

namespace Business.Dto.Frontend.FromForm;

public class Message
{
    [Required] public int ReviewId { get; set; }

    [Required] public string Content { get; set; } = string.Empty;
}