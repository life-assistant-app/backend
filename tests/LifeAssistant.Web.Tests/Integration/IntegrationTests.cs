using System;
using System.Net.Http;
using LifeAssistant.Web.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LifeAssistant.Web.Tests.Integration;

public class IntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
{
    protected readonly HttpClient client;
    protected readonly ApplicationDbContext context;

    public IntegrationTests(WebApplicationFactory<Startup> factory)
    {
        this.client = factory.CreateClient();
        context = factory.Services.GetService<ApplicationDbContext>() ?? throw new InvalidOperationException("Could not get DbContext from DI");
    }
}