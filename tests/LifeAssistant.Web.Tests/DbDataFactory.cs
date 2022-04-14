using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Web.Database;
using LifeAssistant.Web.Database.Entities;

namespace LifeAssistant.Web.Tests;

public class DbDataFactory
{
    private readonly ApplicationDbContext context;
    public string UserPassword => "ized5è54é-@!é";

    public DbDataFactory(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<ApplicationUserEntity> InsertValidatedAgencyEmployee()
    {
        ApplicationUserEntity agencyEmployee = new ApplicationUserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "shepard.n7.employee",
            Password = BCrypt.Net.BCrypt.HashPassword(UserPassword),
            FirstName = "John",
            LastName = "Shepard",
            Role = ApplicationUserRole.AgencyEmployee,
            Validated = true
        };
        await this.context.Users.AddAsync(agencyEmployee);
        await this.context.SaveChangesAsync();

        return agencyEmployee;
    }

    public async Task<ApplicationUserEntity> InsertValidatedLifeAssistant()
    {
        return await InsertLifeAssistant(true);
    }

    public async Task<ApplicationUserEntity> InsertLifeAssistant(bool validated)
    {
        var agencyEmployee = CreateApplicationUserEntity();
        agencyEmployee.Validated = validated;
        await this.context.Users.AddAsync(agencyEmployee);
        await this.context.SaveChangesAsync();

        return agencyEmployee;
    }

    private ApplicationUserEntity CreateApplicationUserEntity()
    {
        ApplicationUserEntity lifeAssistant = new ApplicationUserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "shepard.n7.assistant",
            Password = BCrypt.Net.BCrypt.HashPassword(UserPassword),
            FirstName = "John",
            LastName = "Shepard",
            Role = ApplicationUserRole.LifeAssistant,
            Validated = true
        };
        return lifeAssistant;
    }

    public async Task<ApplicationUserEntity> InsertValidatedLifeAssistantWithAppointments()
    {
        return await InsertLifeAssistantWithAppointments(true);
    }

    public async Task<ApplicationUserEntity> InsertLifeAssistantWithAppointments(bool validated)
    {
        var lifeAssistant = CreateApplicationUserEntity();
        lifeAssistant.Validated = validated;
        lifeAssistant.Appointments = new List<AppointmentEntity>
        {
            new AppointmentEntity()
            {
                DateTime = DateTime.Now.AddDays(1),
                Id = Guid.NewGuid(),
                State = "Planned",
            },
            new AppointmentEntity()
            {
                DateTime = DateTime.Now.AddDays(2),
                Id = Guid.NewGuid(),
                State = "Pending Pickup"
            }
        };

        await this.context.Users.AddAsync(lifeAssistant);
        await this.context.SaveChangesAsync();

        return lifeAssistant;
    }
}