using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Application.Users.Contracts;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Web.Tests.Integration;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LifeAssistant.Web.Tests;

public class AuthenticationIntegrationTests : IntegrationTests
{
    
    public AuthenticationIntegrationTests(WebApplicationFactory<Startup> factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task Register_Norminal_InsertsNewUserInDb()
    {
        // Given
        var request = new RegisterRequest(
            "John Doe", 
            "dgy!zue654)à5@64dez", 
            ApplicationUserRole.LifeAssistant.ToString()
        );
        
        // When
        var response = await this.client.PostAsJsonAsync("/api/auth/register",request);
        
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        RegisterResponse payload = await response.Content.ReadFromJsonAsync<RegisterResponse>();

        payload.Id.Should().NotBeNull();
        payload.Role.Should().Be(ApplicationUserRole.LifeAssistant.ToString());
        payload.Validated.Should().Be(false);
        payload.Username.Should().Be("John Doe");
    }


}