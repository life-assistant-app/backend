using System.Reflection;
using System.Text;
using LifeAssistant.Core.Application.Appointments;
using LifeAssistant.Core.Application.Users;
using LifeAssistant.Core.Domain.Entities;
using LifeAssistant.Core.Domain.Entities.AppointmentState;
using LifeAssistant.Core.Domain.Rules;
using LifeAssistant.Core.Persistence;
using LifeAssistant.Web.Database;
using LifeAssistant.Web.Database.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace LifeAssistant.Web;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureDbContext(services);
        ConfigureAuth(services);
        ConfigureDi(services);

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(ConfigureSwagger);
    }

    private void ConfigureDi(IServiceCollection services)
    {
        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
        services.AddScoped((servicesProviders) =>
            new UsersApplication(servicesProviders.GetService<IApplicationUserRepository>(),
                Configuration["JWT_SECRET"]));
        services.AddScoped<AppointmentsApplication>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddSingleton<IAppointmentStateFactory, AppointmentStateFactory>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped(servicesProviders =>
        {
            var httpContext = servicesProviders
                .GetService<IHttpContextAccessor>()
                .HttpContext;

            Guid currentUserId = Guid.Parse(httpContext.User.Claims.First().Value);

            return new AccessControlManager(currentUserId, servicesProviders.GetService<IApplicationUserRepository>());
        });
    }

    private void ConfigureDbContext(IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
            new NpgsqlConnectionStringBuilder
            {
                Host = Configuration["DB_HOST"],
                Port = int.Parse(Configuration["DB_PORT"]),
                Username = Configuration["DB_USERNAME"],
                Password = Configuration["DB_PASSWORD"],
                Database = Configuration["DB_NAME"]
            }.ToString()));
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
    {
        context.Database.Migrate();
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });

        app.UseRouting();
        app.UseCors(cors =>
        {
            cors.AllowAnyOrigin();
            cors.AllowAnyMethod();
            cors.AllowAnyHeader();
        });

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }


    private void ConfigureSwagger(SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Life Assistant API API",
            Description = "An ASP.NET Core Web API backend for the Life Assistant App",
            Contact = new OpenApiContact
            {
                Name = "Arsène Lapostolet",
                Url = new Uri("https://arsenelapostolet.fr")
            },
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("https://github.com/life-assistant-app/backend/blob/main/LICENSE")
            }
        });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Description = "JWT Bearer Authorization",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    }

    private void ConfigureAuth(IServiceCollection services)
    {
        var jwtSecret = Configuration["JWT_SECRET"] ??
                        throw new ArgumentException("JWT_SECRET Env variable is not set");
        var key = Encoding.ASCII.GetBytes(jwtSecret);

        services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(auth =>
            {
                auth.SaveToken = true;
                auth.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
    }
}