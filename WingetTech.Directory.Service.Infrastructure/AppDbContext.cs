using Microsoft.EntityFrameworkCore;
using WingetTech.Directory.Service.Core.Entities;

namespace WingetTech.Directory.Service.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DirectorySettings> DirectorySettings => Set<DirectorySettings>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DirectorySettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Host).IsRequired();
                entity.Property(e => e.Domain).IsRequired();
                entity.Property(e => e.BaseDn).IsRequired();
                entity.Property(e => e.BindUsername).IsRequired();
                entity.Property(e => e.BindPassword).IsRequired();
            });
        }
    }
}
