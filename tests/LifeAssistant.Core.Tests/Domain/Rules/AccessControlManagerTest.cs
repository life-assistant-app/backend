using System;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Rules;
using LifeAssistant.Core.Tests.Application.FakePersistence;
using LifeAssistant.Web.Tests;
using Xunit;

namespace LifeAssistant.Core.Tests.Domain.Rules;

public class AccessControlManagerTest
{
    private readonly DataFactory dataFactory = new DataFactory();
    
    [Fact]
    public void EnsureUserCanCreateAppointment_AgencyEmployee_DoesNotThrow()
    {
        // Given
        ApplicationUser user = this.dataFactory.CreateAgencyEmployee();
        var fakeRepository = new FakeApplicationUserRepository();
        fakeRepository.Data.Add(user);
        
        var accessControlManager = new AccessControlManager(user.Id, fakeRepository);
        
        // When
        Action act = () => accessControlManager.EnsureUserCanCreateAppointment();
        
        // Then
        act.Should().NotThrow();
    }
    
    [Fact]
    public void EnsureUserCanCreateAppointment_LifeAssistant_Throws()
    {
        // Given
        ApplicationUser user = this.dataFactory.CreateLifeAssistant();
        var fakeRepository = new FakeApplicationUserRepository();
        fakeRepository.Data.Add(user);
        
        var accessControlManager = new AccessControlManager(user.Id, fakeRepository);
        
        // When
        Func<Task> act = async () => await accessControlManager.EnsureUserCanCreateAppointment();
        
        // Then
        act.Should().ThrowAsync<InvalidOperationException>();
    }
}