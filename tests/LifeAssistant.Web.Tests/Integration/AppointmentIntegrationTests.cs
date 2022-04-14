using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using LifeAssistant.Core.Application.Appointments.Contracts;
using LifeAssistant.Web.Database;
using LifeAssistant.Web.Database.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
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

        AppointmentEntity appointmentEntityFromDb = (await this.assertDbContext
            .Users
            .Include(record => record.Appointments)
            .FirstAsync(record => record.Id == lifeAssistant.Id))
            .Appointments
            .First();
        appointmentEntityFromDb.DateTime.Date.Should().Be(request.DateTime.Date);
        appointmentEntityFromDb.State.Should().Be("Planned");
    }
    
    [Fact]
    public async Task CreateAppointment_NonExistingAssistant_Returns404AndDoesNotInsert()
    {
        // Given
        ApplicationUserEntity agencyEmployee = await this.dbDataFactory.InsertValidatedAgencyEmployee();
        await Login(agencyEmployee.UserName, this.dbDataFactory.UserPassword);
        var request = new CreateAppointmentRequest(DateTime.Now.AddDays(1));

        // When
        var response = await this.client.PostAsJsonAsync($"/api/assistants/{Guid.NewGuid()}/appointments", request);
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        
        int numberOfAppointmentsInDb = await this.assertDbContext.Appointments.CountAsync();
        numberOfAppointmentsInDb.Should().Be(0);
    }
    
    [Fact]
    public async Task CreateAppointment_DoneByAssistant_Returns403AndDoesNotInsert()
    {
        // Given
        ApplicationUserEntity lifeAssistant = await this.dbDataFactory.InsertValidatedLifeAssistant();
        await Login(lifeAssistant.UserName, this.dbDataFactory.UserPassword);
        var request = new CreateAppointmentRequest(DateTime.Now.AddDays(1));

        // When
        var response = await this.client.PostAsJsonAsync($"/api/assistants/{lifeAssistant.Id}/appointments", request);
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        
        int numberOfAppointmentsInDb = await this.assertDbContext.Appointments.CountAsync();
        numberOfAppointmentsInDb.Should().Be(0);
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
        result.Count.Should().Be(2);
        result[0].Id.Should().Be(lifeAssistant.Appointments[0].Id);
        result[0].State.Should().Be(lifeAssistant.Appointments[0].State);
        result[0].LifeAssistantId.Should().Be(lifeAssistant.Id);
        result[0].DateTime.Date.Should().Be(lifeAssistant.Appointments[0].DateTime.Date);
        result[1].Id.Should().Be(lifeAssistant.Appointments[1].Id);
        result[1].State.Should().Be(lifeAssistant.Appointments[1].State);
        result[1].LifeAssistantId.Should().Be(lifeAssistant.Id);
        result[1].DateTime.Date.Should().Be(lifeAssistant.Appointments[1].DateTime.Date);
    }
    
    [Fact]
    public async Task SetAppointmentState_SetNewStateToAppointment()
    {
        // Given
        ApplicationUserEntity lifeAssistant = await this.dbDataFactory.InsertValidatedLifeAssistantWithAppointments();
        var appointmentId = lifeAssistant.Appointments[0].Id;

        await Login(lifeAssistant.UserName, this.dbDataFactory.UserPassword);

        // When
        var response = await this.client
            .PutAsJsonAsync(
                $"/api/assistants/{lifeAssistant.Id}/appointments/{appointmentId}",
                new SetAppointStateRequest("Pending Pickup")
            );
        
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetAppointmentResponse>();
        result.Id.Should().Be(appointmentId);
        result.State.Should().Be("Pending Pickup");
        result.LifeAssistantId.Should().Be(lifeAssistant.Id);
        result.DateTime.Date.Should().Be(lifeAssistant.Appointments[0].DateTime.Date);

        AppointmentEntity appointmentEntity =
            await this.assertDbContext.Appointments.FirstAsync(entity => entity.Id == appointmentId);
        appointmentEntity.State.Should().Be("Pending Pickup");
    }

    [Fact]
    public async Task GetLifeAssistantAppointments_ByState_ReturnsAppointmentsWithGivenState()
    {
        // Given
        ApplicationUserEntity lifeAssistant = await this.dbDataFactory.InsertValidatedLifeAssistantWithAppointments();
        await Login(lifeAssistant.UserName, this.dbDataFactory.UserPassword);
        
        // When
        var response = await this.client
            .GetAsync($"/api/assistants/{lifeAssistant.Id}/appointments?state=Planned");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<GetAppointmentResponse>>();
        result.Count.Should().Be(1);
    }
}