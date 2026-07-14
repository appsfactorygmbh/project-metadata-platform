using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class TeamManagement : IntegrationTestsBase
{
    private static StringContent CreateRequest(int buId) =>
        StringContent("""{ "teamName": "Team1", "businessUnitId": """ + buId + """}""");

    private static StringContent CreateRequest2(int buId) =>
        StringContent("""{ "teamName": "Team2", "businessUnitId": """ + buId + """}""");

    [Test]
    public async Task CreateMultipleTeams()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var buId = await CreateBusinessUnit(client, "Health");
        var buId2 = await CreateBusinessUnit(client, "Media");
        var teamId1 = (
            await ToJsonElement(
                client.PutAsync("/Teams", CreateRequest(buId)),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var teamId2 = (
            await ToJsonElement(
                client.PutAsync("/Teams", CreateRequest2(buId2)),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var teams = (await ToJsonElement(client.GetAsync("/Teams"))).GetProperty("resources");

        Assert.That(teams.GetArrayLength(), Is.EqualTo(2));
        Assert.That(teams[0].GetProperty("id").GetInt32(), Is.EqualTo(teamId1));
        Assert.That(teams[0].GetProperty("teamName").GetString(), Is.EqualTo("Team1"));
        Assert.That(teams[1].GetProperty("id").GetInt32(), Is.EqualTo(teamId2));
        Assert.That(teams[1].GetProperty("teamName").GetString(), Is.EqualTo("Team2"));

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        Assert.That(logs.GetArrayLength(), Is.EqualTo(4));

        Assert.That(
            logs[1].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "admin created a new team with properties: TeamName = Team1, BusinessUnit = Health"
            )
        );
        Assert.That(
            logs[0].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "admin created a new team with properties: TeamName = Team2, BusinessUnit = Media"
            )
        );
    }

    [Test]
    public async Task TeamNameMustBeUnique()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var buId = await CreateBusinessUnit(client, "Health");
        _ = (
            await ToJsonElement(
                client.PutAsync("/Teams", CreateRequest(buId)),
                HttpStatusCode.Created
            )
        );

        var error = (
            await ToErrorResponse(
                client.PutAsync("/Teams", CreateRequest(buId)),
                HttpStatusCode.Conflict
            )
        );

        Assert.That(error.Message, Is.EqualTo("A Team with the name Team1 already exists."));
    }

    [Test]
    public async Task BusinessUnitMustExist()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var error = (
            await ToErrorResponse(
                client.PutAsync("/Teams", CreateRequest(1)),
                HttpStatusCode.NotFound
            )
        );

        Assert.That(error.Message, Is.EqualTo("The Business Unit with id 1 was not found."));
    }

    [Test]
    public async Task DeleteTeams()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var buId = await CreateBusinessUnit(client, "Health");
        var teamId1 = (
            await ToJsonElement(
                client.PutAsync("/Teams", CreateRequest(buId)),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
        var teams = (await ToJsonElement(client.GetAsync("/Teams"))).GetProperty("resources");

        Assert.That(teams.GetArrayLength(), Is.EqualTo(1));
        Assert.That(teams[0].GetProperty("id").GetInt32(), Is.EqualTo(teamId1));
        Assert.That(teams[0].GetProperty("teamName").GetString(), Is.EqualTo("Team1"));

        _ = await ToJsonElement(client.DeleteAsync($"/Teams/{teamId1}"), HttpStatusCode.OK);
        var teamsAfterDelete = (await ToJsonElement(client.GetAsync("/Teams"))).GetProperty(
            "resources"
        );

        Assert.That(teamsAfterDelete.GetArrayLength(), Is.EqualTo(0));
    }

    private static async Task<int> CreateBusinessUnit(HttpClient client, string name)
    {
        return (
            await ToJsonElement(
                client.PutAsync(
                    "/BusinessUnits",
                    StringContent($"{{ \"businessUnitName\": \"{name}\"}}")
                ),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
    }
}
