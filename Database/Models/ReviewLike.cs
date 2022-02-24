using Database.Interfaces;

namespace Database.Models;

public partial class ReviewLike : IEntity
{
    public int Id { get; set; }
    public int ReviewId { get; set; }
    public int UserId { get; set; }

    public virtual Review Review { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}