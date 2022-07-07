using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeAssistant.Web.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<ApplicationUserEntity> Users { get; set; }
    public DbSet<AppointmentEntity> Appointments { get; set; }

    private readonly IConfiguration configuration;

    public ApplicationDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
    {
        this.configuration = configuration;
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUserEntity>().HasKey(user => user.Id);
        modelBuilder.Entity<ApplicationUserEntity>().Property(user => user.Id).ValueGeneratedNever();
        modelBuilder.Entity<ApplicationUserEntity>().HasIndex(user => user.UserName).IsUnique();
        modelBuilder.Entity<ApplicationUserEntity>().Property(user => user.FirstName);
        modelBuilder.Entity<ApplicationUserEntity>().Property(user => user.LastName);
        modelBuilder.Entity<ApplicationUserEntity>().Property(user => user.Password);
        modelBuilder.Entity<ApplicationUserEntity>().Property(user => user.Validated);
        modelBuilder.Entity<ApplicationUserEntity>().Property(user => user.Role);
        modelBuilder.Entity<ApplicationUserEntity>().HasMany<AppointmentEntity>()
            .WithOne()
            .HasForeignKey(appointmentEntity => appointmentEntity.LifeAssistantId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ApplicationUserEntity>().HasData(new ApplicationUserEntity()
        {
            Id = Guid.NewGuid(),
            FirstName = "Agency",
            LastName = "Admin",
            Validated = true,
            Role = ApplicationUserRole.AgencyEmployee,
            UserName = "AgencyAdmin",
            Password = BCrypt.Net.BCrypt.HashPassword(configuration["ADMIN_PASSWORD"]),
        });

        modelBuilder.Entity<AppointmentEntity>().HasKey(appointment => appointment.Id);
        modelBuilder.Entity<AppointmentEntity>().Property(appointment => appointment.Id).ValueGeneratedNever();
        modelBuilder.Entity<AppointmentEntity>().Property(appointment => appointment.State);
        modelBuilder.Entity<AppointmentEntity>().Property(appointment => appointment.DateTime);
        modelBuilder.Entity<AppointmentEntity>().Property(appointment => appointment.CreatedDate);
    }
}