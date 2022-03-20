using System;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Web.Database.Entities;
using LifeAssistant.Web.Database.Respositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LifeAssistant.Web.Tests.Database;

public class ApplicationUserRepositoryTest : DatabaseTest
{
    [Fact]
    public async Task FindByUsername_WithExistingUserName_ReturnsUserRecord()
    {
        // Given
        ApplicationUserEntity entity = await this.dbDataFactory.InsertValidatedAgencyEmployeeEntity();
        var repository = new ApplicationUserRepository(this.context);

        // When
        ApplicationUser result = await repository.FindByUsername(entity.UserName);
        
        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(entity.Id);
        result.UserName.Should().Be(entity.UserName);
        result.Password.Should().Be(entity.Password);
        result.Role.Should().Be(entity.Role);
        result.Validated.Should().Be(entity.Validated);
    }
    
    [Fact]
    public async Task Insert_InsertsRecordInDb()
    {
        // Given
        ApplicationUser user = this.dataFactory.CreateAgencyEmployee();

        var repository = new ApplicationUserRepository(this.context);

        // When
        await repository.Insert(user);
        await repository.Save();
        
        // Then
        (await context.Users.CountAsync()).Should().Be(1);
        ApplicationUserEntity userInDb = await context.Users.FirstAsync();
        userInDb.Id.Should().Be(user.Id);
        userInDb.UserName.Should().Be(user.UserName);
        userInDb.Password.Should().Be(user.Password);
        userInDb.FirstName.Should().Be(user.FirstName);
        userInDb.LastName.Should().Be(user.LastName);
        userInDb.Role.Should().Be(user.Role);
        userInDb.Validated.Should().Be(user.Validated);
    }
}