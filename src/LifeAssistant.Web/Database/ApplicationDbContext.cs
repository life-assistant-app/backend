using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeAssistant.Web.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<ApplicationUserEntity> Users { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUserEntity>().HasKey(user => user.Id);
        modelBuilder.Entity<ApplicationUserEntity>().HasIndex(user => user.UserName).IsUnique();
        modelBuilder.Entity<ApplicationUserEntity>().Property(user => user.FirstName);
        modelBuilder.Entity<ApplicationUserEntity>().Property(user => user.LastName);
        modelBuilder.Entity<ApplicationUserEntity>().Property(user => user.Password);
        modelBuilder.Entity<ApplicationUserEntity>().Property(user => user.Validated);
        modelBuilder.Entity<ApplicationUserEntity>().Property(user => user.Role);
    }
}