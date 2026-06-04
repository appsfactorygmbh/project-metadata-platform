using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components.RenderTree;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class BusinessUnitManagement : IntegrationTestsBase
{
    private static readonly StringContent CreateRequest = StringContent(
        """{ "businessUnitName": "BusinessUnit1"}"""
    );

    private static readonly StringContent CreateRequest2 = StringContent(
        """{ "businessUnitName": "BusinessUnit2"}"""
    );

    [Test]
    public async Task CreateMultipleBusinessUnits()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var businessUnitId1 = (
            await ToJsonElement(
                client.PutAsync("/BusinessUnits", CreateRequest),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var businessUnitId2 = (
            await ToJsonElement(
                client.PutAsync("/BusinessUnits", CreateRequest2),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var businessUnits = await ToJsonElement(client.GetAsync("/BusinessUnits"));

        businessUnits.GetArrayLength().Should().Be(2);
        businessUnits[0].GetProperty("id").GetInt32().Should().Be(businessUnitId1);
        businessUnits[0].GetProperty("businessUnitName").GetString().Should().Be("BusinessUnit1");
        businessUnits[1].GetProperty("id").GetInt32().Should().Be(businessUnitId2);
        businessUnits[1].GetProperty("businessUnitName").GetString().Should().Be("BusinessUnit2");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        logs.GetArrayLength().Should().Be(2);

        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new business unit with properties: BusinessUnitName = BusinessUnit1"
            );
        logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new business unit with properties: BusinessUnitName = BusinessUnit2"
            );
    }

    [Test]
    public async Task BusinessUnitNameMustBeUnique()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        _ = (
            await ToJsonElement(
                client.PutAsync("/BusinessUnits", CreateRequest),
                HttpStatusCode.Created
            )
        );

        var error = (
            await ToErrorResponse(
                client.PutAsync("/BusinessUnits", CreateRequest),
                HttpStatusCode.Conflict
            )
        );

        error.Message.Should().Be("A Business Unit with the name BusinessUnit1 already exists.");
    }

    [Test]
    public async Task DeleteBusinessUnits()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var businessUnitId1 = (
            await ToJsonElement(
                client.PutAsync("/BusinessUnits", CreateRequest),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
        var businessUnits = await ToJsonElement(client.GetAsync("/BusinessUnits"));

        businessUnits.GetArrayLength().Should().Be(1);
        businessUnits[0].GetProperty("id").GetInt32().Should().Be(businessUnitId1);
        businessUnits[0].GetProperty("businessUnitName").GetString().Should().Be("BusinessUnit1");

        (await client.DeleteAsync($"/BusinessUnits/{businessUnitId1}"))
            .StatusCode.Should()
            .Be(HttpStatusCode.NoContent);

        var businessUnitsAfterDelete = await ToJsonElement(client.GetAsync("/BusinessUnits"));

        businessUnitsAfterDelete.GetArrayLength().Should().Be(0);
    }
}
