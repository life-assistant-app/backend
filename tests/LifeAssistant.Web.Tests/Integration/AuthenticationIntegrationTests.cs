using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Application.Users.Contracts;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Web.Tests.Integration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LifeAssistant.Web.Tests;

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
            "John Doe",
            "dgy!zue654)à5@64dez",
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
        payload.Username.Should().Be("John Doe");

        ApplicationUser userInDb = await this.assertDbContext.Users.FirstAsync(u => u.Id == payload.Id);
        userInDb.Role.Should().Be(ApplicationUserRole.LifeAssistant);
        userInDb.Validated.Should().Be(false);
        userInDb.Username.Should().Be("John Doe");
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnsToken()
    {
        // Given
        var user = new ApplicationUser(
            "John Doe", 
            BCrypt.Net.BCrypt.HashPassword("dgy!zue654)à5@64dez"),
            ApplicationUserRole.LifeAssistant
        )
        {
            Validated = true
        };

        await this.givenDbContext.Users.AddAsync(user);
        await this.givenDbContext.SaveChangesAsync();

        var request = new LoginRequest("John Doe", "dgy!zue654)à5@64dez");
        
        // When
        var response = await this.client.PostAsJsonAsync("/api/auth/login", request);
        
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        string token = await response.Content.ReadAsStringAsync();
        token.Should().NotBe(string.Empty);
    }
}