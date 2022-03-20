using System;
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

    public async Task<ApplicationUserEntity> InsertValidatedAgencyEmployeeEntity()
    {
        ApplicationUserEntity agencyEmployee = new ApplicationUserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "shepard.n7",
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
        ApplicationUserEntity agencyEmployee = new ApplicationUserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "shepard.n7",
            Password = BCrypt.Net.BCrypt.HashPassword(UserPassword),
            FirstName = "John",
            LastName = "Shepard",
            Role = ApplicationUserRole.LifeAssistant,
            Validated = true
        };
        await this.context.Users.AddAsync(agencyEmployee);
        await this.context.SaveChangesAsync();

        return agencyEmployee;
    }
}