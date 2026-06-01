using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components.RenderTree;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class OfficeLocationManagement : IntegrationTestsBase
{
    private static readonly StringContent CreateRequest = StringContent(
        """{ "officeLocationName": "OfficeLocation1"}"""
    );

    private static readonly StringContent CreateRequest2 = StringContent(
        """{ "officeLocationName": "OfficeLocation2"}"""
    );

    [Test]
    public async Task CreateMultipleOfficeLocations()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var officeLocationId1 = (
            await ToJsonElement(
                client.PutAsync("/OfficeLocations", CreateRequest),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var officeLocationId2 = (
            await ToJsonElement(
                client.PutAsync("/OfficeLocations", CreateRequest2),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var officeLocations = await ToJsonElement(client.GetAsync("/OfficeLocations"));

        officeLocations.GetArrayLength().Should().Be(2);
        officeLocations[0].GetProperty("id").GetInt32().Should().Be(officeLocationId1);
        officeLocations[0]
            .GetProperty("officeLocationName")
            .GetString()
            .Should()
            .Be("OfficeLocation1");
        officeLocations[1].GetProperty("id").GetInt32().Should().Be(officeLocationId2);
        officeLocations[1]
            .GetProperty("officeLocationName")
            .GetString()
            .Should()
            .Be("OfficeLocation2");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        logs.GetArrayLength().Should().Be(2);

        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new office location with properties: OfficeLocationName = OfficeLocation1"
            );
        logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new office location with properties: OfficeLocationName = OfficeLocation2"
            );
    }

    [Test]
    public async Task OfficeLocationNameMustBeUnique()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        _ = (
            await ToJsonElement(
                client.PutAsync("/OfficeLocations", CreateRequest),
                HttpStatusCode.Created
            )
        );

        var error = (
            await ToErrorResponse(
                client.PutAsync("/OfficeLocations", CreateRequest),
                HttpStatusCode.Conflict
            )
        );

        error.Message.Should().Be("A Office Location with the name OfficeLocation1 already exists.");
    }

    [Test]
    public async Task DeleteOfficeLocations()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var officeLocationId1 = (
            await ToJsonElement(
                client.PutAsync("/OfficeLocations", CreateRequest),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
        var officeLocations = await ToJsonElement(client.GetAsync("/OfficeLocations"));

        officeLocations.GetArrayLength().Should().Be(1);
        officeLocations[0].GetProperty("id").GetInt32().Should().Be(officeLocationId1);
        officeLocations[0]
            .GetProperty("officeLocationName")
            .GetString()
            .Should()
            .Be("OfficeLocation1");

        (await client.DeleteAsync($"/OfficeLocations/{officeLocationId1}"))
            .StatusCode.Should()
            .Be(HttpStatusCode.NoContent);

        var officeLocationsAfterDelete = await ToJsonElement(client.GetAsync("/OfficeLocations"));

        officeLocationsAfterDelete.GetArrayLength().Should().Be(0);
    }
}
