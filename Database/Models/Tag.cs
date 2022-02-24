using System;
using System.Collections.Generic;

namespace Database.Models
{
    public partial class Tag
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
}
