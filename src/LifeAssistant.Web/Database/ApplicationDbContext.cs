﻿using LifeAssistant.Web.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeAssistant.Web.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<ApplicationUserEntity> Users { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
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
        modelBuilder.Entity<ApplicationUserEntity>().HasMany<AppointmentEntity>();

        modelBuilder.Entity<AppointmentEntity>().HasKey(appointment => appointment.Id);
        modelBuilder.Entity<AppointmentEntity>().Property(appointment => appointment.State);
        modelBuilder.Entity<AppointmentEntity>().Property(appointment => appointment.DateTime);
    }
}