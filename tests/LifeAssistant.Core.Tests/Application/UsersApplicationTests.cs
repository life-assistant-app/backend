using System;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Application;
using LifeAssistant.Core.Application.Users;
using LifeAssistant.Core.Application.Users.Contracts;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Tests.Application.FakePersistence;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace LifeAssistant.Core.Tests.Application;

public class UsersApplicationTests
{
    [Fact]
    public async Task Register_Nominal_InsertsUnvalidatedUserWithHashedPassword()
    {
        // Given
        var fakeRepository = new FakeApplicationUserRepository();
        var application = new UsersApplication(fakeRepository, null);
        var username = "John Doe";
        var password = "e89fre4f!9er8@";
        var role = "LifeAssistant";

        // When
        RegisterResponse result = await application.Register(new RegisterRequest(username, password, role));

        // Then
        result.Username.Should().Be(username);
        result.Validated.Should().Be(false);
        result.Role.Should().Be(ApplicationUserRole.LifeAssistant.ToString());

        fakeRepository.Data.Should().HaveCount(1);
        fakeRepository.Data.First().Username.Should().Be(username);
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
        var username = "John Doe";
        var password = "e89fre4f!9er8@";
        ApplicationUserRole role = ApplicationUserRole.AgencyEmployee;

        var applicationUser = new ApplicationUser(username, BCrypt.Net.BCrypt.HashPassword(password), role)
        {
            Validated = true
        };

        fakeRepository.Data.Add(applicationUser);

        // When
        string tokenString = await application.Login(new LoginRequest(username, password));

        // Then
        var handler = new JwtSecurityTokenHandler();
        SecurityToken token;
        ClaimsPrincipal principal = handler.ValidateToken(tokenString, tokenValidationSpecs, out token);

        Claim[] claims = principal.Claims.ToArray();
        Claim firstClaim = claims[0];
        Claim secondClaim = claims[1];
        Claim thirdClaim = claims[2];

        firstClaim.Type.Should().Be(ClaimTypes.NameIdentifier);
        firstClaim.Value.Should().Be(applicationUser.Id.ToString());
        secondClaim.Type.Should().Be(ClaimTypes.Name);
        secondClaim.Value.Should().Be(applicationUser.Username);
        thirdClaim.Type.Should().Be(ClaimTypes.Role);
        thirdClaim.Value.Should().Be("AgencyEmployee");
    }

    [Fact]
    public async Task Login_BadCredentials_ThrowsException()
    {
        // Given
        var fakeRepository = new FakeApplicationUserRepository();
        var jwtSecret = "my-secret-string-to-sign-jwt-token";

        var application = new UsersApplication(fakeRepository, jwtSecret);
        var username = "John Doe";
        var password = "e89fre4f!9er8@";
        ApplicationUserRole role = ApplicationUserRole.AgencyEmployee;

        var applicationUser = new ApplicationUser(username, BCrypt.Net.BCrypt.HashPassword(password), role)
        {
            Validated = true
        };
        fakeRepository.Data.Add(applicationUser);

        // When
        Func<Task> act = async () => await application.Login(new LoginRequest(username, "not the right password"));

        // Then
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Login_Unvalidateduser_ThrowsException()
    {
        // Given
        var fakeRepository = new FakeApplicationUserRepository();
        var jwtSecret = "my-secret-string-to-sign-jwt-token";

        var application = new UsersApplication(fakeRepository, jwtSecret);
        var username = "John Doe";
        var password = "e89fre4f!9er8@";
        ApplicationUserRole role = ApplicationUserRole.AgencyEmployee;

        var applicationUser = new ApplicationUser(username, BCrypt.Net.BCrypt.HashPassword(password), role);
        fakeRepository.Data.Add(applicationUser);

        // When
        Func<Task> act = async () => await application.Login(new LoginRequest(username, password));

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}