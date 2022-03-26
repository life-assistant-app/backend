using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Application.Appointments;
using LifeAssistant.Core.Application.Appointments.Contracts;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Rules;
using LifeAssistant.Core.Tests.Application.FakePersistence;
using LifeAssistant.Web.Tests;
using Xunit;

namespace LifeAssistant.Core.Tests.Application;

public class AppointmentsApplicationTests
{
    private readonly DataFactory dataFactory = new DataFactory();

    [Fact]
    public async Task CreateAppointment_ByAgencyEmployeeWithLifeAssistant_InsertsPlannedAppointForLifeAssistant()
    {
        // Given
        ApplicationUser agencyEmployee = dataFactory.CreateAgencyEmployee();
        ApplicationUser lifeAssistant = dataFactory.CreateLifeAssistant();

        DateTime appointmentDate = DateTime.Today
            .Add(TimeSpan.FromDays(1))
            .Add(TimeSpan.FromHours(16));

        var fakeUserRepository = new FakeApplicationUserRepository();
        await fakeUserRepository.Insert(agencyEmployee);
        await fakeUserRepository.Save();
        await fakeUserRepository.Insert(lifeAssistant);
        await fakeUserRepository.Save();

        var accessControlManager = new AccessControlManager(agencyEmployee.Id, fakeUserRepository);
        var appointmentApplication = new AppointmentsApplication(fakeUserRepository, accessControlManager);

        // When
        GetAppointmentResponse appointment = await appointmentApplication.CreateAppointment(lifeAssistant.Id, appointmentDate);

        // Then
        appointment.Id.Should().NotBeEmpty();
        appointment.State.Should().Be("Planned");
        appointment.DateTime.Should().Be(appointmentDate);

        IApplicationUserWithAppointments lifeAssistantFromDb =
            await fakeUserRepository.FindByIdWithAppointments(lifeAssistant.Id);
        lifeAssistantFromDb.Appointments.Count().Should().Be(1);
        lifeAssistantFromDb.Appointments[0].Id.Should().NotBeEmpty();
        lifeAssistantFromDb.Appointments[0].State.Name.Should().Be("Planned");
        lifeAssistantFromDb.Appointments[0].DateTime.Should().Be(appointmentDate);
    }
}