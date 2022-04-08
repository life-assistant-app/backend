using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Application.Appointments;
using LifeAssistant.Core.Application.Appointments.Contracts;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.ApplicationUser;
using LifeAssistant.Core.Domain.Entities.AppointmentState;
using LifeAssistant.Core.Domain.Exceptions;
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
        await fakeUserRepository.Insert(lifeAssistant);
        await fakeUserRepository.Save();

        var accessControlManager = new AccessControlManager(agencyEmployee.Id, fakeUserRepository);
        var appointmentApplication = new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory());

        // When
        GetAppointmentResponse appointment =
            await appointmentApplication.CreateAppointment(lifeAssistant.Id, appointmentDate);

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
    
    
    [Fact]
    public async Task CreateAppointment_ByLifeAssistant_Throws()
    {
        // Given
        ApplicationUser agencyEmployee = dataFactory.CreateAgencyEmployee();
        ApplicationUser lifeAssistant = dataFactory.CreateLifeAssistant();

        DateTime appointmentDate = DateTime.Today
            .Add(TimeSpan.FromDays(1))
            .Add(TimeSpan.FromHours(16));

        var fakeUserRepository = new FakeApplicationUserRepository();
        await fakeUserRepository.Insert(agencyEmployee);
        await fakeUserRepository.Insert(lifeAssistant);
        await fakeUserRepository.Save();

        var accessControlManager = new AccessControlManager(lifeAssistant.Id, fakeUserRepository);
        var appointmentApplication = new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory());

        // When
        Func<Task> act = async () => await appointmentApplication.CreateAppointment(lifeAssistant.Id, appointmentDate);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetAppointments_ByAgencyEmployee_ReturnsAllExistingAppointments()
    {
        // Given
        ApplicationUser lifeAssistant = dataFactory.CreateLifeAssistant();
        
        DateTime dateTime1 = DateTime.Now.AddDays(1);
        DateTime dateTime2 = dateTime1.AddDays(1);
        DateTime dateTime3 = dateTime2.AddDays(1);
        lifeAssistant.Appointments = new List<Appointment>
        {
            new(dateTime1),
            new(dateTime2),
            new(dateTime3),
        };

        var fakeUserRepository = new FakeApplicationUserRepository();
        await fakeUserRepository.Insert(lifeAssistant);
        await fakeUserRepository.Save();

        var accessControlManager = new AccessControlManager(lifeAssistant.Id, fakeUserRepository);
        var application = new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory());
        
        // When
        List<GetAppointmentResponse> result = await application.GetAppointments();
        
        // Then
        result.Count.Should().Be(3);
        result[0].DateTime.Should().Be(dateTime1);
        result[1].DateTime.Should().Be(dateTime2);
        result[2].DateTime.Should().Be(dateTime3);
    }

    [Fact]
    public async Task SetAppointmentState_ByLifeAssistantOwningAppointment_SetsGivenState()
    {
        // Given
        ApplicationUser lifeAssistant = this.dataFactory.CreateLifeAssistantWithAppointments();
        
        var fakeUserRepository = new FakeApplicationUserRepository();
        await fakeUserRepository.Insert(lifeAssistant);
        await fakeUserRepository.Save();
        
        var accessControlManager = new AccessControlManager(lifeAssistant.Id, fakeUserRepository);
        var application = new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory());
        
        // When
        GetAppointmentResponse result = await application.SetAppointmentState(lifeAssistant.Id, lifeAssistant.Appointments[0].Id,
            new SetAppointStateRequest("Pending Pickup"));
        
        // Then
        fakeUserRepository.UpdatedRecords.Should().Contain(lifeAssistant.Id);
        fakeUserRepository.Saved.Should().BeTrue();
        
        result.Id.Should().Be(lifeAssistant.Appointments[0].Id);
        result.DateTime.Should().Be(lifeAssistant.Appointments[0].DateTime);
        result.LifeAssistantId.Should().Be(lifeAssistant.Id);
        result.State.Should().Be("Pending Pickup");
        Appointment updatedAppointment = (await fakeUserRepository.FindByIdWithAppointments(lifeAssistant.Id)).Appointments[0];
        updatedAppointment.State.Name.Should().Be("Pending Pickup");
    }
    
    [Fact]
    public async Task SetAppointmentState_ByOtherLifeAssistants_SetsGivenState()
    {
        // Given
        ApplicationUser lifeAssistant = this.dataFactory.CreateLifeAssistantWithAppointments();
        ApplicationUser otherLifeAssistant = this.dataFactory.CreateLifeAssistant();
        
        var fakeUserRepository = new FakeApplicationUserRepository();
        await fakeUserRepository.Insert(lifeAssistant);
        await fakeUserRepository.Insert(otherLifeAssistant);
        fakeUserRepository.InitSave();
        
        var accessControlManager = new AccessControlManager(otherLifeAssistant.Id, fakeUserRepository);
        var application = new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory());
        
        // When
        Func<Task> act = async () => await application.SetAppointmentState(lifeAssistant.Id, lifeAssistant.Appointments[0].Id,
            new SetAppointStateRequest("Pending Pickup"));
        
        // Then
        await act.Should().ThrowAsync<IllegalAccessException>();
        fakeUserRepository.UpdatedRecords.Should().BeEmpty();
        fakeUserRepository.Saved.Should().BeFalse();
    }
}