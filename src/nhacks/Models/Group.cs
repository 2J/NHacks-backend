using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace nhacks.Models
{
    public class Group
    {
        [ScaffoldColumn(false)]
        public int id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Password { get; set; } //this is in plaintext (lol)

        // connected users
        public virtual ICollection<UserGroup> UserGroups { get; set; }
    }
}
