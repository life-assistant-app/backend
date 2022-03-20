using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Application.Users.Contracts;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Web.Database.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LifeAssistant.Web.Tests.Integration;

public class AuthenticationIntegrationTests : IntegrationTests
{
    public AuthenticationIntegrationTests(WebApplicationFactory<Startup> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_InsertsNewUserInDb()
    {
        // Given
        var request = new RegisterRequest(
            "shepard.n7",
            "dgy!zue654)à5@64dez",
            "John",
            "Shepard",
            ApplicationUserRole.LifeAssistant.ToString()
        );

        // When
        var response = await this.client.PostAsJsonAsync("/api/auth/register", request);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        RegisterResponse? payload = await response.Content.ReadFromJsonAsync<RegisterResponse>();

        payload.Id.Should().NotBeEmpty();
        payload.Role.Should().Be(ApplicationUserRole.LifeAssistant.ToString());
        payload.Validated.Should().Be(false);
        payload.Username.Should().Be(request.Username);
        payload.FirstName.Should().Be(request.FirstName);
        payload.LastName.Should().Be(request.LastName);

        ApplicationUserEntity userInDb = await this.assertDbContext.Users.FirstAsync(u => u.Id == payload.Id);
        userInDb.Role.Should().Be(ApplicationUserRole.LifeAssistant);
        userInDb.Validated.Should().Be(false);
        userInDb.UserName.Should().Be(request.Username);
        userInDb.FirstName.Should().Be(request.FirstName);
        userInDb.LastName.Should().Be(request.LastName);
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnsToken()
    {
        // Given
        var user = await this.dbDataFactory.InsertValidatedLifeAssistant();

        var request = new LoginRequest(user.UserName, this.dbDataFactory.UserPassword);
        
        // When
        var response = await this.client.PostAsJsonAsync("/api/auth/login", request);
        
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        LoginResponse token = await response.Content.ReadFromJsonAsync<LoginResponse>();
        token.SecurityToken.Should().NotBe(string.Empty);
    }
    
    [Fact]
    public async Task Login_WithInCorrectCredentials_ReturnsToken()
    {
        // Given
        var user = await this.dbDataFactory.InsertValidatedLifeAssistant();

        var request = new LoginRequest("invalid username","invalid passowrd");
        
        // When
        var response = await this.client.PostAsJsonAsync("/api/auth/login", request);
        
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}