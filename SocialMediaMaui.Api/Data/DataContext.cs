using Microsoft.EntityFrameworkCore;
using SocialMediaMaui.Api.Data.Entities;

namespace SocialMediaMaui.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<BookMark> BookMarks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookMark>(e =>
            {
                e.HasKey(b => new { b.PostId, b.UserId });
                e.HasOne(b => b.User)
                 .WithMany()
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Like>(e =>
            {
                e.HasKey(b => new { b.PostId, b.UserId });
                e.HasOne(b => b.User)
                 .WithMany()
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
               .HasOne(c => c.User)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
