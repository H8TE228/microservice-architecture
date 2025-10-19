using Microsoft.EntityFrameworkCore;
using PostService.Domain.Entities;

namespace PostService.Infrastructure.Data
{
    public class PostDbContext : DbContext
    {
        public PostDbContext(DbContextOptions<PostDbContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Rating> Ratings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.UserId).IsRequired();
                entity.Property(p => p.Description).HasMaxLength(2000);
                
                entity.HasMany(p => p.Ratings)
                    .WithOne(r => r.Post)
                    .HasForeignKey(r => r.PostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.PostId).IsRequired();
                entity.Property(r => r.RaterUserId).IsRequired();
                entity.Property(r => r.RatedUserId).IsRequired();
                entity.Property(r => r.Type).IsRequired();
                
                entity.HasIndex(r => new { r.PostId, r.RaterUserId }).IsUnique();
            });
        }
    }
}