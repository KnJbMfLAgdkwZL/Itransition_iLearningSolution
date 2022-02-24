using Database.Interfaces;

namespace Database.Models;

public partial class StatusReview : IEntity
{
    public StatusReview()
    {
        Review = new HashSet<Review>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<Review> Review { get; set; }
}