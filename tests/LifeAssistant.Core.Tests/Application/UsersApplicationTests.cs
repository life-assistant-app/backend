using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Application.Users;
using LifeAssistant.Core.Application.Users.Contracts;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Tests.Application.FakePersistence;
using LifeAssistant.Web.Tests;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace LifeAssistant.Core.Tests.Application;

public class UsersApplicationTests
{
    private DataFactory dataFactory = new DataFactory();

    [Fact]
    public async Task Register_Nominal_InsertsUnvalidatedUserWithHashedPassword()
    {
        // Given
        var fakeRepository = new FakeApplicationUserRepository();
        var application = new UsersApplication(fakeRepository, string.Empty);
        var username = "shepard.n7";
        var password = "e89fre4f!9er8@";
        var firstName = "John";
        var lastName = "Shepard";
        var role = "LifeAssistant";

        // When
        RegisterResponse result = await application.Register(
            new RegisterRequest(username, password, firstName, lastName, role)
        );

        // Then
        result.Username.Should().Be(username);
        result.FirstName.Should().Be(firstName);
        result.LastName.Should().Be(lastName);
        result.Validated.Should().Be(false);
        result.Role.Should().Be(ApplicationUserRole.LifeAssistant.ToString());

        fakeRepository.Data.Should().HaveCount(1);
        fakeRepository.Data.First().UserName.Should().Be(username);
        fakeRepository.Data.First().FirstName.Should().Be(firstName);
        fakeRepository.Data.First().LastName.Should().Be(lastName);
        fakeRepository.Data.First().Password.Should().NotBe(password);
        fakeRepository.Data.First().Validated.Should().Be(false);
        fakeRepository.Data.First().Role.Should().Be(ApplicationUserRole.LifeAssistant);
        BCrypt.Net.BCrypt.Verify(password, fakeRepository.Data.First().Password);
    }

    [Fact]
    public async Task Login_GoodCredentialsAndValidatedUser_ReturnsJwtToken()
    {
        // Given
        var fakeRepository = new FakeApplicationUserRepository();
        var jwtSecret = "my-secret-string-to-sign-jwt-token";
        var tokenValidationSpecs = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        var application = new UsersApplication(fakeRepository, jwtSecret);
        ApplicationUser applicationUser = this.dataFactory.CreateAgencyEmployee();
        applicationUser.Validated = true;

        await fakeRepository.Insert(applicationUser);
        await fakeRepository.Save();

        // When
        LoginResponse response = await application.Login(
            new LoginRequest(applicationUser.UserName, this.dataFactory.UserPassword)
        );

        // Then
        string tokenString = response.SecurityToken;
        var handler = new JwtSecurityTokenHandler();
        SecurityToken token;
        ClaimsPrincipal principal = handler.ValidateToken(tokenString, tokenValidationSpecs, out token);

        Claim[] claims = principal.Claims.ToArray();
        Claim firstClaim = claims[0];
        Claim secondClaim = claims[1];
        Claim thirdClaim = claims[2];
        Claim fourthClaim = claims[3];

        firstClaim.Type.Should().Be(ClaimTypes.NameIdentifier);
        firstClaim.Value.Should().Be(applicationUser.Id.ToString());
        secondClaim.Type.Should().Be(ClaimTypes.Name);
        secondClaim.Value.Should().Be(applicationUser.UserName);
        thirdClaim.Type.Should().Be(ClaimTypes.Role);
        thirdClaim.Value.Should().Be("AgencyEmployee");
        fourthClaim.Type.Should().Be(ClaimTypes.Surname);
        fourthClaim.Value.Should().Be("John Shepard");
    }

    [Fact]
    public async Task Login_BadCredentials_ThrowsException()
    {
        // Given
        var fakeRepository = new FakeApplicationUserRepository();
        var jwtSecret = "my-secret-string-to-sign-jwt-token";

        var application = new UsersApplication(fakeRepository, jwtSecret);

        ApplicationUser applicationUser = this.dataFactory.CreateAgencyEmployee();
        await fakeRepository.Insert(applicationUser);
        await fakeRepository.Save();

        // When
        Func<Task> act = async () =>
            await application.Login(new LoginRequest(applicationUser.UserName, "not the right password"));

        // Then
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Login_UnvalidatedUser_ThrowsException()
    {
        // Given
        var fakeRepository = new FakeApplicationUserRepository();
        var jwtSecret = "my-secret-string-to-sign-jwt-token";

        var application = new UsersApplication(fakeRepository, jwtSecret);

        var applicationUser = this.dataFactory.CreateAgencyEmployee();
        await fakeRepository.Insert(applicationUser);
        await fakeRepository.Save();

        // When
        Func<Task> act = async () =>
            await application.Login(new LoginRequest(applicationUser.UserName, this.dataFactory.UserPassword));

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}