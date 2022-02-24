using System;
using System.Collections.Generic;

namespace Database.Models
{
    public partial class UserSocial
    {
        public UserSocial()
        {
            User = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Uid { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Network { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
