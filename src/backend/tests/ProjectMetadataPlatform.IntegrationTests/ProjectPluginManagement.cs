using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Azure;
using Microsoft.Identity.Client.Extensibility;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class ProjectPluginManagement : IntegrationTestsBase
{
    private int _projectId = 0;
    private int _globalPluginId = 0;

    private static StringContent CreateRequest(int pluginId) =>
        StringContent(
            """{ "displayName": "GitLab", "url": "https://gitlab.com", "pluginId": """
                + pluginId.ToString()
                + """  }"""
        );

    private static StringContent CreateRequest2(int pluginId) =>
        StringContent(
            """{ "displayName": "Jira", "url": "https://jira.com", "pluginId": """
                + pluginId.ToString()
                + """  }"""
        );

    [SetUp]
    public async Task Setup()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        _projectId = await CreateProject(client, "Example Project");
        _globalPluginId = await CreateGlobalPlugin(client, "Example Global Plugin");
    }

    [Test]
    public async Task CreateMultipleProjectPluginObjects()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var pluginId1 = (
            await ToJsonElement(
                client.PutAsync($"/Projects/{_projectId}/plugins", CreateRequest(_globalPluginId)),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
        var pluginId2 = (
            await ToJsonElement(
                client.PutAsync($"/Projects/{_projectId}/plugins", CreateRequest2(_globalPluginId)),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var plugin = (
            await ToJsonElement(client.GetAsync($"/Projects/{_projectId}/plugins"))
        ).GetProperty("resources");

        Assert.Multiple(() =>
        {
            Assert.That(plugin.GetArrayLength(), Is.EqualTo(2));
            Assert.That(plugin[0].GetProperty("projectId").GetInt32(), Is.EqualTo(_projectId));
            Assert.That(plugin[0].GetProperty("id").GetInt32(), Is.EqualTo(pluginId1));
            Assert.That(
                plugin[0].GetProperty("pluginName").GetString(),
                Is.EqualTo("Example Global Plugin")
            );
            Assert.That(plugin[0].GetProperty("url").GetString(), Is.EqualTo("https://gitlab.com"));
            Assert.That(plugin[0].GetProperty("displayName").GetString(), Is.EqualTo("GitLab"));
            Assert.That(plugin[0].GetProperty("pluginId").GetInt32(), Is.EqualTo(_globalPluginId));

            Assert.That(plugin[1].GetProperty("projectId").GetInt32(), Is.EqualTo(_projectId));
            Assert.That(plugin[1].GetProperty("id").GetInt32(), Is.EqualTo(pluginId2));
            Assert.That(
                plugin[1].GetProperty("pluginName").GetString(),
                Is.EqualTo("Example Global Plugin")
            );
            Assert.That(plugin[1].GetProperty("url").GetString(), Is.EqualTo("https://jira.com"));
            Assert.That(plugin[1].GetProperty("displayName").GetString(), Is.EqualTo("Jira"));
            Assert.That(plugin[1].GetProperty("pluginId").GetInt32(), Is.EqualTo(_globalPluginId));
        });

        var unarchivedPlugin = (
            await ToJsonElement(client.GetAsync($"/Projects/{_projectId}/unarchivedPlugins"))
        ).GetProperty("resources");
        Assert.Multiple(() =>
        {
            Assert.That(unarchivedPlugin.GetArrayLength(), Is.EqualTo(2));
            Assert.That(
                unarchivedPlugin[0].GetProperty("projectId").GetInt32(),
                Is.EqualTo(_projectId)
            );
            Assert.That(unarchivedPlugin[0].GetProperty("id").GetInt32(), Is.EqualTo(pluginId1));
            Assert.That(
                unarchivedPlugin[0].GetProperty("pluginName").GetString(),
                Is.EqualTo("Example Global Plugin")
            );
            Assert.That(
                unarchivedPlugin[0].GetProperty("url").GetString(),
                Is.EqualTo("https://gitlab.com")
            );
            Assert.That(
                unarchivedPlugin[0].GetProperty("displayName").GetString(),
                Is.EqualTo("GitLab")
            );
            Assert.That(
                unarchivedPlugin[0].GetProperty("pluginId").GetInt32(),
                Is.EqualTo(_globalPluginId)
            );

            Assert.That(
                unarchivedPlugin[1].GetProperty("projectId").GetInt32(),
                Is.EqualTo(_projectId)
            );
            Assert.That(unarchivedPlugin[1].GetProperty("id").GetInt32(), Is.EqualTo(pluginId2));
            Assert.That(
                unarchivedPlugin[1].GetProperty("pluginName").GetString(),
                Is.EqualTo("Example Global Plugin")
            );
            Assert.That(
                unarchivedPlugin[1].GetProperty("url").GetString(),
                Is.EqualTo("https://jira.com")
            );
            Assert.That(
                unarchivedPlugin[1].GetProperty("displayName").GetString(),
                Is.EqualTo("Jira")
            );
            Assert.That(
                unarchivedPlugin[1].GetProperty("pluginId").GetInt32(),
                Is.EqualTo(_globalPluginId)
            );
        });

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        Assert.Multiple(() =>
        {
            Assert.That(logs.GetArrayLength(), Is.EqualTo(5));
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new plugin to project testProject with properties: Url = https://gitlab.com, DisplayName = GitLab"
                )
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new plugin to project testProject with properties: Url = https://jira.com, DisplayName = Jira"
                )
            );
        });
    }

    [Test]
    public async Task UpdateProjectPlugin()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var pluginId = (
            await ToJsonElement(
                client.PutAsync($"/Projects/{_projectId}/plugins", CreateRequest(_globalPluginId)),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var updatedProjectPlugin = await ToJsonElement(
            client.PatchAsync(
                $"/Projects/{_projectId}/plugins/{pluginId}",
                CreateRequest2(_globalPluginId)
            )
        );
        Assert.Multiple(() =>
        {
            Assert.That(updatedProjectPlugin.GetProperty("id").GetInt32(), Is.EqualTo(pluginId));
            Assert.That(
                updatedProjectPlugin.GetProperty("displayName").GetString(),
                Is.EqualTo("Jira")
            );
            Assert.That(
                updatedProjectPlugin.GetProperty("url").GetString(),
                Is.EqualTo("https://jira.com")
            );
        });

        var plugin = (
            await ToJsonElement(client.GetAsync($"/Projects/{_projectId}/plugins"))
        ).GetProperty("resources");
        Assert.Multiple(() =>
        {
            Assert.That(plugin[0].GetProperty("projectId").GetInt32(), Is.EqualTo(_projectId));
            Assert.That(plugin[0].GetProperty("id").GetInt32(), Is.EqualTo(pluginId));
            Assert.That(
                plugin[0].GetProperty("pluginName").GetString(),
                Is.EqualTo("Example Global Plugin")
            );
            Assert.That(plugin[0].GetProperty("url").GetString(), Is.EqualTo("https://jira.com"));
            Assert.That(plugin[0].GetProperty("displayName").GetString(), Is.EqualTo("Jira"));
            Assert.That(plugin[0].GetProperty("pluginId").GetInt32(), Is.EqualTo(_globalPluginId));
        });

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.Multiple(() =>
        {
            Assert.That(logs.GetArrayLength(), Is.EqualTo(5));
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new plugin to project testProject with properties: Url = https://gitlab.com, DisplayName = GitLab"
                )
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin updated plugin properties in project testProject:  set DisplayName from GitLab to Jira,  set Url from https://gitlab.com to https://jira.com"
                )
            );
        });
    }

    [Test]
    public async Task DeleteProjectPlugin()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var billingId = (
            await ToJsonElement(
                client.PutAsync($"/Projects/{_projectId}/plugins", CreateRequest(_globalPluginId)),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var response = await client.DeleteAsync($"/Projects/{_projectId}/plugins/{billingId}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        var plugin = (
            await ToJsonElement(client.GetAsync($"/Projects/{_projectId}/plugins"))
        ).GetProperty("resources");

        Assert.That(plugin.GetArrayLength(), Is.EqualTo(0));
        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.Multiple(() =>
        {
            Assert.That(logs.GetArrayLength(), Is.EqualTo(5));
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new plugin to project testProject with properties: Url = https://gitlab.com, DisplayName = GitLab"
                )
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin removed a plugin from project testProject with properties: Plugin = Example Global Plugin, DisplayName = GitLab, Url = https://gitlab.com"
                )
            );
        });
    }

    [Test]
    public async Task ProjectPluginKindMustBeUnique()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        var response = await client.PutAsync(
            $"/Projects/{_projectId}/plugins",
            CreateRequest(_globalPluginId)
        );
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var errorResponse = await ToErrorResponse(
            client.PutAsync($"/Projects/{_projectId}/plugins", CreateRequest(_globalPluginId)),
            HttpStatusCode.Conflict
        );

        // Assert
        Assert.That(
            errorResponse.Message,
            Is.EqualTo(
                $"A project Plugin with the url https://gitlab.com with a global plugin with the id {_globalPluginId} already exists on the project with the id {_projectId}."
            )
        );
    }

    [Test]
    public async Task NotFoundIsReturnedForInvalidProjectPluginId([Values] bool patch)
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var responseTask = patch
            ? client.PatchAsync($"/Projects/{_projectId}/plugins/1", CreateRequest(_globalPluginId))
            : client.DeleteAsync($"/Projects/{_projectId}/plugins/1");

        // Act
        var errorResponse = await ToErrorResponse(responseTask, HttpStatusCode.NotFound);

        // Assert
        Assert.That(
            errorResponse.Message,
            Does.StartWith("The plugin with id 1 was not found in the project with the id ")
        );
    }

    [Test]
    public async Task NotFoundIsReturnedForInvalidProjectId([Values] bool patch)
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var responseTask = patch
            ? client.PatchAsync($"/Projects/600/plugins/1", CreateRequest(_globalPluginId))
            : client.DeleteAsync($"/Projects/600/plugins/1");

        // Act
        var errorResponse = await ToErrorResponse(responseTask, HttpStatusCode.NotFound);

        // Assert
        Assert.That(errorResponse.Message, Is.EqualTo("The project with id 600 was not found."));
    }

    [Test]
    public async Task NotFoundIsReturnedForInvalidGlobalPluginId()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var responseTask = client.PutAsync($"/Projects/{_projectId}/plugins/", CreateRequest(999));

        // Act
        var errorResponse = await ToErrorResponse(responseTask, HttpStatusCode.NotFound);

        // Assert
        Assert.That(errorResponse.Message, Is.EqualTo("The plugin with id 999 was not found."));
    }

    private static async Task<int> CreateProject(HttpClient client, string name)
    {
        return (
            await ToJsonElement(
                client.PutAsync(
                    "/Projects",
                    StringContent(
                        """
                            {
                              "projectName": "testProject",
                              "clientName": "testClient",
                              "companyId":
                        """
                            + await CreateCompany(client, "Default")
                            + """
                                  ,
                                  "companyState": "EXTERNAL",
                                  "ismsLevel": "NORMAL",
                                  "isEoC": false,
                                  "notes": "Example Notes"
                                }
                            """
                    )
                ),
                HttpStatusCode.Created
            )
        ).GetProperty("id").GetInt32();
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

    private static async Task<int> CreateGlobalPlugin(HttpClient client, string name)
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
}
