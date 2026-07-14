using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Auth.Models;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.IntegrationTests.Utilities;

public class IntegrationTestsBase : IDisposable
{
    private readonly PmpWebApplicationFactory _factory = new();
    private IContainer? _cerbosContainer;

    protected HttpClient CreateClient() => _factory.CreateClient();

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Environment.SetEnvironmentVariable("TESTCONTAINERS_RYUK_DISABLED", "true");
        _cerbosContainer = new ContainerBuilder("ghcr.io/cerbos/cerbos:latest")
            .WithPortBinding(3592, true)
            .WithPortBinding(3593, true)
            .WithCommand(
                "server",
                "--set=storage.driver=sqlite3",
                "--set=storage.sqlite3.dsn=:memory:",
                "--set=server.adminAPI.enabled=true",
                "--set=server.adminAPI.adminCredentials.username=cerbos_user",
                "--set=server.adminAPI.adminCredentials.passwordHash=JDJ5JDEwJHl2ZjJJRERFS3VES2FTcExZU2xacmU5a1lLZHA0Z2FnL2c4VU8yaTguZnpacUo3emozSk4yCgo="
            )
            .Build();

        await _cerbosContainer.StartAsync();
        var cerbosHttpPort = _cerbosContainer.GetMappedPublicPort(3593);
        Environment.SetEnvironmentVariable("PMP_CERBOS_URL", $"http://localhost:{cerbosHttpPort}");
        Environment.SetEnvironmentVariable("PMP_CERBOS_USER", "cerbos_user");
        Environment.SetEnvironmentVariable("PMP_CERBOS_PASSWORD", "changeme");
    }

    [OneTimeTearDown]
    public async Task CleanUpAsync()
    {
        SqliteConnection.ClearAllPools();
        File.Delete("unittest-db.db");
        if (_cerbosContainer is not null)
        {
            await _cerbosContainer.DisposeAsync();
        }
    }

    [SetUp]
    public async Task BaseSetup()
    {
        Environment.SetEnvironmentVariable("PMP_DB_URL", " ");
        Environment.SetEnvironmentVariable("PMP_DB_PORT", " ");
        Environment.SetEnvironmentVariable("PMP_DB_USER", " ");
        Environment.SetEnvironmentVariable("PMP_DB_PASSWORD", " ");
        Environment.SetEnvironmentVariable("PMP_DB_NAME", " ");
        Environment.SetEnvironmentVariable("JWT_VALID_ISSUER", "validIssue");
        Environment.SetEnvironmentVariable("JWT_VALID_AUDIENCE", "validAudience");
        Environment.SetEnvironmentVariable(
            "JWT_ISSUER_SIGNING_KEY",
            "superSecretKeyThatIsAtLeast257BitLong"
        );
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS", "6");
        Environment.SetEnvironmentVariable("ACCESS_TOKEN_EXPIRATION_MINUTES", "15");
        Environment.SetEnvironmentVariable("PMP_JWT_CLOCK_SKEW_SECONDS", "0");
        Environment.SetEnvironmentVariable("PMP_MIGRATE_DB_ON_STARTUP", "true");
        Environment.SetEnvironmentVariable("AZURE_AUTHORITY", "https://placeholder.placeholder");
        Environment.SetEnvironmentVariable("AZURE_BACKEND_CLIENT_ID", "Placeholder");
        Environment.SetEnvironmentVariable("AZURE_FRONTEND_CLIENT_ID", "Placeholder");
        Environment.SetEnvironmentVariable("AZURE_SCOPE", "Placeholder");

        var platformDbContext =
            _factory.Services.GetRequiredService<ProjectMetadataPlatformDbContext>();
        var allEntitiesPlugins = platformDbContext.Plugins.ToList();
        var allEntitiesProjects = platformDbContext.Projects.ToList();
        var allEntitiesProjectsPlugins = platformDbContext.ProjectPluginsRelation.ToList();
        var allEntitiesLogs = platformDbContext.Logs.ToList();
        var allEntitiesRefreshTokens = platformDbContext.Set<RefreshToken>().ToList();
        var allEntitiesApiTokens = platformDbContext.ApiTokens.ToList();
        var allEntitiesTeams = platformDbContext.Teams.ToList();
        var allEntitiesUsers = platformDbContext
            .Users.Where(user => user.Email != "admin@admin.admin")
            .ToList();
        var allEntitiesCompanies = platformDbContext.Companies.ToList();
        var allEntitiesDepartments = platformDbContext.Departments.ToList();
        var allEntitiesBusinessUnits = platformDbContext.BusinessUnits.ToList();
        var allEntitiesOfficeLocations = platformDbContext.OfficeLocations.ToList();

        platformDbContext.Plugins.RemoveRange(allEntitiesPlugins);
        platformDbContext.Projects.RemoveRange(allEntitiesProjects);
        platformDbContext.ProjectPluginsRelation.RemoveRange(allEntitiesProjectsPlugins);
        platformDbContext.Logs.RemoveRange(allEntitiesLogs);
        platformDbContext.Set<RefreshToken>().RemoveRange(allEntitiesRefreshTokens);
        platformDbContext.ApiTokens.RemoveRange(allEntitiesApiTokens);
        platformDbContext.Teams.RemoveRange(allEntitiesTeams);
        platformDbContext.Users.RemoveRange(allEntitiesUsers);
        platformDbContext.Companies.RemoveRange(allEntitiesCompanies);
        platformDbContext.Departments.RemoveRange(allEntitiesDepartments);
        platformDbContext.BusinessUnits.RemoveRange(allEntitiesBusinessUnits);
        platformDbContext.OfficeLocations.RemoveRange(allEntitiesOfficeLocations);

        _ = await platformDbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }

    protected static async Task GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(
        HttpClient client,
        string email = "admin@admin.admin",
        string password = "admin"
    )
    {
        var response = await client.PostAsJsonAsync(
            "/auth/basic",
            new { Email = email, Password = password }
        );
        _ = response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {content!.AccessToken}");
    }

    protected static async Task CreateApiTokenAndAddItToDefaultRequestHeadersOfClient(
        HttpClient client,
        string name = "ApiToken",
        List<TokenScopes>? scopes = null
    )
    {
        var response = await client.PostAsJsonAsync(
            "/auth/ApiTokens",
            new { Name = name, Scopes = scopes ?? [] }
        );
        _ = response.EnsureSuccessStatusCode();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        options.Converters.Add(new JsonStringEnumConverter());
        var content = await response.Content.ReadFromJsonAsync<GetApiTokenDetailsResponse>(options);

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {content!.Token}");
    }

    protected static async Task<JsonElement> ToJsonElement(
        Task<HttpResponseMessage> response,
        HttpStatusCode expectedStatusCode = HttpStatusCode.OK
    )
    {
        var responseMessage = await response;
        Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedStatusCode));
        return (await responseMessage.Content.ReadFromJsonAsync<JsonDocument>())!.RootElement;
    }

    protected static async Task<ErrorResponse> ToErrorResponse(
        Task<HttpResponseMessage> response,
        HttpStatusCode expectedStatusCode
    )
    {
        var responseMessage = await response;
        Assert.That(responseMessage.StatusCode, Is.EqualTo(expectedStatusCode));
        return (await responseMessage.Content.ReadFromJsonAsync<ErrorResponse>())!;
    }

    protected static StringContent StringContent(string content) =>
        new(content, System.Text.Encoding.UTF8, "application/json");
}
