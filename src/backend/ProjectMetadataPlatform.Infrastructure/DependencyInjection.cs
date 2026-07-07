using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cerbos.Api.V1.Effect;
using Cerbos.Api.V1.Policy;
using Cerbos.Sdk;
using Cerbos.Sdk.Builder;
using Cerbos.Sdk.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Polly.Registry;
using ProjectMetadataPlatform.Application;
using ProjectMetadataPlatform.Application.Auth;
using ProjectMetadataPlatform.Application.Authorization;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Users;
using ProjectMetadataPlatform.Infrastructure.Auth;
using ProjectMetadataPlatform.Infrastructure.Authorization;
using ProjectMetadataPlatform.Infrastructure.BusinessUnits;
using ProjectMetadataPlatform.Infrastructure.Companies;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.DataAccess.Interceptors;
using ProjectMetadataPlatform.Infrastructure.Departments;
using ProjectMetadataPlatform.Infrastructure.Logs;
using ProjectMetadataPlatform.Infrastructure.OfficeLocations;
using ProjectMetadataPlatform.Infrastructure.Plugins;
using ProjectMetadataPlatform.Infrastructure.Projects;
using ProjectMetadataPlatform.Infrastructure.Teams;
using ProjectMetadataPlatform.Infrastructure.Users;
using static Cerbos.Api.V1.Policy.Match.Types;

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
        var cerbosUrl = Environment.GetEnvironmentVariable("PMP_CERBOS_URL");
        _ = serviceCollection.AddScoped<IAuthorizationTracker, AuthorizationTracker>();
        _ = serviceCollection.AddScoped<AuthorizationEnforcerInterceptor>();
        serviceCollection.AddDbContextWithPostgresConnection();
        _ = serviceCollection.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<ProjectMetadataPlatformDbContext>()
        );
        serviceCollection.ConfigureAuth(jwtBearerEvents);
        _ = serviceCollection.AddScoped<IPluginRepository, PluginRepository>();
        _ = serviceCollection.AddScoped<IProjectsRepository, ProjectsRepository>();
        _ = serviceCollection.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        _ = serviceCollection.AddScoped<IApiTokenRepository, ApiTokenRepository>();
        _ = serviceCollection.AddScoped<IUsersRepository, UsersRepository>();
        _ = serviceCollection.AddScoped<ITeamRepository, TeamRepository>();
        _ = serviceCollection.AddScoped<IBusinessUnitRepository, BusinessUnitRepository>();
        _ = serviceCollection.AddScoped<IDepartmentRepository, DepartmentRepository>();
        _ = serviceCollection.AddScoped<ICompanyRepository, CompanyRepository>();
        _ = serviceCollection.AddScoped<IOfficeLocationRepository, OfficeLocationRepository>();
        _ = serviceCollection.AddScoped<
            IPasswordHasher<ApplicationUser>,
            PasswordHasher<ApplicationUser>
        >();
        _ = serviceCollection.AddScoped<IPasswordHasher<ApiToken>, PasswordHasher<ApiToken>>();
        _ = serviceCollection.AddScoped<ILogRepository, LogRepository>();
        _ = serviceCollection.AddScoped<IAuthorizationService, AuthorizationService>();

        _ = serviceCollection.AddScoped(provider => AddCerbosClient(cerbosUrl ?? ""));
        _ = serviceCollection.AddScoped(provider => AddCerbosAdminClient(cerbosUrl ?? ""));
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

        _ = serviceCollection.AddDbContext<ProjectMetadataPlatformDbContext>(
            (sp, options) =>
            {
                var authInterceptor = sp.GetRequiredService<AuthorizationEnforcerInterceptor>();
                _ = options.UseNpgsql(connectionString).AddInterceptors(authInterceptor);
            }
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
        _ = serviceCollection.AddScoped<IUserStore<ApplicationUser>>(provider =>
        {
            var userStore = new UserStore<
                ApplicationUser,
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
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ProjectMetadataPlatformDbContext>()
            .AddDefaultTokenProviders();

        var tokenDescriptorInformation = TokenDescriptorInformation.ReadFromEnvVariables();
        _ = serviceCollection.Configure<IdentityOptions>(options =>
            options.User.RequireUniqueEmail = true
        );

        _ = serviceCollection
            .AddAuthentication(AuthenticationSchemes.SELECTOR)
            .AddPolicyScheme(
                AuthenticationSchemes.SELECTOR,
                AuthenticationSchemes.SELECTOR,
                options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        string? authorizationHeader = context.Request.Headers[
                            HeaderNames.Authorization
                        ];
                        if (
                            !string.IsNullOrEmpty(authorizationHeader)
                            && authorizationHeader.StartsWith("Bearer ")
                        )
                        {
                            var token = authorizationHeader.Replace("Bearer ", "");
                            var jwtHandler = new JwtSecurityTokenHandler();
                            if (!jwtHandler.CanReadToken(token))
                            {
                                return AuthenticationSchemes.API_TOKEN;
                            }
                            if (
                                jwtHandler.ReadJwtToken(token).Issuer
                                == tokenDescriptorInformation.ValidIssuer
                            )
                            {
                                return AuthenticationSchemes.BASIC;
                            }
                            if (
                                jwtHandler
                                    .ReadJwtToken(token)
                                    .Issuer.Contains(
                                        EnvironmentUtils.GetEnvVarOrLoadFromFile("AZURE_AUTHORITY")
                                    )
                            )
                            {
                                return AuthenticationSchemes.AZURE;
                            }
                        }
                        return AuthenticationSchemes.BASIC;
                    };
                }
            )
            .AddJwtBearer(
                AuthenticationSchemes.BASIC,
                options =>
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
                }
            )
            .AddScheme<AuthenticationSchemeOptions, ApiTokenAuthenticationHandler>(
                AuthenticationSchemes.API_TOKEN,
                options => { }
            )
            .AddMicrosoftIdentityWebApi(
                options => { },
                options =>
                {
                    options.Authority = EnvironmentUtils.GetEnvVarOrLoadFromFile("AZURE_AUTHORITY");
                    options.ClientId = EnvironmentUtils.GetEnvVarOrLoadFromFile(
                        "AZURE_BACKEND_CLIENT_ID"
                    );
                },
                AuthenticationSchemes.AZURE
            );
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

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (userManager.Users.Any())
        {
            return;
        }

        var user = new ApplicationUser
        {
            UserName = "admin@admin.admin",
            Email = "admin@admin.admin",
            EmployeeId = "0",
            IsActive = true,
            IsScimProvisioned = false,
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
        var tracker = scope.ServiceProvider.GetRequiredService<IAuthorizationTracker>();
        tracker.MarkAsChecked();
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

    /// <summary>
    /// Checks the Connection to the database context. Retries if no connection.
    /// </summary>
    public static async Task CheckConnection(this IServiceProvider serviceProvider)
    {
        using var serviceScope = serviceProvider.CreateScope();
        var services = serviceScope.ServiceProvider;
        var pipelineProvider = services.GetRequiredService<ResiliencePipelineProvider<string>>();
        var pipeline = pipelineProvider.GetPipeline("DbCheck-Pipeline");
        var dbContext = services.GetRequiredService<ProjectMetadataPlatformDbContext>();

        await pipeline.ExecuteAsync(async token =>
        {
            if (dbContext.Database.IsNpgsql() && !await dbContext.Database.CanConnectAsync(token))
                throw new ArgumentException("Can't Connect to DB");
        });
    }

    /// <summary>
    /// Adds the Cerbos Client.
    /// </summary>
    /// <param name="url">Service Url</param>
    /// <returns>The new Client</returns>
    public static ICerbosClient AddCerbosClient(string url)
    {
        return CerbosClientBuilder.ForTarget(url).WithPlaintext().Build();
    }

    /// <summary>
    /// Adds the Cerbos Admin Client.
    /// </summary>
    /// <param name="url">Service Url</param>
    /// <returns>The new Client</returns>
    public static ICerbosAdminClient AddCerbosAdminClient(string url)
    {
        var user = EnvironmentUtils.GetEnvVarOrLoadFromFile("PMP_CERBOS_USER");
        var password = EnvironmentUtils.GetEnvVarOrLoadFromFile("PMP_CERBOS_PASSWORD");
        return CerbosClientBuilder.ForTarget(url).WithPlaintext().BuildAdminClient(user, password);
    }

    /// <summary>
    /// Adds the Default Principal Policies for Users and Tokens.
    /// </summary>
    /// <returns></returns>
    public static async Task AddDefaultPolicies(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var adminClient = services.GetRequiredService<ICerbosAdminClient>();
        var basePolicy = new PrincipalPolicy()
        {
            Principal = AuthorizationConstants.PRINCIPLE_USER,
            Version = AuthorizationConstants.POLICY_VERSION,
            Rules =
            {
                new PrincipalRule
                {
                    Resource = "*",
                    Actions =
                    {
                        new PrincipalRule.Types.Action
                        {
                            Action_ = "*",
                            Effect = Effect.Allow,
                            Condition = new Condition
                            {
                                Match = new Match
                                {
                                    Any = new ExprList
                                    {
                                        Of =
                                        {
                                            new Match
                                            {
                                                Expr = "P.attr.Email == 'admin@admin.admin'",
                                            },
                                            new Match
                                            {
                                                Expr =
                                                    "P.attr.Departments.exists(d, d.DepartmentName in ['IT Admin', 'IT Development'])",
                                            },
                                        },
                                    },
                                },
                            },
                        },
                        new PrincipalRule.Types.Action
                        {
                            Action_ = "*",
                            Effect = Effect.Deny,
                            Condition = new Condition
                            {
                                Match = new Match
                                {
                                    Any = new ExprList
                                    {
                                        Of = { new Match { Expr = "P.attr.IsActive == false" } },
                                    },
                                },
                            },
                        },
                    },
                },
            },
        };
        var basePolicyRequest = AddOrUpdatePolicyRequest
            .NewInstance()
            .With(
                new Cerbos.Sdk.Policy.Policy(
                    new Policy
                    {
                        ApiVersion = AuthorizationConstants.API_VERSION,
                        PrincipalPolicy = basePolicy,
                    }
                )
            );
        _ = await adminClient.AddOrUpdatePolicyAsync(basePolicyRequest);

        var apiTokenPolicy = new PrincipalPolicy
        {
            Principal = AuthorizationConstants.PRINCIPLE_TOKEN,
            Version = AuthorizationConstants.POLICY_VERSION,
            Rules =
            {
                new PrincipalRule
                {
                    Resource = "*",
                    Actions = { CreateApiTokenActionConditions() },
                },
            },
        };
        var apiTokenPolicyRequest = AddOrUpdatePolicyRequest
            .NewInstance()
            .With(
                new Cerbos.Sdk.Policy.Policy(
                    new Policy
                    {
                        ApiVersion = AuthorizationConstants.API_VERSION,
                        PrincipalPolicy = apiTokenPolicy,
                    }
                )
            );
        _ = await adminClient.AddOrUpdatePolicyAsync(apiTokenPolicyRequest);

        scope.Dispose();
    }

    /// <summary>
    /// Creates Conditions for Actions for Api Tokens.
    /// </summary>
    /// <returns>List of Conditions per Action.</returns>
    private static Google.Protobuf.Collections.RepeatedField<PrincipalRule.Types.Action> CreateApiTokenActionConditions()
    {
        var actionsList =
            new Google.Protobuf.Collections.RepeatedField<PrincipalRule.Types.Action>();

        foreach (var action in Enum.GetValues<AuthorizationConstants.Actions>())
        {
            actionsList.Add(
                new PrincipalRule.Types.Action
                {
                    Action_ = action.ToString(),
                    Effect = Effect.Allow,
                    Condition = new Condition
                    {
                        Match = new Match
                        {
                            Any = new ExprList
                            {
                                Of =
                                {
                                    new Match
                                    {
                                        Expr =
                                            $"'{action}_' + R.kind.upperAscii() in P.attr.Scopes",
                                    },
                                    new Match
                                    {
                                        Expr =
                                            $"'SCIM' in P.attr.Scopes && R.kind == '{nameof(ApplicationUser)}'",
                                    },
                                },
                            },
                        },
                    },
                }
            );
            actionsList.Add(
                new PrincipalRule.Types.Action
                {
                    Action_ = action.ToString(),
                    Effect = Effect.Deny,
                    Condition = new Condition
                    {
                        Match = new Match
                        {
                            None = new ExprList
                            {
                                Of =
                                {
                                    new Match
                                    {
                                        Expr =
                                            $"'{action}_' + R.kind.upperAscii() in P.attr.Scopes",
                                    },
                                    new Match
                                    {
                                        Expr =
                                            $"'SCIM' in P.attr.Scopes && R.kind == '{nameof(ApplicationUser)}'",
                                    },
                                },
                            },
                        },
                    },
                }
            );
        }
        return actionsList;
    }
}
