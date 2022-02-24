using Database.Interfaces;

namespace Database.Models;

public partial class Review : IEntity
{
    public Review()
    {
        Comment = new HashSet<Comment>();
        ReviewLike = new HashSet<ReviewLike>();
        ReviewTag = new HashSet<ReviewTag>();
        ReviewUserRating = new HashSet<ReviewUserRating>();
    }

    public int Id { get; set; }
    public int ProductId { get; set; }
    public int AuthorId { get; set; }
    public int StatusId { get; set; }
    public string ProductName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime CreationDate { get; set; }
    public DateTime RedactionDate { get; set; }
    public DateTime DeletionDate { get; set; }
    public byte AuthorAssessment { get; set; }
    public float AverageUserRating { get; set; }

    public virtual User Author { get; set; } = null!;
    public virtual ProductGroup Product { get; set; } = null!;
    public virtual StatusReview Status { get; set; } = null!;
    public virtual ICollection<Comment> Comment { get; set; }
    public virtual ICollection<ReviewLike> ReviewLike { get; set; }
    public virtual ICollection<ReviewTag> ReviewTag { get; set; }
    public virtual ICollection<ReviewUserRating> ReviewUserRating { get; set; }
}