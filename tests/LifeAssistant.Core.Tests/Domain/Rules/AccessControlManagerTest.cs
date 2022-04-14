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
        fakeRepository.InitSave();

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
        fakeRepository.InitSave();

        var accessControlManager = new AccessControlManager(user.Id, fakeRepository);

        // When
        Func<Task> act = async () => await accessControlManager.EnsureUserCanCreateAppointment();

        // Then
        await act.Should().ThrowAsync<IllegalAccessException>();
    }

    [Fact]
    public async Task EnsureCurrentUserCanReadOrUpdateUserAppointments_CurrentUserIsAnyAgencyEmployee_NoException()
    {
        // Given
        ApplicationUser lifeAssistant = this.dataFactory.CreateLifeAssistant();
        ApplicationUser agencyEmployee = this.dataFactory.CreateAgencyEmployee();
        var fakeRepository = new FakeApplicationUserRepository();
        await fakeRepository.Insert(lifeAssistant);
        await fakeRepository.Insert(agencyEmployee);
        fakeRepository.InitSave();

        var accessControlManager = new AccessControlManager(agencyEmployee.Id, fakeRepository);

        // When
        Func<Task> act = async () =>
            await accessControlManager.EnsureCurrentUserCanReadOrUpdateUserAppointments(lifeAssistant.Id);
        
        // Then
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureCurrentUserCanReadOrUpdateUserAppointments_CurrentUserIsTheLifeAssistant_NoException()
    {
        // Given
        ApplicationUser lifeAssistant = this.dataFactory.CreateLifeAssistant();
        var fakeRepository = new FakeApplicationUserRepository();
        await fakeRepository.Insert(lifeAssistant);
        fakeRepository.InitSave();

        var accessControlManager = new AccessControlManager(lifeAssistant.Id, fakeRepository);

        // When
        Func<Task> act = async () =>
            await accessControlManager.EnsureCurrentUserCanReadOrUpdateUserAppointments(lifeAssistant.Id);
        
        // Then
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task EnsureCurrentUserCanReadOrUpdateUserAppointments_CurrentUserIsAnotherLifeAssistant_Throws()
    {
        // Given
        ApplicationUser lifeAssistant = this.dataFactory.CreateLifeAssistant();
        ApplicationUser otherLifeAssistant = this.dataFactory.CreateLifeAssistant();
        var fakeRepository = new FakeApplicationUserRepository();
        await fakeRepository.Insert(lifeAssistant);
        await fakeRepository.Insert(otherLifeAssistant);
        fakeRepository.InitSave();

        var accessControlManager = new AccessControlManager(otherLifeAssistant.Id, fakeRepository);

        // When
        Func<Task> act = async () =>
            await accessControlManager.EnsureCurrentUserCanReadOrUpdateUserAppointments(lifeAssistant.Id);
        
        // Then
        await act.Should().ThrowAsync<IllegalAccessException>();
    }
}