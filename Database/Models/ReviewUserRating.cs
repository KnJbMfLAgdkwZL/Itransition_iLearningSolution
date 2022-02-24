using System;
using System.Collections.Generic;

namespace Database.Models
{
    public partial class ReviewUserRating
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public byte Assessment { get; set; }

        public virtual Review Review { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
