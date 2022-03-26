using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.AppointmentState;
using LifeAssistant.Web.Database.Entities;
using LifeAssistant.Web.Database.Respositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LifeAssistant.Web.Tests.Database;

public class ApplicationUserRepositoryTest : DatabaseTest
{
    private AppointmentStateFactory factory = new AppointmentStateFactory();

    [Fact]
    public async Task FindByUsername_WithExistingUserName_ReturnsUserRecord()
    {
        // Given
        ApplicationUserEntity entity = await this.dbDataFactory.InsertValidatedAgencyEmployeeEntity();
        var repository = new ApplicationUserRepository(this.context, factory);

        // When
        IApplicationUser result = await repository.FindByUsername(entity.UserName);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.UserName.Should().Be(entity.UserName);
        result.Password.Should().Be(entity.Password);
        result.Role.Should().Be(entity.Role);
        result.Validated.Should().Be(entity.Validated);
    }

    [Fact]
    public async Task FindById_WithExistingId_ReturnsUserRecord()
    {
        // Given
        ApplicationUserEntity entity = await this.dbDataFactory
            .InsertValidatedAgencyEmployeeEntity();
        var repository = new ApplicationUserRepository(this.context, factory);

        // When
        IApplicationUser result = await repository.FindById(entity.Id);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.UserName.Should().Be(entity.UserName);
        result.Password.Should().Be(entity.Password);
        result.Role.Should().Be(entity.Role);
        result.Validated.Should().Be(entity.Validated);
    }

    [Fact]
    public async Task Update_UpdatesRecordInDb()
    {
        // Given
        ApplicationUserEntity entity = await this.dbDataFactory.InsertValidatedAgencyEmployeeEntity();
        var repository = new ApplicationUserRepository(this.context, factory);
        entity.FirstName = "Updated";
        entity.Appointments.Add(new AppointmentEntity()
        {
            State = "Planned", DateTime = DateTime.Now.AddDays(1)
        });

        // When
        await repository.Update(entity.ToDomainEntity(new AppointmentStateFactory()));
        await repository.Save();

        // Then
        entity = this.context
            .Users
            .Include(u => u.Appointments)
            .First(item => item.Id == entity.Id);
        entity.FirstName.Should().Be("Updated");
        entity.Appointments.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Insert_InsertsRecordInDb()
    {
        // Given
        ApplicationUser user = this.dataFactory.CreateAgencyEmployee();
        user.Appointments.Add(new Appointment(DateTime.Now.AddDays(1)));

        var repository = new ApplicationUserRepository(this.context, factory);

        // When
        await repository.Insert(user);
        await repository.Save();

        // Then
        (await context.Users.CountAsync()).Should().Be(1);
        ApplicationUserEntity userInDb = await context
            .Users
            .Include(u => u.Appointments)
            .FirstAsync();

        userInDb.Id.Should().Be(user.Id);
        userInDb.UserName.Should().Be(user.UserName);
        userInDb.Password.Should().Be(user.Password);
        userInDb.FirstName.Should().Be(user.FirstName);
        userInDb.LastName.Should().Be(user.LastName);
        userInDb.Role.Should().Be(user.Role);
        userInDb.Validated.Should().Be(user.Validated);
        userInDb.Appointments.Should().HaveCount(1);
    }

    [Fact]
    public async Task FindByIdWithAppointments_RetrievesRecordWithChildRecords()
    {
        // Given
        ApplicationUser user = this.dataFactory.CreateAgencyEmployee();
        user.Appointments.Add(new Appointment(DateTime.Now.AddDays(1)));
        await this.context.Users.AddAsync((new ApplicationUserEntity(user)));
        await this.context.SaveChangesAsync();

        var repository = new ApplicationUserRepository(this.context, factory);

        // When
        IApplicationUserWithAppointments result = await repository.FindByIdWithAppointments(user.Id);

        // Then
        result.Id.Should().Be(user.Id);
        result.UserName.Should().Be(user.UserName);
        result.Password.Should().Be(user.Password);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
        result.Role.Should().Be(user.Role);
        result.Validated.Should().Be(user.Validated);
        result.Appointments.Should().HaveCount(1);
    }
}