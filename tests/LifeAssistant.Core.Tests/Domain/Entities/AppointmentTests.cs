using System;
using FluentAssertions;
using LifeAssistant.Core.Domain.Entities;
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
            this.dataFactory.CreateLifeAssistant(),
            dateTime
        );
        
        // Then
        appointment.Id.Should().NotBeEmpty();
        appointment.State.Should().Be("Planned");
        appointment.DateTime.Should().Be(dateTime);
    }
    
    [Fact]
    public void Appointment_AcceptPlanned_IsAccepted()
    {
        // Given
        var appointment = new Appointment(
            this.dataFactory.CreateLifeAssistant(),
            DateTime.Now.Add(TimeSpan.FromDays(3))
        );

        // When
        appointment.Accept();
        
        // Then
        appointment.State.Should().Be("Pending Pickup");
    }
    
    [Fact]
    public void Appointment_AcceptFinished_Throws()
    {
        // Given
        var appointment = new Appointment(
            this.dataFactory.CreateLifeAssistant(),
            DateTime.Now.Add(TimeSpan.FromDays(3))
        );
        appointment.Accept();
        appointment.Pickup();

        // When
        Action act = () => appointment.Accept();
        
        // Then
        act.Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void Appointment_PickupFinished_Throws()
    {
        // Given
        var appointment = new Appointment(
            this.dataFactory.CreateLifeAssistant(),
            DateTime.Now.Add(TimeSpan.FromDays(3))
        );

        appointment.Accept();
        appointment.Pickup();

        // When
        Action act = () => appointment.Pickup();
        
        // Then
        act.Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void Appointment_AcceptPending_Throws()
    {
        // Given
        var appointment = new Appointment(
            this.dataFactory.CreateLifeAssistant(),
            DateTime.Now.Add(TimeSpan.FromDays(3))
        );
        appointment.Accept();

        // When
        Action act = () => appointment.Accept();
        
        // Then
        act.Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void Appointment_PickUpPlanned_Throws()
    {
        // Given
        var appointment = new Appointment(
            this.dataFactory.CreateLifeAssistant(),
            DateTime.Now.Add(TimeSpan.FromDays(3))
        );

        // When
        Action act = () => appointment.Pickup();
        
        // Then
        act.Should().Throw<InvalidOperationException>();
    }
    
    [Fact]
    public void Appointment_PickUpPending_IsFinished()
    {
        // Given
        var appointment = new Appointment(
            this.dataFactory.CreateLifeAssistant(),
            DateTime.Now.Add(TimeSpan.FromDays(3))
        );
        appointment.Accept();

        // When
        appointment.Pickup();
        
        // Then
        appointment.State.Should().Be("Finished");
    }
    
    [Fact]
    public void Appointment_DateInThePast_Throws()
    {
        // Given
        var dateTime = DateTime.Now.Subtract(TimeSpan.FromDays(3));
        
        // When
        Action act = () => new Appointment(
            this.dataFactory.CreateAgencyEmployee(),
            dateTime
        );
        
        // Then
        act.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Appointment_WithAgencyEmployee_Throws()
    {
        // Given
        var dateTime = DateTime.Now.Add(TimeSpan.FromDays(3));
        
        // When
        Action act = () => new Appointment(
            this.dataFactory.CreateAgencyEmployee(),
            dateTime
        );
        
        // Then
        act.Should().Throw<ArgumentException>();
    }
}