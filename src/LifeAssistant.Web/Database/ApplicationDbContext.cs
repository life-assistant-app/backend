using LifeAssistant.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeAssistant.Web.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<ApplicationUser> Users { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>().HasKey(user => user.Id);
        modelBuilder.Entity<ApplicationUser>().HasIndex(user => user.Username).IsUnique();
        modelBuilder.Entity<ApplicationUser>().Property(user => user.Password);
        modelBuilder.Entity<ApplicationUser>().Property(user => user.Validated);
        modelBuilder.Entity<ApplicationUser>().Property(user => user.Role);
    }
}