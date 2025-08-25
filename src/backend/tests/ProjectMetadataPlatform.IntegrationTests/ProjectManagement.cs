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
    private static readonly StringContent CreateRequest = StringContent(
        """
        {
          "projectName": "testProject",
          "clientName": "testClient",
          "offerId": "testId",
          "company": "testCompany",
          "companyState": "EXTERNAL",
          "ismsLevel": "NORMAL",
          "notes": "Example Notes"
        }
        """
    );

    private static readonly StringContent CreateRequest2 = StringContent(
        """
        {
          "projectName": "otherTestProject2",
          "clientName": "testClient2",
          "offerId": "testId2",
          "company": "testCompany2",
          "companyState": "EXTERNAL",
          "ismsLevel": "VERY_HIGH",
          "notes": "Example Notes 2"
        }
        """
    );

    private static readonly StringContent UpdateisArchivedRequest = StringContent(
        """
        {
          "projectName": "testProject",
          "clientName": "testClient",
          "isArchived": true,
          "offerId": "testId",
          "company": "testCompany",
          "companyState": "EXTERNAL",
          "ismsLevel": "NORMAL",
          "notes": "Example Notes"
        }
        """
    );

    private static readonly StringContent UpdateRequest = StringContent(
        """
        {
          "projectName": "testProject",
          "clientName": "testClient2",
          "offerId": "testId2",
          "company": "testCompany2",
          "companyState": "INTERNAL",
          "ismsLevel": "HIGH",
          "notes": "testNotes2"
        }
        """
    );

    private static StringContent RequestWithPlugins(int pluginId1, int pluginId2) =>
        StringContent(
            """
                   {
                     "projectName": "testProject",
                     "clientName": "testClient",
                     "offerId": "testId",
                     "company": "testCompany",
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

    private static StringContent RequestWithPlugins2(int pluginId1, int pluginId2) =>
        StringContent(
            """
                   {
                     "projectName": "testProject",
                     "clientName": "testClient",
                     "offerId": "testId",
                     "company": "testCompany",
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
        var putResponse = await client.PutAsync("/Projects", CreateRequest);

        putResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        putResponse.Headers.Location.Should().NotBeNull();

        var getResponse = await client.GetAsync(putResponse.Headers.Location);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

        var rootElement = getResponseContent!.RootElement;
        rootElement.GetProperty("projectName").GetString().Should().Be("testProject");
        rootElement.GetProperty("clientName").GetString().Should().Be("testClient");
        rootElement.GetProperty("offerId").GetString().Should().Be("testId");
        rootElement.GetProperty("company").GetString().Should().Be("testCompany");
        rootElement.GetProperty("companyState").GetString().Should().Be("EXTERNAL");
        rootElement.GetProperty("ismsLevel").GetString().Should().Be("NORMAL");
        rootElement.GetProperty("id").GetInt32().Should().BeGreaterThan(0);
        rootElement.GetProperty("notes").GetString().Should().Be("Example Notes");
    }

    [Test]
    public async Task CreateMultipleProjects()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var putResponse = await client.PutAsync("/Projects", CreateRequest);
        putResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        putResponse = await client.PutAsync("/Projects", CreateRequest2);
        putResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var getResponse = await client.GetAsync("/Projects");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

        var rootElement = getResponseContent!.RootElement;
        rootElement.GetArrayLength().Should().Be(2);

        var firstProject = rootElement[0];
        firstProject.GetProperty("projectName").GetString().Should().Be("testProject");
        firstProject.GetProperty("clientName").GetString().Should().Be("testClient");
        firstProject.GetProperty("company").GetString().Should().Be("testCompany");
        firstProject.GetProperty("ismsLevel").GetString().Should().Be("NORMAL");
        firstProject.GetProperty("notes").GetString().Should().Be("Example Notes");
        firstProject.GetProperty("id").GetInt32().Should().BeGreaterThan(0);

        var secondProject = rootElement[1];
        secondProject.GetProperty("projectName").GetString().Should().Be("otherTestProject2");
        secondProject.GetProperty("clientName").GetString().Should().Be("testClient2");
        secondProject.GetProperty("company").GetString().Should().Be("testCompany2");
        secondProject.GetProperty("ismsLevel").GetString().Should().Be("VERY_HIGH");
        secondProject.GetProperty("notes").GetString().Should().Be("Example Notes 2");
        secondProject.GetProperty("id").GetInt32().Should().BeGreaterThan(0);
    }

    [Test]
    public async Task UpdateProject()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var projectId = (
            await ToJsonElement(client.PutAsync("/Projects", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();

        var updateResponse = await client.PutAsync(
            $"/Projects?projectId=" + projectId,
            UpdateRequest
        );
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        updateResponse.Headers.Location.Should().NotBeNull();

        var getResponse = await client.GetAsync(updateResponse.Headers.Location);

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

        var rootElement = getResponseContent!.RootElement;
        rootElement.GetProperty("projectName").GetString().Should().Be("testProject");
        rootElement.GetProperty("clientName").GetString().Should().Be("testClient2");
        rootElement.GetProperty("offerId").GetString().Should().Be("testId2");
        rootElement.GetProperty("company").GetString().Should().Be("testCompany2");
        rootElement.GetProperty("companyState").GetString().Should().Be("INTERNAL");
        rootElement.GetProperty("ismsLevel").GetString().Should().Be("HIGH");
        rootElement.GetProperty("notes").GetString().Should().Be("testNotes2");
        rootElement.GetProperty("id").GetInt32().Should().BeGreaterThan(0);

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        logs.GetArrayLength().Should().Be(2);

        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin created a new project with properties: ProjectName = testProject, Slug = testproject, ClientName = testClient, OfferId = testId, Company = testCompany, CompanyState = EXTERNAL, IsmsLevel = NORMAL, Notes = Example Notes"
            );

        logs[0]
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

        // Act
        // Assert
        var projectId = (
            await ToJsonElement(
                client.PutAsync("/Projects", RequestWithPlugins(pluginId1, pluginId2)),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var projectPlugins = await ToJsonElement(client.GetAsync($"/Projects/{projectId}/Plugins"));

        projectPlugins.GetArrayLength().Should().Be(2);
        projectPlugins[0].GetProperty("url").GetString().Should().Be("www.appsfactory.gitlab.com");
        projectPlugins[0].GetProperty("displayName").GetString().Should().Be("GitLab");
        projectPlugins[0].GetProperty("pluginName").GetString().Should().Be("GitLab");
        projectPlugins[1].GetProperty("url").GetString().Should().Be("www.jira.com");
        projectPlugins[1].GetProperty("displayName").GetString().Should().Be("Jira");
        projectPlugins[1].GetProperty("pluginName").GetString().Should().Be("Jira");

        var updateResponse = await client.PutAsync(
            "/Projects?projectId=" + projectId,
            RequestWithPlugins2(pluginId1, pluginId3)
        );
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        updateResponse.Headers.Location.Should().NotBeNull();

        var project = await ToJsonElement(client.GetAsync(updateResponse.Headers.Location));

        project.GetProperty("projectName").GetString().Should().Be("testProject");
        project.GetProperty("clientName").GetString().Should().Be("testClient");
        project.GetProperty("offerId").GetString().Should().Be("testId");
        project.GetProperty("company").GetString().Should().Be("testCompany");
        project.GetProperty("companyState").GetString().Should().Be("EXTERNAL");
        project.GetProperty("ismsLevel").GetString().Should().Be("NORMAL");
        project.GetProperty("notes").GetString().Should().Be("testNotes");
        project.GetProperty("id").GetInt32().Should().BeGreaterThan(0);

        projectPlugins = await ToJsonElement(client.GetAsync($"/Projects/{projectId}/Plugins"));

        projectPlugins.GetArrayLength().Should().Be(2);
        projectPlugins[0].GetProperty("url").GetString().Should().Be("www.appsfactory.gitlab.com");
        projectPlugins[0].GetProperty("displayName").GetString().Should().Be("Appsfactory GitLab");
        projectPlugins[0].GetProperty("pluginName").GetString().Should().Be("GitLab");
        projectPlugins[1]
            .GetProperty("url")
            .GetString()
            .Should()
            .Be("www.appsfactory.confluence.com");
        projectPlugins[1].GetProperty("displayName").GetString().Should().Be("Confluence");
        projectPlugins[1].GetProperty("pluginName").GetString().Should().Be("Confluence");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        logs.GetArrayLength().Should().Be(9);

        logs[5]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin created a new project with properties: ProjectName = testProject, Slug = testproject, ClientName = testClient, OfferId = testId, Company = testCompany, CompanyState = EXTERNAL, IsmsLevel = NORMAL, Notes = testNotes"
            );

        logs[4]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new plugin to project testProject with properties: Url = www.appsfactory.gitlab.com, DisplayName = GitLab"
            );

        logs[3]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new plugin to project testProject with properties: Url = www.jira.com, DisplayName = Jira"
            );

        logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new plugin to project testProject with properties: Plugin = Confluence, DisplayName = Confluence, Url = www.appsfactory.confluence.com"
            );

        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin removed a plugin from project testProject with properties: Plugin = Jira, DisplayName = Jira, Url = www.jira.com"
            );

        logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin updated plugin properties in project testProject:  set DisplayName from GitLab to Appsfactory GitLab"
            );
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

        // Act
        // Assert
        var projectId = (
            await ToJsonElement(client.PutAsync("/Projects", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();
        var projects = await ToJsonElement(client.GetAsync($"/Projects/"));
        var count = projects.GetArrayLength();
        await client.PutAsync($"/Projects?projectId=" + projectId, UpdateisArchivedRequest);

        (await client.DeleteAsync($"/Projects/{projectId}"))
            .StatusCode.Should()
            .Be(HttpStatusCode.NoContent);

        var projects2 = await ToJsonElement(client.GetAsync($"/Projects/"));
        projects2.GetArrayLength().Should().Be(count - 1);

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        logs.GetArrayLength().Should().Be(3);

        logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin created a new project with properties: ProjectName = testProject, Slug = testproject, ClientName = testClient, OfferId = testId, Company = testCompany, CompanyState = EXTERNAL, IsmsLevel = NORMAL, Notes = Example Notes"
            );
        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin archived project testProject");
        logs[0]
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

        // Act
        var projectId = (
            await ToJsonElement(client.PutAsync("/Projects", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();

        var errorResponse = await ToErrorResponse(
            client.PutAsync($"/Projects?projectId=" + projectId, RequestWithPlugins(1, 2)),
            HttpStatusCode.NotFound
        );

        // Assert
        errorResponse.Message.Should().Be("The Plugins with these ids do not exist: 1, 2");
    }

    [Test]
    public async Task ProjectSlugMustBeUnique()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        (await client.PutAsync("/Projects", CreateRequest))
            .StatusCode.Should()
            .Be(HttpStatusCode.Created);
        var errorResponse = await ToErrorResponse(
            client.PutAsync("/Projects", CreateRequest),
            HttpStatusCode.Conflict
        );

        // Assert
        errorResponse.Message.Should().Be("A Project with this slug already exists: testproject");
    }

    [Test]
    public async Task NotFoundIsReturnedForInvalidProjectId(
        [Values("GET", "PUT", "DELETE", "PLUGINS", "UNARCHIVED_PLUGINS")] string endpoint
    )
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var responseTask = endpoint switch
        {
            "GET" => client.GetAsync("/Projects/1"),
            "PUT" => client.PutAsync("/Projects?projectId=1", CreateRequest),
            "DELETE" => client.DeleteAsync("/Projects/1"),
            "PLUGINS" => client.GetAsync("/Projects/1/Plugins"),
            "UNARCHIVED_PLUGINS" => client.GetAsync("/Projects/1/UnarchivedPlugins"),
            _ => throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, null),
        };

        // Act
        var errorResponse = await ToErrorResponse(responseTask, HttpStatusCode.NotFound);

        // Assert
        errorResponse.Message.Should().Be("The project with id 1 was not found.");
    }

    [Test]
    public async Task NotFoundIsReturnedForInvalidProjectSlug(
        [Values("GET", "PUT", "DELETE", "PLUGINS", "UNARCHIVED_PLUGINS")] string endpoint
    )
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var responseTask = endpoint switch
        {
            "GET" => client.GetAsync("/Projects/testproject"),
            "PUT" => client.PutAsync("/Projects/testproject", CreateRequest),
            "DELETE" => client.DeleteAsync("/Projects/testproject"),
            "PLUGINS" => client.GetAsync("/Projects/testproject/Plugins"),
            "UNARCHIVED_PLUGINS" => client.GetAsync("/Projects/testproject/UnarchivedPlugins"),
            _ => throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, null),
        };

        // Act
        var errorResponse = await ToErrorResponse(responseTask, HttpStatusCode.NotFound);

        // Assert
        errorResponse.Message.Should().Be("The project with slug testproject was not found.");
    }

    [Test]
    public async Task ProjectMustBeArchivedToBeDeleted()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        var projectId = (
            await ToJsonElement(client.PutAsync("/Projects", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();
        var errorResponse = await ToErrorResponse(
            client.DeleteAsync($"/Projects/{projectId}"),
            HttpStatusCode.BadRequest
        );

        // Assert
        errorResponse.Message.Should().Be($"The project {projectId} is not archived.");
    }
}
