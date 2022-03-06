using System;
using System.Threading.Tasks;
using LifeAssistant.Web.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;

namespace LifeAssistant.Web.Tests.Database;

public class DatabaseTest : IAsyncLifetime
{
    protected ApplicationDbContext context;

    public virtual async Task InitializeAsync()
    {
        this.context = this.BuildNewDbContext();
        await this.context.Database.MigrateAsync();
    }

    private ApplicationDbContext BuildNewDbContext()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Environment.GetEnvironmentVariable("DB_HOST"),
            Port = int.Parse(Environment.GetEnvironmentVariable("DB_PORT")),
            Username =  Environment.GetEnvironmentVariable("DB_USERNAME"),
            Password =  Environment.GetEnvironmentVariable("DB_PASSWORD"),
            Database =  Environment.GetEnvironmentVariable("DB_NAME")
        };
        var options = new DbContextOptionsBuilder()
            .UseNpgsql(builder.ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    public async Task DisposeAsync()
    {
        await this.context.Database.EnsureDeletedAsync();
        await context.DisposeAsync();
    }
}