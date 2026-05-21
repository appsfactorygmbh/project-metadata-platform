using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class TeamManagement : IntegrationTestsBase
{
    private static readonly StringContent CreateRequest = StringContent(
        """{ "teamName": "Team1", "BusinessUnit": "Health"}"""
    );
    private static readonly StringContent CreateRequest2 = StringContent(
        """{ "teamName": "Team2", "BusinessUnit": "Media"}"""
    );

    [Test]
    public async Task CreateMultipleTeams()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var teamId1 = (
            await ToJsonElement(client.PutAsync("/Teams", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();

        var teamId2 = (
            await ToJsonElement(client.PutAsync("/Teams", CreateRequest2), HttpStatusCode.Created)
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

        logs.GetArrayLength().Should().Be(2);

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

        _ = (await ToJsonElement(client.PutAsync("/Teams", CreateRequest), HttpStatusCode.Created));

        var error = (
            await ToErrorResponse(client.PutAsync("/Teams", CreateRequest), HttpStatusCode.Conflict)
        );

        error.Message.Should().Be("A Team with the name Team1 already exists.");
    }

    [Test]
    public async Task DeleteTeams()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var teamId1 = (
            await ToJsonElement(client.PutAsync("/Teams", CreateRequest), HttpStatusCode.Created)
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
}
