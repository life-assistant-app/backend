using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Application.Appointments;
using LifeAssistant.Core.Application.Appointments.Contracts;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.ApplicationUser;
using LifeAssistant.Core.Domain.Entities.Appointments;
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
    public async Task CleanupAppointments_DeletesAppointmentToDelete()
    {
        // Given
        var appointmentStateFactory = new AppointmentStateFactory();
        var appointments = new List<Appointment>
        {
            new(
                Guid.NewGuid(),
                DateTime.Now.AddDays(1), 
                appointmentStateFactory, 
                "Planned",
                DateOnly.FromDateTime(DateTime.Now.AddDays(-3))
            ),
            new(
                Guid.NewGuid(),
                DateTime.Now.AddDays(3), 
                appointmentStateFactory, 
                "Refused",
                DateOnly.FromDateTime(DateTime.Now.AddDays(-1))
            )
        };
        var fakeAppointmentRepository = new FakeAppointmentRepository(appointments);

        Guid appointmentToDeleteId = appointments[1].Id;

        var appointmentApplication = new AppointmentsApplication(
                null,
                null,
                new AppointmentStateFactory(),
                fakeAppointmentRepository
        );

        // When
        await appointmentApplication.CleanupAppointments();

        // Then
        fakeAppointmentRepository.DeletedAppointments.Should().ContainSingle();
        fakeAppointmentRepository.DeletedAppointments.First().Id.Should().Be(appointmentToDeleteId);
    }

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
        fakeUserRepository.InitSave();

        var accessControlManager = new AccessControlManager(agencyEmployee.Id, fakeUserRepository);
        var appointmentApplication =
            new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory(), null);

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
        var appointmentApplication =
            new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory(), null);

        // When
        Func<Task> act = async () => await appointmentApplication.CreateAppointment(lifeAssistant.Id, appointmentDate);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetAllAppointments_ByAgencyEmployee_ReturnsAllExistingAppointments()
    {
        // Given
        ApplicationUser lifeAssistant = dataFactory.CreateLifeAssistant();
        ApplicationUser agencyEmployee = dataFactory.CreateAgencyEmployee();

        DateTime dateTime1 = DateTime.Now.AddDays(3);
        DateTime dateTime2 = DateTime.Now.AddDays(2);
        DateTime dateTime3 = DateTime.Now.AddDays(1);
        lifeAssistant.Appointments = new List<Appointment>
        {
            new(dateTime1),
            new(dateTime2),
            new(dateTime3),
        };

        var fakeUserRepository = new FakeApplicationUserRepository();
        await fakeUserRepository.Insert(lifeAssistant);
        await fakeUserRepository.Insert(agencyEmployee);
        fakeUserRepository.InitSave();

        var accessControlManager = new AccessControlManager(agencyEmployee.Id, fakeUserRepository);
        var application =
            new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory(), null);

        // When
        List<GetAppointmentResponse> result = await application.GetAllAppointments();

        // Then
        result.Count.Should().Be(3);
        result[0].Id.Should().Be(lifeAssistant.Appointments[2].Id);
        result[1].Id.Should().Be(lifeAssistant.Appointments[1].Id);
        result[2].Id.Should().Be(lifeAssistant.Appointments[0].Id);
    }
    [Fact]
    public async Task GetAppointments_ByLifeAssistant_Throws()
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
        fakeUserRepository.InitSave();

        var accessControlManager = new AccessControlManager(lifeAssistant.Id, fakeUserRepository);
        var application = new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory(), null);
        
        // When
        Func<Task> act = async () => await application.GetAllAppointments();
        
        // Then
        await act.Should().ThrowAsync<IllegalAccessException>();
    }

    
    [Fact]
    public async Task GetAppointments_ByAgencyEmployee_ReturnsAllExistingAppointments()
    {
        // Given
        ApplicationUser agencyEmployee = dataFactory.CreateAgencyEmployee();
        ApplicationUser lifeAssistant = dataFactory.CreateLifeAssistant();
        
        DateTime dateTime1 = DateTime.Now.AddDays(1);
        DateTime dateTime2 = dateTime1.AddDays(1);
        DateTime dateTime3 = dateTime2.AddDays(1);
        lifeAssistant.Appointments = new List<Appointment>
        {
            new(dateTime3),
            new(dateTime2),
            new(dateTime1),
        };

        var fakeUserRepository = new FakeApplicationUserRepository();
        await fakeUserRepository.Insert(lifeAssistant);
        await fakeUserRepository.Insert(agencyEmployee);
        fakeUserRepository.InitSave();

        var accessControlManager = new AccessControlManager(agencyEmployee.Id, fakeUserRepository);
        var application = new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory(),null);
        
        // When
        List<GetAppointmentResponse> result = await application.GetAllAppointments();
        
        // Then
        result.Count.Should().Be(3);
        result[0].Id.Should().Be(lifeAssistant.Appointments[2].Id);
        result[1].Id.Should().Be(lifeAssistant.Appointments[1].Id);
        result[2].Id.Should().Be(lifeAssistant.Appointments[0].Id);
    }

    public async Task GetAllAppointments_ByLifeAssistant_Throws()
    {
        // Given
        ApplicationUser lifeAssistant = dataFactory.CreateLifeAssistant();
        ApplicationUser agencyEmployee = dataFactory.CreateAgencyEmployee();

        DateTime dateTime1 = DateTime.Now.AddDays(3);
        DateTime dateTime2 = DateTime.Now.AddDays(2);
        DateTime dateTime3 = DateTime.Now.AddDays(1);
        lifeAssistant.Appointments = new List<Appointment>
        {
            new(dateTime1),
            new(dateTime2),
            new(dateTime3),
        };

        var fakeUserRepository = new FakeApplicationUserRepository();
        await fakeUserRepository.Insert(lifeAssistant);
        await fakeUserRepository.Insert(agencyEmployee);
        fakeUserRepository.InitSave();

        var accessControlManager = new AccessControlManager(agencyEmployee.Id, fakeUserRepository);
        var application =
            new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory(), null);

        // When
        Func<Task> act = async () => await application.GetAllAppointments();

        // Then
        await act.Should().ThrowAsync<IllegalAccessException>();
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
        var application =
            new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory(), null);

        // When
        GetAppointmentResponse result = await application.SetAppointmentState(lifeAssistant.Id,
            lifeAssistant.Appointments[0].Id,
            new SetAppointStateRequest("Pending Pickup"));

        // Then
        fakeUserRepository.UpdatedRecords.Should().Contain(lifeAssistant.Id);
        fakeUserRepository.Saved.Should().BeTrue();

        result.Id.Should().Be(lifeAssistant.Appointments[0].Id);
        result.DateTime.Should().Be(lifeAssistant.Appointments[0].DateTime);
        result.LifeAssistantId.Should().Be(lifeAssistant.Id);
        result.State.Should().Be("Pending Pickup");
        Appointment updatedAppointment =
            (await fakeUserRepository.FindByIdWithAppointments(lifeAssistant.Id)).Appointments[0];
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
        var application =
            new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory(), null);

        // When
        Func<Task> act = async () => await application.SetAppointmentState(lifeAssistant.Id,
            lifeAssistant.Appointments[0].Id,
            new SetAppointStateRequest("Pending Pickup"));

        // Then
        await act.Should().ThrowAsync<IllegalAccessException>();
        fakeUserRepository.UpdatedRecords.Should().BeEmpty();
        fakeUserRepository.Saved.Should().BeFalse();
    }

    [Fact]
    public async Task GetAssistantAppointments_WithState_GetOnlyAppointmentWithGivenState()
    {
        // Given
        ApplicationUser lifeAssistant = this.dataFactory.CreateLifeAssistantWithAppointments();
        Appointment appointment = lifeAssistant.Appointments.First();

        appointment.State = new AppointmentStateFactory().BuildStateFromAppointment(appointment, "Pending Pickup");
        var fakeUserRepository = new FakeApplicationUserRepository();
        await fakeUserRepository.Insert(lifeAssistant);
        fakeUserRepository.InitSave();

        var accessControlManager = new AccessControlManager(lifeAssistant.Id, fakeUserRepository);
        var application =
            new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory(), null);

        // When
        List<GetAppointmentResponse> result =
            await application.GetLifeAssistantAppointments(lifeAssistant.Id, "Pending Pickup");

        // Then
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(appointment.Id);
        result.First().State.Should().Be("Pending Pickup");
        result.First().DateTime.Should().Be(appointment.DateTime);
        result.First().LifeAssistantId.Should().Be(lifeAssistant.Id);
    }

    [Fact]
    public async Task GetAssistantAppointments_NoState_ReturnsAllAppointmentsFromLifeAssistant()
    {
        // Given
        ApplicationUser lifeAssistant = this.dataFactory.CreateLifeAssistantWithAppointments();
        Appointment appointment = lifeAssistant.Appointments.First();

        appointment.State = new AppointmentStateFactory().BuildStateFromAppointment(appointment, "Pending Pickup");
        var fakeUserRepository = new FakeApplicationUserRepository();
        await fakeUserRepository.Insert(lifeAssistant);
        fakeUserRepository.InitSave();

        var accessControlManager = new AccessControlManager(lifeAssistant.Id, fakeUserRepository);
        var application =
            new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory(), null);

        // When
        List<GetAppointmentResponse> result = await application.GetLifeAssistantAppointments(lifeAssistant.Id);

        // Then
        result.Should().HaveCount(3);
        result[0].Id.Should().Be(lifeAssistant.Appointments[2].Id);
        result[1].Id.Should().Be(lifeAssistant.Appointments[1].Id);
        result[2].Id.Should().Be(lifeAssistant.Appointments[0].Id);
    }

    [Fact]
    public async Task GetAssistantAppointments_OtherLifeAssistant_ReturnsAllAppointmentsFromLifeAssistant()
    {
        // Given
        ApplicationUser lifeAssistant = this.dataFactory.CreateLifeAssistantWithAppointments();
        ApplicationUser otherLifeAssistant = this.dataFactory.CreateLifeAssistant();
        Appointment appointment = lifeAssistant.Appointments.First();

        appointment.State = new AppointmentStateFactory().BuildStateFromAppointment(appointment, "Pending Pickup");
        var fakeUserRepository = new FakeApplicationUserRepository();
        await fakeUserRepository.Insert(lifeAssistant);
        await fakeUserRepository.Insert(otherLifeAssistant);
        fakeUserRepository.InitSave();

        var accessControlManager = new AccessControlManager(otherLifeAssistant.Id, fakeUserRepository);
        var application =
            new AppointmentsApplication(fakeUserRepository, accessControlManager, new AppointmentStateFactory(), null);

        // When
        Func<Task> act = async () => await application.GetLifeAssistantAppointments(lifeAssistant.Id);

        // Then
        await act.Should().ThrowAsync<IllegalAccessException>();
    }
}