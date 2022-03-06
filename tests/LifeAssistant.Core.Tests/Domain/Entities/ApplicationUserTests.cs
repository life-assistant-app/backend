using System;
using FluentAssertions;
using LifeAssistant.Core.Domain.Entities;
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
        Action action = () => new ApplicationUser(invalidValue, "Test Password", ApplicationUserRole.LifeAssistant);
        
        // Then
        action.Should().Throw<ArgumentException>();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Password_InvalidValue_Throws(string invalidValue)
    {
        // When
        Action action = () => new ApplicationUser("Test Username", invalidValue, ApplicationUserRole.LifeAssistant);
        
        // Then
        action.Should().Throw<ArgumentException>();
    }
}