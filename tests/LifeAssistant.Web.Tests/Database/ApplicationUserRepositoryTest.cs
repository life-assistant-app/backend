using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Web.Database.Respositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LifeAssistant.Web.Tests.Database;

public class ApplicationUserRepositoryTest : DatabaseTest
{
    [Fact]
    public async Task FindByUsername_WithExistingUserName_InsertsRecordInDb()
    {
        // Given
        ApplicationUser user = this.dataFactory.CreateAgencyEmployee();
        await this.context.Users.AddAsync(user);
        await this.context.SaveChangesAsync();
        
        var repository = new ApplicationUserRepository(this.context);

        // When
        ApplicationUser result = await repository.FindByUsername(user.Username);
        
        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Username.Should().Be(user.Username);
        result.Password.Should().Be(user.Password);
        result.Role.Should().Be(user.Role);
        result.Validated.Should().Be(user.Validated);
    }
}