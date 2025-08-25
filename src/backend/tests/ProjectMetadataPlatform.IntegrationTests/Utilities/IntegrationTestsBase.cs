using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
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

    protected HttpClient CreateClient() => _factory.CreateClient();

    [OneTimeTearDown]
    public void CleanUp()
    {
        SqliteConnection.ClearAllPools();
        File.Delete("unittest-db.db");
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
        Environment.SetEnvironmentVariable("PMP_MIGRATE_DB_ON_STARTUP", "true");

        var platformDbContext =
            _factory.Services.GetRequiredService<ProjectMetadataPlatformDbContext>();
        var allEntitiesPlugins = platformDbContext.Plugins.ToList();
        var allEntitiesProjects = platformDbContext.Projects.ToList();
        var allEntitiesProjectsPlugins = platformDbContext.ProjectPluginsRelation.ToList();
        var allEntitiesLogs = platformDbContext.Logs.ToList();
        var allEntitiesRefreshTokens = platformDbContext.Set<RefreshToken>().ToList();
        var allEntitiesUsers = platformDbContext
            .Users.Where(user => user.Email != "admin@admin.admin")
            .ToList();
        platformDbContext.Plugins.RemoveRange(allEntitiesPlugins);
        platformDbContext.Projects.RemoveRange(allEntitiesProjects);
        platformDbContext.ProjectPluginsRelation.RemoveRange(allEntitiesProjectsPlugins);
        platformDbContext.Logs.RemoveRange(allEntitiesLogs);
        platformDbContext.Set<RefreshToken>().RemoveRange(allEntitiesRefreshTokens);
        platformDbContext.Users.RemoveRange(allEntitiesUsers);

        await platformDbContext.SaveChangesAsync();
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
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {content!.AccessToken}");
    }

    protected static async Task<JsonElement> ToJsonElement(
        Task<HttpResponseMessage> response,
        HttpStatusCode expectedStatusCode = HttpStatusCode.OK
    )
    {
        var responseMessage = await response;
        responseMessage.StatusCode.Should().Be(expectedStatusCode);
        return (await responseMessage.Content.ReadFromJsonAsync<JsonDocument>())!.RootElement;
    }

    protected static async Task<ErrorResponse> ToErrorResponse(
        Task<HttpResponseMessage> response,
        HttpStatusCode expectedStatusCode
    )
    {
        var responseMessage = await response;
        responseMessage.StatusCode.Should().Be(expectedStatusCode);
        return (await responseMessage.Content.ReadFromJsonAsync<ErrorResponse>())!;
    }

    protected static StringContent StringContent(string content) =>
        new(content, System.Text.Encoding.UTF8, "application/json");
}
