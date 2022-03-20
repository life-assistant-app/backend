using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Application.Appointments;
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
        fakeUserRepository.Data.Add(agencyEmployee);
        fakeUserRepository.Data.Add(lifeAssistant);
        
        var accessControlManager = new AccessControlManager(agencyEmployee.Id, fakeUserRepository);
        var appointmentApplication = new AppointmentsApplication(fakeUserRepository, accessControlManager);
        
        // When
        Appointment appointment = await appointmentApplication.CreateAppointment(lifeAssistant, appointmentDate);

        // Then
        appointment.Id.Should().NotBeEmpty();
        appointment.State.Name.Should().Be("Planned");
        appointment.DateTime.Should().Be(appointmentDate);

        ApplicationUser lifeAssistantFromDb = fakeUserRepository.Data.First();
        lifeAssistantFromDb.Appointments.Count().Shoud().Be(1);
        lifeAssistantFromDb.Appointments[0].Id.Should().NotBeEmpty();
        lifeAssistantFromDb.Appointments[0].State.Name.Should().Be("Planned");
        lifeAssistantFromDb.Appointments[0].DateTime.Should().Be(appointmentDate);
    }
}