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
    private readonly WebApplicationFactory<Startup> factory;

    public IntegrationTests(WebApplicationFactory<Startup> factory)
    {
        this.factory = factory;
        context = factory.Services.GetService<ApplicationDbContext>();
    }
}