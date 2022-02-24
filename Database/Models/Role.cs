using Database.Interfaces;

namespace Database.Models;

public partial class Role : IEntity
{
    public Role()
    {
        User = new HashSet<User>();
    }

    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public virtual ICollection<User> User { get; set; }
}