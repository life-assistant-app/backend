using System;
using System.Threading.Tasks;
using LifeAssistant.Web.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;

namespace LifeAssistant.Web.Tests.Database;

public class DatabaseTest : WebTests,IAsyncLifetime
{
    protected ApplicationDbContext context;
    protected DbDataFactory dbDataFactory;

    public DatabaseTest()
    {
        this.context = this.BuildNewDbContext();
        this.dbDataFactory = new DbDataFactory(this.context);
    }

    public virtual async Task InitializeAsync()
    {
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
        this.context.RemoveRange(this.context.Users);
        await this.context.SaveChangesAsync();
        await this.context.DisposeAsync();
    }
}