using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Application.Appointments.Contracts;
using LifeAssistant.Web.Database.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LifeAssistant.Web.Tests.Integration;

public class AppointmentIntegrationTests : IntegrationTests
{
    public AppointmentIntegrationTests(WebApplicationFactory<Startup> factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateAppointment_Returns201AndInsertsRecordInDb()
    {
        // Given
        ApplicationUserEntity agencyEmployee = await this.dbDataFactory.InsertValidatedAgencyEmployee();
        ApplicationUserEntity lifeAssistant = await this.dbDataFactory.InsertValidatedLifeAssistant();
        await Login(agencyEmployee.UserName, this.dbDataFactory.UserPassword);
        var request = new CreateAppointmentRequest(DateTime.Now.AddDays(1));

        // When
        var response = await this.client.PostAsJsonAsync($"/api/assistants/{lifeAssistant.Id}/appointments", request);
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<GetAppointmentResponse>();
        result.DateTime.Should().Be(request.DateTime);
        result.State.ToString().Should().Be("Planned");
    }
    
    [Fact]
    public async Task GetsAppointment_ReturnsAppointments()
    {
        // Given
        ApplicationUserEntity agencyEmployee = await this.dbDataFactory.InsertValidatedAgencyEmployee();
        ApplicationUserEntity lifeAssistant = await this.dbDataFactory.InsertValidatedLifeAssistantWithAppointments();

        await Login(agencyEmployee.UserName, this.dbDataFactory.UserPassword);

        // When
        var response = await this.client.GetAsync($"/api/appointments");
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<GetAppointmentResponse>>();
        result.Count.Should().Be(1);
        result[0].Id.Should().Be(lifeAssistant.Appointments[0].Id);
        result[0].State.Should().Be(lifeAssistant.Appointments[0].State);
        result[0].DateTime.Date.Should().Be(lifeAssistant.Appointments[0].DateTime.Date);
    }
}