using System;
using System.Net.Http;
using System.Threading.Tasks;
using LifeAssistant.Web.Database;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Xunit;

namespace LifeAssistant.Web.Tests.Integration;

public class IntegrationTests : WebTests,IClassFixture<WebApplicationFactory<Startup>>, IAsyncLifetime
{
    protected readonly HttpClient client;
    protected readonly ApplicationDbContext givenDbContext;
    protected readonly ApplicationDbContext assertDbContext;
    protected readonly DbDataFactory dbDataFactory;

    public IntegrationTests(WebApplicationFactory<Startup> factory)
    {
        this.client = factory.CreateClient();
        IConfiguration configuration = factory.Services.GetService<IConfiguration>() ?? throw new InvalidOperationException("Can't get Configuration from DI");

        givenDbContext = new ApplicationDbContext(GetOptionsForOtherDbContext(configuration));
        assertDbContext = new ApplicationDbContext(GetOptionsForOtherDbContext(configuration));

        this.dbDataFactory = new DbDataFactory(this.givenDbContext);
    }
    
    public DbContextOptions GetOptionsForOtherDbContext(IConfiguration configuration)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = configuration["DB_HOST"],
            Port = int.Parse(configuration["DB_PORT"]),
            Username = configuration["DB_USERNAME"],
            Password = configuration["DB_PASSWORD"],
            Database = configuration["DB_NAME"]
        };
        var options = new DbContextOptionsBuilder()
            .UseNpgsql(builder.ToString())
            .Options;
        return options;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        this.assertDbContext.RemoveRange(this.assertDbContext.Users);
        await this.assertDbContext.SaveChangesAsync();

        Task disposeAssertContext = this.assertDbContext.DisposeAsync().AsTask();
        Task disposeGivenContext = this.givenDbContext.DisposeAsync().AsTask();
        await Task.WhenAll(new[] {disposeAssertContext, disposeGivenContext});
    }
}