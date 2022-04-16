using System;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.ApplicationUser;
using LifeAssistant.Core.Domain.Exceptions;
using LifeAssistant.Core.Domain.Rules;
using LifeAssistant.Core.Tests.Application.FakePersistence;
using LifeAssistant.Web.Tests;
using Xunit;

namespace LifeAssistant.Core.Tests.Domain.Rules;

public class AccessControlManagerTest
{
    private readonly DataFactory dataFactory = new DataFactory();

    [Fact]
    public async Task EnsureUserCanCreateAppointment_AgencyEmployee_DoesNotThrow()
    {
        // Given
        ApplicationUser user = this.dataFactory.CreateAgencyEmployee();
        var fakeRepository = new FakeApplicationUserRepository();
        await fakeRepository.Insert(user);
        await fakeRepository.Save();

        var accessControlManager = new AccessControlManager(user.Id, fakeRepository);

        // When
        Action act = () => accessControlManager.EnsureUserCanCreateAppointment();

        // Then
        act.Should().NotThrow();
    }

    [Fact]
    public async Task EnsureUserCanCreateAppointment_LifeAssistant_Throws()
    {
        // Given
        ApplicationUser user = this.dataFactory.CreateLifeAssistant();
        var fakeRepository = new FakeApplicationUserRepository();
        await fakeRepository.Insert(user);
        await fakeRepository.Save();

        var accessControlManager = new AccessControlManager(user.Id, fakeRepository);

        // When
        Func<Task> act = async () => await accessControlManager.EnsureUserCanCreateAppointment();

        // Then
        await act.Should().ThrowAsync<IllegalAccessException>();
    }

    [Fact]
    public async Task EnsureIsAgencyEmployee_AgencyEmployee_DontThrows()
    {
        // Given
        ApplicationUser user = this.dataFactory.CreateAgencyEmployee();
        var fakeRepository = new FakeApplicationUserRepository();
        await fakeRepository.Insert(user);
        await fakeRepository.Save();

        var accessControlManager = new AccessControlManager(user.Id, fakeRepository);
        
        // When
        Func<Task> act = async () => await accessControlManager.EnsureIsAgencyEmployee();
        
        // Then
        await act.Should().NotThrowAsync();
    }
    
    [Fact]
    public async Task EnsureIsAgencyEmployee_LifeAssistant_Throws()
    {
        // Given
        ApplicationUser user = this.dataFactory.CreateLifeAssistant();
        var fakeRepository = new FakeApplicationUserRepository();
        await fakeRepository.Insert(user);
        await fakeRepository.Save();

        var accessControlManager = new AccessControlManager(user.Id, fakeRepository);

        // When
        Func<Task> act = async () => await accessControlManager.EnsureIsAgencyEmployee();

        // Then
        await act.Should().ThrowAsync<IllegalAccessException>();
    }
}