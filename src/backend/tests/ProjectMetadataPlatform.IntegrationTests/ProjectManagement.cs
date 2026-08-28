using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
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
        Assert.Multiple(() =>
        {
            Assert.That(putResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(putResponse.Headers.Location, Is.Not.Null);
        });
        var getResponse = await client.GetAsync(putResponse.Headers.Location);
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

        var rootElement = getResponseContent!.RootElement;
        Assert.That(rootElement.GetProperty("projectName").GetString(), Is.EqualTo("testProject"));
        Assert.That(rootElement.GetProperty("clientName").GetString(), Is.EqualTo("testClient"));
        Assert.That(
            rootElement.GetProperty("company").GetProperty("companyName").GetString(),
            Is.EqualTo("testCompany")
        );
        Assert.That(rootElement.GetProperty("companyState").GetString(), Is.EqualTo("EXTERNAL"));
        Assert.That(rootElement.GetProperty("ismsLevel").GetString(), Is.EqualTo("NORMAL"));
        Assert.That(rootElement.GetProperty("id").GetInt32(), Is.GreaterThan(0));
        Assert.That(rootElement.GetProperty("notes").GetString(), Is.EqualTo("Example Notes"));
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
        Assert.That(putResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var companyId2 = await CreateCompany(client, "testCompany2");
        putResponse = await client.PutAsync("/Projects", CreateRequest2(companyId2));
        Assert.That(putResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var getResponse = await client.GetAsync("/Projects");

        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

        var rootElement = getResponseContent!.RootElement.GetProperty("resources");

        Assert.That(rootElement.GetArrayLength(), Is.EqualTo(2));
        var firstProject = rootElement[0];
        Assert.That(firstProject.GetProperty("projectName").GetString(), Is.EqualTo("testProject"));
        Assert.That(firstProject.GetProperty("clientName").GetString(), Is.EqualTo("testClient"));
        Assert.That(
            firstProject.GetProperty("company").GetProperty("companyName").GetString(),
            Is.EqualTo("testCompany")
        );
        Assert.That(firstProject.GetProperty("ismsLevel").GetString(), Is.EqualTo("NORMAL"));
        Assert.That(firstProject.GetProperty("id").GetInt32(), Is.GreaterThan(0));
        Assert.That(firstProject.GetProperty("notes").GetString(), Is.EqualTo("Example Notes"));
        var secondProject = rootElement[1];

        Assert.That(
            secondProject.GetProperty("projectName").GetString(),
            Is.EqualTo("otherTestProject2")
        );
        Assert.That(secondProject.GetProperty("clientName").GetString(), Is.EqualTo("testClient2"));

        Assert.That(
            secondProject.GetProperty("company").GetProperty("companyName").GetString(),
            Is.EqualTo("testCompany2")
        );
        Assert.That(secondProject.GetProperty("ismsLevel").GetString(), Is.EqualTo("VERY_HIGH"));
        Assert.That(secondProject.GetProperty("id").GetInt32(), Is.GreaterThan(0));
        Assert.That(secondProject.GetProperty("notes").GetString(), Is.EqualTo("Example Notes 2"));
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
        Assert.Multiple(() =>
        {
            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(updateResponse.Headers.Location, Is.Not.Null);
        });

        var getResponse = await client.GetAsync(updateResponse.Headers.Location);
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

        var rootElement = getResponseContent!.RootElement;

        Assert.That(rootElement.GetProperty("projectName").GetString(), Is.EqualTo("testProject"));
        Assert.That(rootElement.GetProperty("clientName").GetString(), Is.EqualTo("testClient2"));
        Assert.That(
            rootElement.GetProperty("company").GetProperty("companyName").GetString(),
            Is.EqualTo("testCompany2")
        );
        Assert.That(rootElement.GetProperty("companyState").GetString(), Is.EqualTo("INTERNAL"));
        Assert.That(rootElement.GetProperty("ismsLevel").GetString(), Is.EqualTo("HIGH"));
        Assert.That(rootElement.GetProperty("id").GetInt32(), Is.GreaterThan(0));
        Assert.That(rootElement.GetProperty("notes").GetString(), Is.EqualTo("testNotes2"));

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.That(logs.GetArrayLength(), Is.EqualTo(4));

        Assert.That(
            logs[2].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "admin created a new project with properties: ProjectName = testProject, Slug = testproject, ClientName = testClient, Company = testCompany, CompanyState = EXTERNAL, IsmsLevel = NORMAL, IsEoC = False, Notes = Example Notes"
            )
        );

        Assert.That(
            logs[0].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "admin updated project testProject:  set ClientName from testClient to testClient2,  set Company from testCompany to testCompany2,  set CompanyState from EXTERNAL to INTERNAL,  set IsmsLevel from NORMAL to HIGH,  set Notes from Example Notes to testNotes2"
            )
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
        var projects = (await ToJsonElement(client.GetAsync($"/Projects/"))).GetProperty(
            "resources"
        );
        var count = projects.GetArrayLength();
        _ = await client.PutAsync(
            $"/Projects?projectId=" + projectId,
            UpdateisArchivedRequest(companyId)
        );

        Assert.That(
            (await client.DeleteAsync($"/Projects/{projectId}")).StatusCode,
            Is.EqualTo(HttpStatusCode.NoContent)
        );

        var projects2 = (await ToJsonElement(client.GetAsync($"/Projects/"))).GetProperty(
            "resources"
        );
        Assert.That(projects2.GetArrayLength(), Is.EqualTo(count - 1));

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        Assert.That(logs.GetArrayLength(), Is.EqualTo(4));

        Assert.That(
            logs[2].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "admin created a new project with properties: ProjectName = testProject, Slug = testproject, ClientName = testClient, Company = testCompany, CompanyState = EXTERNAL, IsmsLevel = NORMAL, IsEoC = False, Notes = Example Notes"
            )
        );
        Assert.That(
            logs[1].GetProperty("logMessage").GetString(),
            Is.EqualTo("admin archived project testProject")
        );
        Assert.That(
            logs[0].GetProperty("logMessage").GetString(),
            Is.EqualTo("admin removed project testProject")
        );
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
        Assert.That(errorResponse.Message, Is.EqualTo("The Company with id 1 was not found."));
    }

    [Test]
    public async Task ProjectSlugMustBeUnique()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var companyId = await CreateCompany(client, "Test Company");
        // Act
        Assert.That(
            (await client.PutAsync("/Projects", CreateRequest(companyId))).StatusCode,
            Is.EqualTo(HttpStatusCode.Created)
        );
        var errorResponse = await ToErrorResponse(
            client.PutAsync("/Projects", CreateRequest(companyId)),
            HttpStatusCode.Conflict
        );

        // Assert
        Assert.That(
            errorResponse.Message,
            Is.EqualTo("A Project with this slug already exists: testproject")
        );
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
        Assert.That(errorResponse.Message, Is.EqualTo("The project with id 1 was not found."));
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
        Assert.That(
            errorResponse.Message,
            Is.EqualTo("The project with slug testproject was not found.")
        );
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
        Assert.That(errorResponse.Message, Is.EqualTo($"The project {projectId} is not archived."));
    }
}
