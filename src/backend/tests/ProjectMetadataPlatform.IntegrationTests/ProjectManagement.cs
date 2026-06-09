using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class ProjectManagement : IntegrationTestsBase
{
    private static StringContent CreateRequest(int companyId) =>
        StringContent(
            """
                {
                  "projectName": "testProject",
                  "clientName": "testClient",
                  "offerId": "testId",
                  "companyId":
            """
                + companyId
                + """
                      ,
                      "companyState": "EXTERNAL",
                      "ismsLevel": "NORMAL",
                      "isEoC": false,
                      "notes": "Example Notes"
                    }
                """
        );

    private static StringContent CreateRequest2(int companyId) =>
        StringContent(
            """
            {
              "projectName": "otherTestProject2",
              "clientName": "testClient2",
              "offerId": "testId2",
                  "companyId":
            """
                + companyId
                + """
                      ,
                  "companyState": "EXTERNAL",
                  "ismsLevel": "VERY_HIGH",
                  "notes": "Example Notes 2"
                }
                """
        );

    private static StringContent UpdateisArchivedRequest(int companyId) =>
        StringContent(
            """
            {
              "projectName": "testProject",
              "clientName": "testClient",
              "isArchived": true,
              "offerId": "testId",
                  "companyId":
            """
                + companyId
                + """
                      ,
                  "companyState": "EXTERNAL",
                  "ismsLevel": "NORMAL",
                  "notes": "Example Notes"
                }
                """
        );

    private static StringContent UpdateRequest(int companyId) =>
        StringContent(
            """
            {
              "projectName": "testProject",
              "clientName": "testClient2",
              "offerId": "testId2",
                  "companyId":
            """
                + companyId
                + """
                      ,
                  "companyState": "INTERNAL",
                  "ismsLevel": "HIGH",
                  "notes": "testNotes2"
                }
                """
        );

    private static StringContent RequestWithPlugins(int companyId, int pluginId1, int pluginId2) =>
        StringContent(
            """
                   {
                     "projectName": "testProject",
                     "clientName": "testClient",
                     "offerId": "testId",
                  "companyId":
            """
                + companyId
                + """
                      ,
                         "companyState": "EXTERNAL",
                         "ismsLevel": "NORMAL",
                         "notes": "testNotes",
                         "pluginList": [
                           {
                             "url": "www.appsfactory.gitlab.com",
                             "displayName": "GitLab",
                             "id":
                """
                + pluginId1
                + """
            },
            {
              "url": "www.jira.com",
              "displayName": "Jira",
              "id":
"""
                + pluginId2
                + """
                    }
                  ]
                }
                """
        );

    private static StringContent RequestWithPlugins2(int companyId, int pluginId1, int pluginId2) =>
        StringContent(
            """
                   {
                     "projectName": "testProject",
                     "clientName": "testClient",
                     "offerId": "testId",
                  "companyId":
            """
                + companyId
                + """
                      ,
                         "companyState": "EXTERNAL",
                         "ismsLevel": "NORMAL",
                         "notes": "testNotes",
                         "pluginList": [
                           {
                             "url": "www.appsfactory.gitlab.com",
                             "displayName": "Appsfactory GitLab",
                             "id":
                """
                + pluginId1
                + """
            },
            {
              "url": "www.appsfactory.confluence.com",
              "displayName": "Confluence",
              "id":
"""
                + pluginId2
                + """
                    }
                  ]
                }
                """
        );

    [Test]
    public async Task CreateProject()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var companyId = await CreateCompany(client, "testCompany");

        var putResponse = await client.PutAsync("/Projects", CreateRequest(companyId));

        _ = putResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        _ = putResponse.Headers.Location.Should().NotBeNull();

        var getResponse = await client.GetAsync(putResponse.Headers.Location);

        _ = getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

        var rootElement = getResponseContent!.RootElement;
        _ = rootElement.GetProperty("projectName").GetString().Should().Be("testProject");
        _ = rootElement.GetProperty("clientName").GetString().Should().Be("testClient");
        _ = rootElement.GetProperty("offerId").GetString().Should().Be("testId");
        _ = rootElement
            .GetProperty("company")
            .GetProperty("companyName")
            .GetString()
            .Should()
            .Be("testCompany");
        _ = rootElement.GetProperty("companyState").GetString().Should().Be("EXTERNAL");
        _ = rootElement.GetProperty("ismsLevel").GetString().Should().Be("NORMAL");
        _ = rootElement.GetProperty("id").GetInt32().Should().BeGreaterThan(0);
        _ = rootElement.GetProperty("notes").GetString().Should().Be("Example Notes");
    }

    [Test]
    public async Task CreateMultipleProjects()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var companyId = await CreateCompany(client, "testCompany");
        var putResponse = await client.PutAsync("/Projects", CreateRequest(companyId));
        _ = putResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var companyId2 = await CreateCompany(client, "testCompany2");
        putResponse = await client.PutAsync("/Projects", CreateRequest2(companyId2));
        _ = putResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getResponse = await client.GetAsync("/Projects");

        _ = getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

        var rootElement = getResponseContent!.RootElement;
        _ = rootElement.GetArrayLength().Should().Be(2);

        var firstProject = rootElement[0];
        _ = firstProject.GetProperty("projectName").GetString().Should().Be("testProject");
        _ = firstProject.GetProperty("clientName").GetString().Should().Be("testClient");
        _ = firstProject
            .GetProperty("company")
            .GetProperty("companyName")
            .GetString()
            .Should()
            .Be("testCompany");
        _ = firstProject.GetProperty("ismsLevel").GetString().Should().Be("NORMAL");
        _ = firstProject.GetProperty("notes").GetString().Should().Be("Example Notes");
        _ = firstProject.GetProperty("id").GetInt32().Should().BeGreaterThan(0);

        var secondProject = rootElement[1];
        _ = secondProject.GetProperty("projectName").GetString().Should().Be("otherTestProject2");
        _ = secondProject.GetProperty("clientName").GetString().Should().Be("testClient2");
        _ = secondProject
            .GetProperty("company")
            .GetProperty("companyName")
            .GetString()
            .Should()
            .Be("testCompany2");
        _ = secondProject.GetProperty("ismsLevel").GetString().Should().Be("VERY_HIGH");
        _ = secondProject.GetProperty("notes").GetString().Should().Be("Example Notes 2");
        _ = secondProject.GetProperty("id").GetInt32().Should().BeGreaterThan(0);
    }

    [Test]
    public async Task UpdateProject()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var companyId = await CreateCompany(client, "testCompany");
        // Act
        // Assert
        var projectId = (
            await ToJsonElement(
                client.PutAsync("/Projects", CreateRequest(companyId)),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
        var companyId2 = await CreateCompany(client, "testCompany2");
        var updateResponse = await client.PutAsync(
            $"/Projects?projectId=" + projectId,
            UpdateRequest(companyId2)
        );
        _ = updateResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        _ = updateResponse.Headers.Location.Should().NotBeNull();

        var getResponse = await client.GetAsync(updateResponse.Headers.Location);

        _ = getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

        var rootElement = getResponseContent!.RootElement;
        _ = rootElement.GetProperty("projectName").GetString().Should().Be("testProject");
        _ = rootElement.GetProperty("clientName").GetString().Should().Be("testClient2");
        _ = rootElement.GetProperty("offerId").GetString().Should().Be("testId2");
        _ = rootElement
            .GetProperty("company")
            .GetProperty("companyName")
            .GetString()
            .Should()
            .Be("testCompany2");
        _ = rootElement.GetProperty("companyState").GetString().Should().Be("INTERNAL");
        _ = rootElement.GetProperty("ismsLevel").GetString().Should().Be("HIGH");
        _ = rootElement.GetProperty("notes").GetString().Should().Be("testNotes2");
        _ = rootElement.GetProperty("id").GetInt32().Should().BeGreaterThan(0);

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        _ = logs.GetArrayLength().Should().Be(4);

        _ = logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin created a new project with properties: ProjectName = testProject, Slug = testproject, ClientName = testClient, OfferId = testId, Company = testCompany, CompanyState = EXTERNAL, IsmsLevel = NORMAL, IsEoC = False, Notes = Example Notes"
            );

        _ = logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin updated project testProject:  set ClientName from testClient to testClient2,  set OfferId from testId to testId2,  set Company from testCompany to testCompany2,  set CompanyState from EXTERNAL to INTERNAL,  set IsmsLevel from NORMAL to HIGH,  set Notes from Example Notes to testNotes2"
            );
    }

    [Test]
    public async Task ProjectWithPlugins()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var pluginId1 = await CreatePlugin(client, "GitLab");
        var pluginId2 = await CreatePlugin(client, "Jira");
        var pluginId3 = await CreatePlugin(client, "Confluence");
        var companyId = await CreateCompany(client, "testCompany");
        // Act
        // Assert
        var projectId = (
            await ToJsonElement(
                client.PutAsync("/Projects", RequestWithPlugins(companyId, pluginId1, pluginId2)),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var projectPlugins = await ToJsonElement(client.GetAsync($"/Projects/{projectId}/Plugins"));

        _ = projectPlugins.GetArrayLength().Should().Be(2);
        _ = projectPlugins[0]
            .GetProperty("url")
            .GetString()
            .Should()
            .Be("www.appsfactory.gitlab.com");
        _ = projectPlugins[0].GetProperty("displayName").GetString().Should().Be("GitLab");
        _ = projectPlugins[0].GetProperty("pluginName").GetString().Should().Be("GitLab");
        _ = projectPlugins[1].GetProperty("url").GetString().Should().Be("www.jira.com");
        _ = projectPlugins[1].GetProperty("displayName").GetString().Should().Be("Jira");
        _ = projectPlugins[1].GetProperty("pluginName").GetString().Should().Be("Jira");

        var updateResponse = await client.PutAsync(
            "/Projects?projectId=" + projectId,
            RequestWithPlugins2(companyId, pluginId1, pluginId3)
        );
        _ = updateResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        _ = updateResponse.Headers.Location.Should().NotBeNull();

        var project = await ToJsonElement(client.GetAsync(updateResponse.Headers.Location));

        _ = project.GetProperty("projectName").GetString().Should().Be("testProject");
        _ = project.GetProperty("clientName").GetString().Should().Be("testClient");
        _ = project.GetProperty("offerId").GetString().Should().Be("testId");
        _ = project
            .GetProperty("company")
            .GetProperty("companyName")
            .GetString()
            .Should()
            .Be("testCompany");
        _ = project.GetProperty("companyState").GetString().Should().Be("EXTERNAL");
        _ = project.GetProperty("ismsLevel").GetString().Should().Be("NORMAL");
        _ = project.GetProperty("notes").GetString().Should().Be("testNotes");
        _ = project.GetProperty("id").GetInt32().Should().BeGreaterThan(0);

        projectPlugins = await ToJsonElement(client.GetAsync($"/Projects/{projectId}/Plugins"));

        _ = projectPlugins.GetArrayLength().Should().Be(2);
        _ = projectPlugins[0]
            .GetProperty("url")
            .GetString()
            .Should()
            .Be("www.appsfactory.gitlab.com");
        _ = projectPlugins[0]
            .GetProperty("displayName")
            .GetString()
            .Should()
            .Be("Appsfactory GitLab");
        _ = projectPlugins[0].GetProperty("pluginName").GetString().Should().Be("GitLab");
        _ = projectPlugins[1]
            .GetProperty("url")
            .GetString()
            .Should()
            .Be("www.appsfactory.confluence.com");
        _ = projectPlugins[1].GetProperty("displayName").GetString().Should().Be("Confluence");
        _ = projectPlugins[1].GetProperty("pluginName").GetString().Should().Be("Confluence");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        _ = logs.GetArrayLength().Should().Be(10);

        _ = logs[5]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin created a new project with properties: ProjectName = testProject, Slug = testproject, ClientName = testClient, OfferId = testId, Company = testCompany, CompanyState = EXTERNAL, IsmsLevel = NORMAL, IsEoC = False, Notes = testNotes"
            );

        _ = logs[4]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new plugin to project testProject with properties: Url = www.appsfactory.gitlab.com, DisplayName = GitLab"
            );

        _ = logs[3]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new plugin to project testProject with properties: Url = www.jira.com, DisplayName = Jira"
            );

        _ = logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new plugin to project testProject with properties: Plugin = Confluence, DisplayName = Confluence, Url = www.appsfactory.confluence.com"
            );

        _ = logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin removed a plugin from project testProject with properties: Plugin = Jira, DisplayName = Jira, Url = www.jira.com"
            );

        _ = logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin updated plugin properties in project testProject:  set DisplayName from GitLab to Appsfactory GitLab"
            );
    }

    private static async Task<int> CreateCompany(HttpClient client, string name)
    {
        return (
            await ToJsonElement(
                client.PutAsync("/Companies", StringContent($"{{ \"companyName\": \"{name}\"}}")),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
    }

    private static async Task<int> CreatePlugin(HttpClient client, string name)
    {
        return (
            await ToJsonElement(
                client.PutAsync(
                    "/Plugins",
                    StringContent(
                        $"{{ \"baseUrl\": \"www.{name}.com\", \"isArchived\": false, \"keys\": [], \"pluginName\": \"{name}\"}}"
                    )
                ),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
    }

    [Test]
    public async Task DeleteProject()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var companyId = await CreateCompany(client, "testCompany");
        // Act
        // Assert
        var projectId = (
            await ToJsonElement(
                client.PutAsync("/Projects", CreateRequest(companyId)),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
        var projects = await ToJsonElement(client.GetAsync($"/Projects/"));
        var count = projects.GetArrayLength();
        _ = await client.PutAsync(
            $"/Projects?projectId=" + projectId,
            UpdateisArchivedRequest(companyId)
        );

        _ = (await client.DeleteAsync($"/Projects/{projectId}"))
            .StatusCode.Should()
            .Be(HttpStatusCode.NoContent);

        var projects2 = await ToJsonElement(client.GetAsync($"/Projects/"));
        _ = projects2.GetArrayLength().Should().Be(count - 1);

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        _ = logs.GetArrayLength().Should().Be(4);

        _ = logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin created a new project with properties: ProjectName = testProject, Slug = testproject, ClientName = testClient, OfferId = testId, Company = testCompany, CompanyState = EXTERNAL, IsmsLevel = NORMAL, IsEoC = False, Notes = Example Notes"
            );
        _ = logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin archived project testProject");
        _ = logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin removed project testProject");
    }

    [Test]
    public async Task GlobalPluginIdsMustExist()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var companyId = await CreateCompany(client, "Test Company");
        // Act
        var projectId = (
            await ToJsonElement(
                client.PutAsync("/Projects", CreateRequest(companyId)),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var errorResponse = await ToErrorResponse(
            client.PutAsync(
                $"/Projects?projectId=" + projectId,
                RequestWithPlugins(companyId, 1, 2)
            ),
            HttpStatusCode.NotFound
        );

        // Assert
        _ = errorResponse.Message.Should().Be("The Plugins with these ids do not exist: 1, 2");
    }

    [Test]
    public async Task CompanyMustExist()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var errorResponse = await ToErrorResponse(
            client.PutAsync($"/Projects", CreateRequest(1)),
            HttpStatusCode.NotFound
        );

        // Assert
        _ = errorResponse.Message.Should().Be("The Company with id 1 was not found.");
    }

    [Test]
    public async Task ProjectSlugMustBeUnique()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var companyId = await CreateCompany(client, "Test Company");
        // Act
        _ = (await client.PutAsync("/Projects", CreateRequest(companyId)))
            .StatusCode.Should()
            .Be(HttpStatusCode.Created);
        var errorResponse = await ToErrorResponse(
            client.PutAsync("/Projects", CreateRequest(companyId)),
            HttpStatusCode.Conflict
        );

        // Assert
        _ = errorResponse
            .Message.Should()
            .Be("A Project with this slug already exists: testproject");
    }

    [Test]
    public async Task NotFoundIsReturnedForInvalidProjectId(
        [Values("GET", "PUT", "DELETE", "PLUGINS", "UNARCHIVED_PLUGINS")] string endpoint
    )
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var companyId = await CreateCompany(client, "Test Company");
        var responseTask = endpoint switch
        {
            "GET" => client.GetAsync("/Projects/1"),
            "PUT" => client.PutAsync("/Projects?projectId=1", CreateRequest(companyId)),
            "DELETE" => client.DeleteAsync("/Projects/1"),
            "PLUGINS" => client.GetAsync("/Projects/1/Plugins"),
            "UNARCHIVED_PLUGINS" => client.GetAsync("/Projects/1/UnarchivedPlugins"),
            _ => throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, null),
        };

        // Act
        var errorResponse = await ToErrorResponse(responseTask, HttpStatusCode.NotFound);

        // Assert
        _ = errorResponse.Message.Should().Be("The project with id 1 was not found.");
    }

    [Test]
    public async Task NotFoundIsReturnedForInvalidProjectSlug(
        [Values("GET", "PUT", "DELETE", "PLUGINS", "UNARCHIVED_PLUGINS")] string endpoint
    )
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var companyId = await CreateCompany(client, "Test Company");
        var responseTask = endpoint switch
        {
            "GET" => client.GetAsync("/Projects/testproject"),
            "PUT" => client.PutAsync("/Projects/testproject", CreateRequest(companyId)),
            "DELETE" => client.DeleteAsync("/Projects/testproject"),
            "PLUGINS" => client.GetAsync("/Projects/testproject/Plugins"),
            "UNARCHIVED_PLUGINS" => client.GetAsync("/Projects/testproject/UnarchivedPlugins"),
            _ => throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, null),
        };

        // Act
        var errorResponse = await ToErrorResponse(responseTask, HttpStatusCode.NotFound);

        // Assert
        _ = errorResponse.Message.Should().Be("The project with slug testproject was not found.");
    }

    [Test]
    public async Task ProjectMustBeArchivedToBeDeleted()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var companyId = await CreateCompany(client, "Test Company");
        // Act
        var projectId = (
            await ToJsonElement(
                client.PutAsync("/Projects", CreateRequest(companyId)),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
        var errorResponse = await ToErrorResponse(
            client.DeleteAsync($"/Projects/{projectId}"),
            HttpStatusCode.BadRequest
        );

        // Assert
        _ = errorResponse.Message.Should().Be($"The project {projectId} is not archived.");
    }
}
