using System;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.AppointmentState;
using LifeAssistant.Web.Database.Entities;
using LifeAssistant.Web.Database.Repositories;
using Xunit;

namespace LifeAssistant.Web.Tests.Database;

public class AppointmentRepositoryTests : DatabaseTest
{
    [Fact]
    public async Task GetAppointments_ReturnsAllAppointmentsInDatabase()
    {
        // Given
        var appointment1 = new AppointmentEntity(){ DateTime = DateTime.Now.AddDays(1), Id = Guid.NewGuid(), State = "Planned"};
        var appointment2 = new AppointmentEntity(){ DateTime = DateTime.Now.AddDays(2), Id = Guid.NewGuid(), State = "Planned"};
        var appointment3 = new AppointmentEntity(){ DateTime = DateTime.Now.AddDays(3), Id = Guid.NewGuid(), State = "Planned"};
        await this.context.Appointments.AddRangeAsync(appointment1, appointment2, appointment3);
        await this.context.SaveChangesAsync();

        var repository = new AppointmentRepository(this.context, new AppointmentStateFactory());
        
        // When
        var result = await repository.GetAppointments();

        // Then
        result.Count.Should().Be(3);
        result[0].Id.Should().Be(appointment1.Id);
        result[0].State.Name.Should().Be(appointment1.State);
        result[0].DateTime.Should().Be(appointment1.DateTime);
        result[1].Id.Should().Be(appointment2.Id);
        result[1].State.Name.Should().Be(appointment2.State);
        result[1].DateTime.Should().Be(appointment2.DateTime);
        result[2].Id.Should().Be(appointment3.Id);
        result[2].State.Name.Should().Be(appointment3.State);
        result[2].DateTime.Should().Be(appointment3.DateTime);
    }
}