using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class AuthorizationManagement : IntegrationTestsBase
{
    [Test]
    public async Task GetResources_ReturnsResources_Test()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var resources = (
            await ToJsonElement(client.GetAsync("/Authorization/Resources"), HttpStatusCode.OK)
        );

        Assert.That(resources.GetArrayLength(), Is.EqualTo(1));
        Assert.That(resources[0].ToString(), Is.EqualTo("Project"));
    }

    [Test]
    public async Task GetPermissions_ReturnsAllowed_Test()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var permissions = (
            await ToJsonElement(client.GetAsync("/Authorization/Project"), HttpStatusCode.OK)
        );
        Assert.That(permissions.GetArrayLength(), Is.EqualTo(4));
        Assert.That(permissions[0].GetProperty("action").ToString(), Is.EqualTo("GET"));
        Assert.That(
            permissions[0].GetProperty("filter").GetProperty("value").ToString(),
            Is.EqualTo("AlwaysAllowed")
        );
        Assert.That(permissions[1].GetProperty("action").ToString(), Is.EqualTo("CREATE"));
        Assert.That(
            permissions[1].GetProperty("filter").GetProperty("value").ToString(),
            Is.EqualTo("AlwaysAllowed")
        );
        Assert.That(permissions[2].GetProperty("action").ToString(), Is.EqualTo("EDIT"));
        Assert.That(
            permissions[2].GetProperty("filter").GetProperty("value").ToString(),
            Is.EqualTo("AlwaysAllowed")
        );
        Assert.That(permissions[3].GetProperty("action").ToString(), Is.EqualTo("DELETE"));
        Assert.That(
            permissions[3].GetProperty("filter").GetProperty("value").ToString(),
            Is.EqualTo("AlwaysAllowed")
        );
    }
}
