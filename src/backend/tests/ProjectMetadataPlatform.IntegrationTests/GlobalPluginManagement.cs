using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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

        var plugins = (await ToJsonElement(client.GetAsync("/Plugins"))).GetProperty("resources");
        Assert.Multiple(() =>
        {
            Assert.That(plugins.GetArrayLength(), Is.EqualTo(2));
            Assert.That(plugins[0].GetProperty("id").GetInt32(), Is.EqualTo(pluginId1));
            Assert.That(plugins[0].GetProperty("pluginName").GetString(), Is.EqualTo("GitLab"));
            Assert.That(plugins[0].GetProperty("isArchived").GetBoolean(), Is.False);
            Assert.That(plugins[0].GetProperty("keys").EnumerateArray(), Is.Empty);
            Assert.That(
                plugins[0].GetProperty("baseUrl").GetString(),
                Is.EqualTo("https://gitlab.com")
            );
            Assert.That(plugins[1].GetProperty("id").GetInt32(), Is.EqualTo(pluginId2));
            Assert.That(plugins[1].GetProperty("pluginName").GetString(), Is.EqualTo("Jira"));
            Assert.That(plugins[1].GetProperty("isArchived").GetBoolean(), Is.False);
            Assert.That(plugins[1].GetProperty("keys").EnumerateArray(), Is.Empty);
            Assert.That(
                plugins[1].GetProperty("baseUrl").GetString(),
                Is.EqualTo("https://jira.com")
            );
        });

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        Assert.Multiple(() =>
        {
            Assert.That(logs.GetArrayLength(), Is.EqualTo(2));
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new global plugin with properties: PluginName = GitLab, IsArchived = False, BaseUrl = https://gitlab.com, Keys[0] = key1"
                )
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new global plugin with properties: PluginName = Jira, IsArchived = False, BaseUrl = https://jira.com, Keys[0] = key2"
                )
            );
        });
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
        Assert.Multiple(() =>
        {
            Assert.That(updatedPlugin.GetProperty("id").GetInt32(), Is.EqualTo(pluginId));
            Assert.That(updatedPlugin.GetProperty("pluginName").GetString(), Is.EqualTo("Jira"));
            Assert.That(updatedPlugin.GetProperty("isArchived").GetBoolean(), Is.False);
            Assert.That(updatedPlugin.GetProperty("keys").EnumerateArray(), Is.Empty);
        });

        var plugins = (await ToJsonElement(client.GetAsync("/Plugins"))).GetProperty("resources");
        Assert.Multiple(() =>
        {
            Assert.That(plugins.GetArrayLength(), Is.EqualTo(1));
            Assert.That(plugins[0].GetProperty("id").GetInt32(), Is.EqualTo(pluginId));
            Assert.That(plugins[0].GetProperty("pluginName").GetString(), Is.EqualTo("Jira"));
            Assert.That(plugins[0].GetProperty("isArchived").GetBoolean(), Is.False);
            Assert.That(plugins[0].GetProperty("keys").EnumerateArray(), Is.Empty);
        });

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.Multiple(() =>
        {
            Assert.That(logs.GetArrayLength(), Is.EqualTo(2));
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new global plugin with properties: PluginName = GitLab, IsArchived = False, BaseUrl = https://gitlab.com, Keys[0] = key1"
                )
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin updated global plugin GitLab: set PluginName from GitLab to Jira, set BaseUrl from https://gitlab.com to https://jira.com"
                )
            );
        });
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

        Assert.Multiple(() =>
        {
            Assert.That(updatedPlugin.GetProperty("id").GetInt32(), Is.EqualTo(pluginId));
            Assert.That(updatedPlugin.GetProperty("pluginName").GetString(), Is.EqualTo("GitLab"));
            Assert.That(updatedPlugin.GetProperty("isArchived").GetBoolean(), Is.True);
            Assert.That(updatedPlugin.GetProperty("keys").EnumerateArray(), Is.Empty);
        });
        var plugins = (await ToJsonElement(client.GetAsync("/Plugins"))).GetProperty("resources");
        Assert.Multiple(() =>
        {
            Assert.That(plugins.GetArrayLength(), Is.EqualTo(1));
            Assert.That(plugins[0].GetProperty("id").GetInt32(), Is.EqualTo(pluginId));
            Assert.That(plugins[0].GetProperty("pluginName").GetString(), Is.EqualTo("GitLab"));
            Assert.That(plugins[0].GetProperty("isArchived").GetBoolean(), Is.True);
            Assert.That(plugins[0].GetProperty("keys").EnumerateArray(), Is.Empty);
        });
        updatedPlugin = await ToJsonElement(
            client.PatchAsync($"/Plugins/{pluginId}", StringContent("""{ "isArchived": false }"""))
        );

        Assert.Multiple(() =>
        {
            Assert.That(updatedPlugin.GetProperty("id").GetInt32(), Is.EqualTo(pluginId));
            Assert.That(updatedPlugin.GetProperty("pluginName").GetString(), Is.EqualTo("GitLab"));
            Assert.That(updatedPlugin.GetProperty("isArchived").GetBoolean(), Is.False);
            Assert.That(updatedPlugin.GetProperty("keys").EnumerateArray(), Is.Empty);
        });
        plugins = (await ToJsonElement(client.GetAsync("/Plugins"))).GetProperty("resources");

        Assert.Multiple(() =>
        {
            Assert.That(plugins.GetArrayLength(), Is.EqualTo(1));
            Assert.That(plugins[0].GetProperty("id").GetInt32(), Is.EqualTo(pluginId));
            Assert.That(plugins[0].GetProperty("pluginName").GetString(), Is.EqualTo("GitLab"));
            Assert.That(plugins[0].GetProperty("isArchived").GetBoolean(), Is.False);
            Assert.That(plugins[0].GetProperty("keys").EnumerateArray(), Is.Empty);
        });

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        Assert.Multiple(() =>
        {
            Assert.That(logs.GetArrayLength(), Is.EqualTo(3));
            Assert.That(
                logs[2].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new global plugin with properties: PluginName = GitLab, IsArchived = False, BaseUrl = https://gitlab.com, Keys[0] = key1"
                )
            );
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo("admin archived global plugin GitLab")
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo("admin unarchived global plugin GitLab")
            );
        });
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

        Assert.That(updatedPlugin.GetProperty("isArchived").GetBoolean(), Is.True);

        var deleteResponse = await ToJsonElement(client.DeleteAsync($"/Plugins/{pluginId}"));
        Assert.That(deleteResponse.GetProperty("pluginId").GetInt32(), Is.EqualTo(pluginId));
        var plugins = (await ToJsonElement(client.GetAsync("/Plugins"))).GetProperty("resources");

        Assert.That(plugins.GetArrayLength(), Is.EqualTo(0));
        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.Multiple(() =>
        {
            Assert.That(logs.GetArrayLength(), Is.EqualTo(3));
            Assert.That(
                logs[2].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new global plugin with properties: PluginName = GitLab, IsArchived = False, BaseUrl = https://gitlab.com, Keys[0] = key1"
                )
            );
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo("admin archived global plugin GitLab")
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo("admin removed global plugin GitLab")
            );
        });
    }

    [Test]
    public async Task PluginNameMustBeUnique()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        var response = (await client.PutAsync("/Plugins", CreateRequest));
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var errorResponse = await ToErrorResponse(
            client.PutAsync("/Plugins", CreateRequest),
            HttpStatusCode.Conflict
        );

        // Assert
        Assert.That(
            errorResponse.Message,
            Is.EqualTo("A global Plugin with the name GitLab already exists.")
        );
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
        Assert.That(errorResponse.Message, Is.EqualTo("The plugin with id 1 was not found."));
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
        Assert.That(errorResponse.Message, Is.EqualTo("The plugin 1 is not archived."));
    }
}
