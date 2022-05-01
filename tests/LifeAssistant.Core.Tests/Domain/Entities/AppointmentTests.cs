using System;
using FluentAssertions;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.Appointments;
using LifeAssistant.Core.Domain.Entities.AppointmentState;
using LifeAssistant.Core.Domain.Exceptions;
using LifeAssistant.Web.Tests;
using Xunit;

namespace LifeAssistant.Core.Tests.Domain.Entities;

public class AppointmentTests
{
    private DataFactory dataFactory = new DataFactory();

    [Fact]
    public void Appointment_JustCreate_IsPlanned()
    {
        // Given
        var dateTime = DateTime.Now.Add(TimeSpan.FromDays(3));

        // When
        var appointment = new Appointment(
            dateTime
        );

        // Then
        appointment.Id.Should().NotBeEmpty();
        appointment.State.Name.Should().Be("Planned");
        appointment.DateTime.Should().Be(dateTime);
    }

    [Fact]
    public void Appointment_AcceptPlanned_IsAccepted()
    {
        // Given
        var appointment = new Appointment(DateTime.Now.Add(TimeSpan.FromDays(3)));

        // When
        appointment.State = new PendingAppointmentState();

        // Then
        appointment.State.Name.Should().Be("Pending Pickup");
    }

    [Fact]
    public void Appointment_RefusePlanned_IsRefused()
    {
        // Given
        var appointment = new Appointment(DateTime.Now.Add(TimeSpan.FromDays(3)));

        // When
        appointment.State = new RefusedAppointmentState();

        // Then
        appointment.State.Name.Should().Be("Refused");
    }

    [Fact]
    public void Appointment_RefusePending_Throws()
    {
        // Given
        var appointment = new Appointment(DateTime.Now.Add(TimeSpan.FromDays(3)));
        appointment.State = new PendingAppointmentState();
        // When
        Action act = () => appointment.State = new RefusedAppointmentState();

        // Then
        act.Should().Throw<EntityStateException>();
    }

    [Fact]
    public void Appointment_RefuseFinished_Throws()
    {
        // Given
        var appointment = new Appointment(DateTime.Now.Add(TimeSpan.FromDays(3)));
        appointment.State = new PendingAppointmentState();
        appointment.State = new FinishedAppointmentState();

        // When
        Action act = () => appointment.State = new RefusedAppointmentState();

        // Then
        act.Should().Throw<EntityStateException>();
    }

    [Fact]
    public void Appointment_AcceptFinished_Throws()
    {
        // Given
        var appointment = new Appointment(DateTime.Now.Add(TimeSpan.FromDays(3)));
        appointment.State = new PendingAppointmentState();
        appointment.State = new FinishedAppointmentState();

        // When
        Action act = () => appointment.State = new PendingAppointmentState();

        // Then
        act.Should().Throw<EntityStateException>();
    }

    [Fact]
    public void Appointment_PickupFinished_Throws()
    {
        // Given
        var appointment = new Appointment(DateTime.Now.Add(TimeSpan.FromDays(3)));

        appointment.State = new PendingAppointmentState();
        appointment.State = new FinishedAppointmentState();

        // When
        Action act = () => appointment.State = new PendingAppointmentState();

        // Then
        act.Should().Throw<EntityStateException>();
    }

    [Fact]
    public void Appointment_AcceptPending_Throws()
    {
        // Given
        var appointment = new Appointment(DateTime.Now.Add(TimeSpan.FromDays(3)));
        appointment.State = new PendingAppointmentState();

        // When
        Action act = () => appointment.State = new PendingAppointmentState();
        
        // Then
        act.Should().Throw<EntityStateException>();
    }

    [Fact]
    public void Appointment_PickUpPlanned_Throws()
    {
        // Given
        var appointment = new Appointment(DateTime.Now.Add(TimeSpan.FromDays(3)));

        // When
        Action act = () => appointment.State = new FinishedAppointmentState();

        // Then
        act.Should().Throw<EntityStateException>();
    }

    [Fact]
    public void Appointment_PickUpPending_IsFinished()
    {
        // Given
        var appointment = new Appointment(DateTime.Now.Add(TimeSpan.FromDays(3)));
        appointment.State = new PendingAppointmentState();

        // When
        appointment.State = new FinishedAppointmentState();

        // Then
        appointment.State.Name.Should().Be("Finished");
    }

    [Fact]
    public void Appointment_DateInThePast_Throws()
    {
        // Given
        var dateTime = DateTime.Now.Subtract(TimeSpan.FromDays(3));

        // When
        Action act = () => new Appointment(dateTime);

        // Then
        act.Should().Throw<EntityStateException>();
    }

    [Fact]
    public void Appointment_DateInThePastButFinished_DoesntThrows()
    {
        // Given
        var dateTime = DateTime.Now.Subtract(TimeSpan.FromDays(3));

        // When
        Action act = () => new Appointment(
            Guid.NewGuid(),
            dateTime, 
            new AppointmentStateFactory(),
            "Finished",
            DateOnly.FromDateTime(DateTime.Today)
        );

        // Then
        act.Should().NotThrow();
    }

    [Fact]
    public void Appointment_Creation_InitsCreatedDateToNow()
    {
        // When
        var appointment = new Appointment(DateTime.Now.AddDays(3));

        // Then
        appointment.CreatedDate.Should().Be(DateOnly.FromDateTime(DateTime.Today));
    }
}