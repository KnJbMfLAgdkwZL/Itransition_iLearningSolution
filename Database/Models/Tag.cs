using Database.Interfaces;

namespace Database.Models;

public partial class Tag : IEntity
{
    public Tag()
    {
        ReviewTag = new HashSet<ReviewTag>();
    }

    public int Id { get; set; }
    public int Amount { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<ReviewTag> ReviewTag { get; set; }
}