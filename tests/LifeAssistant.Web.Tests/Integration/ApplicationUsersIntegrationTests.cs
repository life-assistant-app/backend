using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Application.Users.Contracts;
using LifeAssistant.Web.Database.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LifeAssistant.Web.Tests.Integration;

public class ApplicationUsersIntegrationTests : IntegrationTests
{
    public ApplicationUsersIntegrationTests(WebApplicationFactory<Startup> factory) : base(factory)
    {

    }

    [Fact]
    public async Task GetLifeAssistants_Returns200AndLifeAssistantRecords()
    {
        // Given
        ApplicationUserEntity assistant = await this.dbDataFactory.InsertValidatedLifeAssistant();
        ApplicationUserEntity agencyEmployee = await this.dbDataFactory.InsertValidatedAgencyEmployee();

        await Login(agencyEmployee.UserName, this.dbDataFactory.UserPassword);
        
        // When
        var response = await this.client.GetAsync("api/assistants");
        
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        List<GetUserResponse> data = await response.Content.ReadFromJsonAsync<List<GetUserResponse>>();

        data.Count.Should().Be(1);
        data.First().Id.Should().Be(assistant.Id);
    }
}