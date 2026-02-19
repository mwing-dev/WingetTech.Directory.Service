using Microsoft.EntityFrameworkCore;
using WingetTech.Directory.Service.Core.Entities;

namespace WingetTech.Directory.Service.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core database context for the directory service.
/// </summary>
public class DirectoryDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public DirectoryDbContext(DbContextOptions<DirectoryDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets the database settings dataset.
    /// </summary>
    public DbSet<DatabaseSettings> DatabaseSettings => Set<DatabaseSettings>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DatabaseSettings>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Host).IsRequired();
            entity.Property(e => e.DatabaseName).IsRequired();
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.Password).IsRequired();
        });
    }
}
