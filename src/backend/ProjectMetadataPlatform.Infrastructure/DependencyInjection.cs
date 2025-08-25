using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProjectMetadataPlatform.Application;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Infrastructure.Auth;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Logs;
using ProjectMetadataPlatform.Infrastructure.Plugins;
using ProjectMetadataPlatform.Infrastructure.Projects;
using ProjectMetadataPlatform.Infrastructure.Teams;
using ProjectMetadataPlatform.Infrastructure.Users;

namespace ProjectMetadataPlatform.Infrastructure;

/// <summary>
/// Methods for dependency injection.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the necessary dependencies for the infrastructure layer.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="jwtBearerEvents">The events for the JWT bearer.</param>
    /// <returns>The service collection with the add dependencies.</returns>
    public static IServiceCollection AddInfrastructureDependencies(
        this IServiceCollection serviceCollection,
        JwtBearerEvents jwtBearerEvents
    )
    {
        serviceCollection.AddDbContextWithPostgresConnection();
        _ = serviceCollection.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<ProjectMetadataPlatformDbContext>()
        );
        serviceCollection.ConfigureAuth(jwtBearerEvents);
        _ = serviceCollection.AddScoped<IPluginRepository, PluginRepository>();
        _ = serviceCollection.AddScoped<IProjectsRepository, ProjectsRepository>();
        _ = serviceCollection.AddScoped<IAuthRepository, AuthRepository>();
        _ = serviceCollection.AddScoped<IUsersRepository, UsersRepository>();
        _ = serviceCollection.AddScoped<ITeamRepository, TeamRepository>();
        _ = serviceCollection.AddScoped<
            IPasswordHasher<IdentityUser>,
            PasswordHasher<IdentityUser>
        >();

        _ = serviceCollection.AddScoped<ILogRepository, LogRepository>();
        return serviceCollection;
    }

    private static void AddDbContextWithPostgresConnection(
        this IServiceCollection serviceCollection
    )
    {
        var url = Environment.GetEnvironmentVariable("PMP_DB_URL");
        var port = Environment.GetEnvironmentVariable("PMP_DB_PORT");
        var user = EnvironmentUtils.GetEnvVarOrLoadFromFile("PMP_DB_USER");
        var password = EnvironmentUtils.GetEnvVarOrLoadFromFile("PMP_DB_PASSWORD");
        var database = EnvironmentUtils.GetEnvVarOrLoadFromFile("PMP_DB_NAME");

        var connectionString =
            $"Host={url};Port={port};User Id={user};Password={password};Database={database}";

        _ = serviceCollection.AddDbContext<ProjectMetadataPlatformDbContext>(options =>
            options.UseNpgsql(connectionString)
        );
    }

    /// <summary>
    /// Configures the authentication for the project.
    /// </summary>
    private static void ConfigureAuth(
        this IServiceCollection serviceCollection,
        JwtBearerEvents jwtBearerEvents
    )
    {
        _ = serviceCollection.AddScoped<IUserStore<IdentityUser>>(provider =>
        {
            var userStore = new UserStore<
                IdentityUser,
                IdentityRole,
                ProjectMetadataPlatformDbContext,
                string
            >(provider.GetRequiredService<ProjectMetadataPlatformDbContext>())
            {
                AutoSaveChanges = false,
            };
            return userStore;
        });

        _ = serviceCollection
            .AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ProjectMetadataPlatformDbContext>()
            .AddDefaultTokenProviders();

        var tokenDescriptorInformation = TokenDescriptorInformation.ReadFromEnvVariables();
        _ = serviceCollection.Configure<IdentityOptions>(options =>
            options.User.RequireUniqueEmail = true
        );
        _ = serviceCollection
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = tokenDescriptorInformation.ValidIssuer,
                    ValidAudience = tokenDescriptorInformation.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(tokenDescriptorInformation.IssuerSigningKey)
                    ),
                    ClockSkew = Environment.GetEnvironmentVariable("PMP_JWT_CLOCK_SKEW_SECONDS")
                        is { } clockSkew
                        ? TimeSpan.FromSeconds(
                            double.Parse(clockSkew, CultureInfo.InvariantCulture)
                        )
                        : TimeSpan.FromMinutes(5),
                };

                options.Events = jwtBearerEvents;
            });
    }

    /// <summary>
    /// Adds the admin user to the database.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static void AddAdminUser(this IServiceProvider serviceProvider)
    {
        string password;
        try
        {
            password = EnvironmentUtils.GetEnvVarOrLoadFromFile("PMP_ADMIN_PASSWORD").Trim();
        }
        catch (InvalidOperationException)
        {
            password = "admin";
        }

        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        if (userManager.Users.Any())
        {
            return;
        }

        var user = new IdentityUser
        {
            UserName = "admin@admin.admin",
            Email = "admin@admin.admin",
            Id = "1",
        };
        user.PasswordHash = userManager.PasswordHasher.HashPassword(user, password);
        var identityResult = userManager.CreateAsync(user).Result;

        if (!identityResult.Succeeded)
        {
            throw new InvalidOperationException(
                "Could not create admin user: "
                    + string.Join(", ", identityResult.Errors.Select(e => e.Description))
            );
        }

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        _ = unitOfWork.CompleteAsync();
    }

    /// <summary>
    /// Migrates the database.
    /// </summary>
    public static void MigrateDatabase(this IServiceProvider serviceProvider)
    {
        if (
            Environment.GetEnvironmentVariable("PMP_MIGRATE_DB_ON_STARTUP")?.ToLowerInvariant()
            is "true"
        )
        {
            using var serviceScope = serviceProvider.CreateScope();
            var services = serviceScope.ServiceProvider;

            var dbContext = services.GetRequiredService<ProjectMetadataPlatformDbContext>();
            dbContext.Database.Migrate();
        }
    }
}
