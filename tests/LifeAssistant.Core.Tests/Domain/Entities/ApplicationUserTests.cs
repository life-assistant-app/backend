using System;
using System.Collections.Generic;
using FluentAssertions;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.ApplicationUser;
using LifeAssistant.Core.Domain.Exceptions;
using Xunit;

namespace LifeAssistant.Core.Tests.Domain.Entities;

public class ApplicationUserTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Username_InvalidValue_Throws(string invalidValue)
    {
        // When
        Action action = () => new ApplicationUser(
            invalidValue,
            "Test Password",
            "Test FirstName",
            "Test LastName",
            ApplicationUserRole.LifeAssistant
        );

        // Then
        action.Should().Throw<EntityStateException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Password_InvalidValue_Throws(string invalidValue)
    {
        // When
        Action action = () => new ApplicationUser(
            "Test Username",
            invalidValue,
            "Test FirstName",
            "Test LastName",
            ApplicationUserRole.LifeAssistant
        );

        // Then
        action.Should().Throw<EntityStateException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void FirstName_InvalidValue_Throws(string invalidValue)
    {
        // When
        Action action = () => new ApplicationUser(
            "Test Username",
            "Test Password",
            invalidValue,
            "Test LastName",
            ApplicationUserRole.LifeAssistant
        );

        // Then
        action.Should().Throw<EntityStateException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void LastName_InvalidValue_Throws(string invalidValue)
    {
        // When
        Action action = () => new ApplicationUser(
            "Test Username",
            "Test Password",
            "Test FirstName",
            invalidValue,
            ApplicationUserRole.LifeAssistant
        );

        // Then
        action.Should().Throw<EntityStateException>();
    }

    [Fact]
    public void Constructor_Creation_LoadsFieldsAndInits()
    {
        // Given
        string username = "Test Username";
        string password = "Test Password";
        string firstname = "Test FirstName";
        string lastname = "Test LastName";
        ApplicationUserRole role = ApplicationUserRole.LifeAssistant;

        // When
        var user = new ApplicationUser(username, password, firstname, lastname, role);

        // Then
        user.Id.Should().NotBeEmpty();
        user.UserName.Should().Be(username);
        user.FirstName.Should().Be(firstname);
        user.LastName.Should().Be(lastname);
        user.Role.Should().Be(role);
        user.Validated.Should().Be(false);
    }

    [Fact]
    public void Constructor_Reconstitution_LoadsFields()
    {
        // Given
        var id = Guid.NewGuid();
        const string username = "Test Username";
        const string password = "Test Password";
        const string firstname = "Test FirstName";
        const string lastname = "Test LastName";
        const ApplicationUserRole role = ApplicationUserRole.LifeAssistant;
        const bool activated = true;
        var appointments = new List<Appointment>();


        // When
        var user = new ApplicationUser(id, username, password, firstname, lastname, role, activated, appointments);

        // Then
        user.Id.Should().NotBeEmpty();
        user.UserName.Should().Be(username);
        user.FirstName.Should().Be(firstname);
        user.LastName.Should().Be(lastname);
        user.Role.Should().Be(role);
        user.Validated.Should().Be(true);
        user.Appointments.Should().NotBeNull();
        user.Appointments.Should().BeEmpty();
    }
}