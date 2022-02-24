using System;
using System.Collections.Generic;

namespace Database.Models
{
    public partial class ProductGroup
    {
        public ProductGroup()
        {
            Review = new HashSet<Review>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Review> Review { get; set; }
    }
}
