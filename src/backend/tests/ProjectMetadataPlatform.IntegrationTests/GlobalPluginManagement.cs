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

        _ = plugins.GetArrayLength().Should().Be(2);
        _ = plugins[0].GetProperty("id").GetInt32().Should().Be(pluginId1);
        _ = plugins[0].GetProperty("pluginName").GetString().Should().Be("GitLab");
        _ = plugins[0].GetProperty("isArchived").GetBoolean().Should().BeFalse();
        _ = plugins[0].GetProperty("keys").EnumerateArray().Should().BeEmpty();
        _ = plugins[0].GetProperty("baseUrl").GetString().Should().Be("https://gitlab.com");
        _ = plugins[1].GetProperty("id").GetInt32().Should().Be(pluginId2);
        _ = plugins[1].GetProperty("pluginName").GetString().Should().Be("Jira");
        _ = plugins[1].GetProperty("isArchived").GetBoolean().Should().BeFalse();
        _ = plugins[1].GetProperty("keys").EnumerateArray().Should().BeEmpty();
        _ = plugins[1].GetProperty("baseUrl").GetString().Should().Be("https://jira.com");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        _ = logs.GetArrayLength().Should().Be(2);

        _ = logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new global plugin with properties: PluginName = GitLab, IsArchived = False, BaseUrl = https://gitlab.com, Keys[0] = key1"
            );
        _ = logs[0]
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

        _ = updatedPlugin.GetProperty("id").GetInt32().Should().Be(pluginId);
        _ = updatedPlugin.GetProperty("pluginName").GetString().Should().Be("Jira");
        _ = updatedPlugin.GetProperty("isArchived").GetBoolean().Should().BeFalse();
        _ = updatedPlugin.GetProperty("keys").EnumerateArray().Should().BeEmpty();

        var plugins = await ToJsonElement(client.GetAsync("/Plugins"));

        _ = plugins.GetArrayLength().Should().Be(1);
        _ = plugins[0].GetProperty("id").GetInt32().Should().Be(pluginId);
        _ = plugins[0].GetProperty("pluginName").GetString().Should().Be("Jira");
        _ = plugins[0].GetProperty("isArchived").GetBoolean().Should().BeFalse();
        _ = plugins[0].GetProperty("keys").EnumerateArray().Should().BeEmpty();

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        _ = logs.GetArrayLength().Should().Be(2);

        _ = logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new global plugin with properties: PluginName = GitLab, IsArchived = False, BaseUrl = https://gitlab.com, Keys[0] = key1"
            );

        _ = logs[0]
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

        _ = updatedPlugin.GetProperty("id").GetInt32().Should().Be(pluginId);
        _ = updatedPlugin.GetProperty("pluginName").GetString().Should().Be("GitLab");
        _ = updatedPlugin.GetProperty("isArchived").GetBoolean().Should().BeTrue();
        _ = updatedPlugin.GetProperty("keys").EnumerateArray().Should().BeEmpty();

        var plugins = await ToJsonElement(client.GetAsync("/Plugins"));

        _ = plugins.GetArrayLength().Should().Be(1);
        _ = plugins[0].GetProperty("id").GetInt32().Should().Be(pluginId);
        _ = plugins[0].GetProperty("pluginName").GetString().Should().Be("GitLab");
        _ = plugins[0].GetProperty("isArchived").GetBoolean().Should().BeTrue();
        _ = plugins[0].GetProperty("keys").EnumerateArray().Should().BeEmpty();

        updatedPlugin = await ToJsonElement(
            client.PatchAsync($"/Plugins/{pluginId}", StringContent("""{ "isArchived": false }"""))
        );

        _ = updatedPlugin.GetProperty("id").GetInt32().Should().Be(pluginId);
        _ = updatedPlugin.GetProperty("pluginName").GetString().Should().Be("GitLab");
        _ = updatedPlugin.GetProperty("isArchived").GetBoolean().Should().BeFalse();
        _ = updatedPlugin.GetProperty("keys").EnumerateArray().Should().BeEmpty();

        plugins = await ToJsonElement(client.GetAsync("/Plugins"));

        _ = plugins.GetArrayLength().Should().Be(1);
        _ = plugins[0].GetProperty("id").GetInt32().Should().Be(pluginId);
        _ = plugins[0].GetProperty("pluginName").GetString().Should().Be("GitLab");
        _ = plugins[0].GetProperty("isArchived").GetBoolean().Should().BeFalse();
        _ = plugins[0].GetProperty("keys").EnumerateArray().Should().BeEmpty();

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        _ = logs.GetArrayLength().Should().Be(3);

        _ = logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new global plugin with properties: PluginName = GitLab, IsArchived = False, BaseUrl = https://gitlab.com, Keys[0] = key1"
            );

        _ = logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin archived global plugin GitLab");

        _ = logs[0]
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

        _ = updatedPlugin.GetProperty("isArchived").GetBoolean().Should().BeTrue();

        var deleteResponse = await ToJsonElement(client.DeleteAsync($"/Plugins/{pluginId}"));
        _ = deleteResponse.GetProperty("pluginId").GetInt32().Should().Be(pluginId);

        var plugins = await ToJsonElement(client.GetAsync("/Plugins"));

        _ = plugins.GetArrayLength().Should().Be(0);

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        _ = logs.GetArrayLength().Should().Be(3);

        _ = logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new global plugin with properties: PluginName = GitLab, IsArchived = False, BaseUrl = https://gitlab.com, Keys[0] = key1"
            );

        _ = logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin archived global plugin GitLab");

        _ = logs[0]
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
        _ = (await client.PutAsync("/Plugins", CreateRequest))
            .StatusCode.Should()
            .Be(HttpStatusCode.Created);
        var errorResponse = await ToErrorResponse(
            client.PutAsync("/Plugins", CreateRequest),
            HttpStatusCode.Conflict
        );

        // Assert
        _ = errorResponse
            .Message.Should()
            .Be("A global Plugin with the name GitLab already exists.");
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
        _ = errorResponse.Message.Should().Be("The plugin with id 1 was not found.");
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
        _ = errorResponse.Message.Should().Be("The plugin 1 is not archived.");
    }
}
