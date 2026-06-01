using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components.RenderTree;
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

        var teams = await ToJsonElement(client.GetAsync("/Teams"));

        teams.GetArrayLength().Should().Be(2);
        teams[0].GetProperty("id").GetInt32().Should().Be(teamId1);
        teams[0].GetProperty("teamName").GetString().Should().Be("Team1");
        teams[1].GetProperty("id").GetInt32().Should().Be(teamId2);
        teams[1].GetProperty("teamName").GetString().Should().Be("Team2");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        logs.GetArrayLength().Should().Be(4);

        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin created a new team with properties: TeamName = Team1, BusinessUnit = Health"
            );
        logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin created a new team with properties: TeamName = Team2, BusinessUnit = Media");
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

        error.Message.Should().Be("A Team with the name Team1 already exists.");
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

        error.Message.Should().Be("The Business Unit with id 1 was not found.");
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
        var teams = await ToJsonElement(client.GetAsync("/Teams"));

        teams.GetArrayLength().Should().Be(1);
        teams[0].GetProperty("id").GetInt32().Should().Be(teamId1);
        teams[0].GetProperty("teamName").GetString().Should().Be("Team1");

        await ToJsonElement(client.DeleteAsync($"/Teams/{teamId1}"), HttpStatusCode.OK);
        var teamsAfterDelete = await ToJsonElement(client.GetAsync("/Teams"));

        teamsAfterDelete.GetArrayLength().Should().Be(0);
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
