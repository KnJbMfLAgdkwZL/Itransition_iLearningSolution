using System;
using System.Collections.Generic;

namespace Database.Models
{
    public partial class User
    {
        public User()
        {
            Comment = new HashSet<Comment>();
            Review = new HashSet<Review>();
            ReviewLike = new HashSet<ReviewLike>();
            ReviewUserRating = new HashSet<ReviewUserRating>();
        }

        public int Id { get; set; }
        public int SocialId { get; set; }
        public int RoleId { get; set; }
        public string Nickname { get; set; } = null!;
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string? Avatar { get; set; }
        public int ReviewsLikes { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual UserSocial Social { get; set; } = null!;
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<Review> Review { get; set; }
        public virtual ICollection<ReviewLike> ReviewLike { get; set; }
        public virtual ICollection<ReviewUserRating> ReviewUserRating { get; set; }
    }
}
