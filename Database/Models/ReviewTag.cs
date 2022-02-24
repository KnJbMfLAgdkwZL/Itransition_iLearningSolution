using System;
using System.Collections.Generic;

namespace Database.Models
{
    public partial class ReviewTag
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int TagId { get; set; }

        public virtual Review Review { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
