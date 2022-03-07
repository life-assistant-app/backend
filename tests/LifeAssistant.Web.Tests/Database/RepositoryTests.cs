using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Web.Database.Respositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LifeAssistant.Web.Tests.Database;

public class RepositoryTests : DatabaseTest
{
    [Fact]
    public async Task Insert_InsertsRecordInDb()
    {
        // Given
        ApplicationUser user = new ApplicationUser(
            "John Doe",
            "ized5è54é-@!é", 
            ApplicationUserRole.AgencyEmployee
        );

        var repository = new ApplicationUserRepository(this.context);

        // When
        await repository.Insert(user);
        await repository.Save();
        
        // Then
        (await context.Users.CountAsync()).Should().Be(1);
        ApplicationUser userInDb = await context.Users.FirstAsync();
        userInDb.Id.Should().Be(user.Id);
        userInDb.Username.Should().Be(user.Username);
        userInDb.Password.Should().Be(user.Password);
        userInDb.Role.Should().Be(user.Role);
        userInDb.Validated.Should().Be(user.Validated);
    }
    
}