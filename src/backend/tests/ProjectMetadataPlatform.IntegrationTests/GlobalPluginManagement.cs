using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class GlobalPluginManagement : IntegrationTestsBase
{
    private static readonly StringContent CreateRequest = StringContent(
        """{ "pluginName": "GitLab", "isArchived": false, "keys": [ "key1" ], "baseUrl": "https://gitlab.com" }"""
    );
    private static readonly StringContent CreateRequest2 = StringContent(
        """{ "pluginName": "Jira", "isArchived": false, "keys": [ "key2" ], "baseUrl": "https://jira.com" }"""
    );

    [Test]
    public async Task CreateMultiplePlugins()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var pluginId1 = (
            await ToJsonElement(client.PutAsync("/Plugins", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();
        var pluginId2 = (
            await ToJsonElement(client.PutAsync("/Plugins", CreateRequest2), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();

        var plugins = await ToJsonElement(client.GetAsync("/Plugins"));

        plugins.GetArrayLength().Should().Be(2);
        plugins[0].GetProperty("id").GetInt32().Should().Be(pluginId1);
        plugins[0].GetProperty("pluginName").GetString().Should().Be("GitLab");
        plugins[0].GetProperty("isArchived").GetBoolean().Should().BeFalse();
        plugins[0].GetProperty("keys").EnumerateArray().Should().BeEmpty();
        plugins[0].GetProperty("baseUrl").GetString().Should().Be("https://gitlab.com");
        plugins[1].GetProperty("id").GetInt32().Should().Be(pluginId2);
        plugins[1].GetProperty("pluginName").GetString().Should().Be("Jira");
        plugins[1].GetProperty("isArchived").GetBoolean().Should().BeFalse();
        plugins[1].GetProperty("keys").EnumerateArray().Should().BeEmpty();
        plugins[1].GetProperty("baseUrl").GetString().Should().Be("https://jira.com");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        logs.GetArrayLength().Should().Be(2);

        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new global plugin with properties: PluginName = GitLab, IsArchived = False, BaseUrl = https://gitlab.com, Keys[0] = key1"
            );
        logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new global plugin with properties: PluginName = Jira, IsArchived = False, BaseUrl = https://jira.com, Keys[0] = key2"
            );
    }

    [Test]
    public async Task UpdatePlugin()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var pluginId = (
            await ToJsonElement(client.PutAsync("/Plugins", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();

        var updatedPlugin = await ToJsonElement(
            client.PatchAsync($"/Plugins/{pluginId}", CreateRequest2)
        );

        updatedPlugin.GetProperty("id").GetInt32().Should().Be(pluginId);
        updatedPlugin.GetProperty("pluginName").GetString().Should().Be("Jira");
        updatedPlugin.GetProperty("isArchived").GetBoolean().Should().BeFalse();
        updatedPlugin.GetProperty("keys").EnumerateArray().Should().BeEmpty();

        var plugins = await ToJsonElement(client.GetAsync("/Plugins"));

        plugins.GetArrayLength().Should().Be(1);
        plugins[0].GetProperty("id").GetInt32().Should().Be(pluginId);
        plugins[0].GetProperty("pluginName").GetString().Should().Be("Jira");
        plugins[0].GetProperty("isArchived").GetBoolean().Should().BeFalse();
        plugins[0].GetProperty("keys").EnumerateArray().Should().BeEmpty();

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        logs.GetArrayLength().Should().Be(2);

        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new global plugin with properties: PluginName = GitLab, IsArchived = False, BaseUrl = https://gitlab.com, Keys[0] = key1"
            );

        logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin updated global plugin GitLab: set PluginName from GitLab to Jira, set BaseUrl from https://gitlab.com to https://jira.com"
            );
    }

    [Test]
    public async Task ArchivePlugin()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var pluginId = (
            await ToJsonElement(client.PutAsync("/Plugins", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();

        var updatedPlugin = await ToJsonElement(
            client.PatchAsync($"/Plugins/{pluginId}", StringContent("""{ "isArchived": true }"""))
        );

        updatedPlugin.GetProperty("id").GetInt32().Should().Be(pluginId);
        updatedPlugin.GetProperty("pluginName").GetString().Should().Be("GitLab");
        updatedPlugin.GetProperty("isArchived").GetBoolean().Should().BeTrue();
        updatedPlugin.GetProperty("keys").EnumerateArray().Should().BeEmpty();

        var plugins = await ToJsonElement(client.GetAsync("/Plugins"));

        plugins.GetArrayLength().Should().Be(1);
        plugins[0].GetProperty("id").GetInt32().Should().Be(pluginId);
        plugins[0].GetProperty("pluginName").GetString().Should().Be("GitLab");
        plugins[0].GetProperty("isArchived").GetBoolean().Should().BeTrue();
        plugins[0].GetProperty("keys").EnumerateArray().Should().BeEmpty();

        updatedPlugin = await ToJsonElement(
            client.PatchAsync($"/Plugins/{pluginId}", StringContent("""{ "isArchived": false }"""))
        );

        updatedPlugin.GetProperty("id").GetInt32().Should().Be(pluginId);
        updatedPlugin.GetProperty("pluginName").GetString().Should().Be("GitLab");
        updatedPlugin.GetProperty("isArchived").GetBoolean().Should().BeFalse();
        updatedPlugin.GetProperty("keys").EnumerateArray().Should().BeEmpty();

        plugins = await ToJsonElement(client.GetAsync("/Plugins"));

        plugins.GetArrayLength().Should().Be(1);
        plugins[0].GetProperty("id").GetInt32().Should().Be(pluginId);
        plugins[0].GetProperty("pluginName").GetString().Should().Be("GitLab");
        plugins[0].GetProperty("isArchived").GetBoolean().Should().BeFalse();
        plugins[0].GetProperty("keys").EnumerateArray().Should().BeEmpty();

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        logs.GetArrayLength().Should().Be(3);

        logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new global plugin with properties: PluginName = GitLab, IsArchived = False, BaseUrl = https://gitlab.com, Keys[0] = key1"
            );

        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin archived global plugin GitLab");

        logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin unarchived global plugin GitLab");
    }

    [Test]
    public async Task DeletePlugin()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var pluginId = (
            await ToJsonElement(client.PutAsync("/Plugins", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();

        var updatedPlugin = await ToJsonElement(
            client.PatchAsync($"/Plugins/{pluginId}", StringContent("""{ "isArchived": true }"""))
        );

        updatedPlugin.GetProperty("isArchived").GetBoolean().Should().BeTrue();

        var deleteResponse = await ToJsonElement(client.DeleteAsync($"/Plugins/{pluginId}"));
        deleteResponse.GetProperty("pluginId").GetInt32().Should().Be(pluginId);

        var plugins = await ToJsonElement(client.GetAsync("/Plugins"));

        plugins.GetArrayLength().Should().Be(0);

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        logs.GetArrayLength().Should().Be(3);

        logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new global plugin with properties: PluginName = GitLab, IsArchived = False, BaseUrl = https://gitlab.com, Keys[0] = key1"
            );

        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin archived global plugin GitLab");

        logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin removed global plugin GitLab");
    }

    [Test]
    public async Task PluginNameMustBeUnique()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        (await client.PutAsync("/Plugins", CreateRequest))
            .StatusCode.Should()
            .Be(HttpStatusCode.Created);
        var errorResponse = await ToErrorResponse(
            client.PutAsync("/Plugins", CreateRequest),
            HttpStatusCode.Conflict
        );

        // Assert
        errorResponse.Message.Should().Be("A global Plugin with the name GitLab already exists.");
    }

    [Test]
    public async Task NotFoundIsReturnedForInvalidPluginId([Values] bool patch)
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var responseTask = patch
            ? client.PatchAsync("/Plugins/1", CreateRequest)
            : client.DeleteAsync("/Plugins/1");

        // Act
        var errorResponse = await ToErrorResponse(responseTask, HttpStatusCode.NotFound);

        // Assert
        errorResponse.Message.Should().Be("The plugin with id 1 was not found.");
    }

    [Test]
    public async Task PluginMustBeArchivedToBeDeleted()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        var pluginId = (
            await ToJsonElement(client.PutAsync("/Plugins", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();
        var errorResponse = await ToErrorResponse(
            client.DeleteAsync($"/Plugins/{pluginId}"),
            HttpStatusCode.BadRequest
        );

        // Assert
        errorResponse.Message.Should().Be("The plugin 1 is not archived.");
    }
}
