using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;

namespace nhacks.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //Users <-> UserGroup many-to-many relationship
            builder.Entity<UserGroup>()
                .HasKey(t => new { t.UserId, t.GroupId });
        }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<UserGroup> UserGroup { get; set; }
        public DbSet<Group> Group { get; set; }
    }
}
