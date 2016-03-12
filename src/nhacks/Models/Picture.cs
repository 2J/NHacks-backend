using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace nhacks.Models
{
    public class Picture
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        public string UserId { get; set; } //TODO: Foreign key
        //[ForeignKey("UserId")]
        //public virtual ApplicationUser User { get; set; }

        public string ScannedUserId { get; set; } 
        [ForeignKey("ScannedUserId")]
        public virtual ApplicationUser ScannedUser { get; set; }

        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        public DateTime ScannedAt { get; set; }
    }
}
