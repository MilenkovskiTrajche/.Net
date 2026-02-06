using Lab1IS.Models;
using Lab1IS.Models.Identity;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Lab1IS.Data
{
    public class ApplicationDbContext : IdentityDbContext<MuseumsApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
            public DbSet<Artifact> Artifacts { get; set; }
            public DbSet<Collection> Collections { get; set; }
            public DbSet<Visit> Visits { get; set; }
            public DbSet<Visitor> Visitors { get; set; }

            protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Artifact>()
                .HasOne(a => a.Collection)
                .WithMany(c => c.Artifacts)
                .HasForeignKey(a => a.CollectionId);

            builder.Entity<Visit>()
                .HasOne(v => v.Visitor)
                .WithMany(vis => vis.Visits)
                .HasForeignKey(v => v.VisitorId);

            builder.Entity<Visit>()
                .HasOne(v => v.Artifact)
                .WithMany()
                .HasForeignKey(v => v.ArtifactId);

        }
    }
    
}
