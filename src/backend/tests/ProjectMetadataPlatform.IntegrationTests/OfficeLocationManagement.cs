using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
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

        _ = officeLocations.GetArrayLength().Should().Be(2);
        _ = officeLocations[0].GetProperty("id").GetInt32().Should().Be(officeLocationId1);
        _ = officeLocations[0]
            .GetProperty("officeLocationName")
            .GetString()
            .Should()
            .Be("OfficeLocation1");
        _ = officeLocations[1].GetProperty("id").GetInt32().Should().Be(officeLocationId2);
        _ = officeLocations[1]
            .GetProperty("officeLocationName")
            .GetString()
            .Should()
            .Be("OfficeLocation2");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        _ = logs.GetArrayLength().Should().Be(2);

        _ = logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new office location with properties: OfficeLocationName = OfficeLocation1"
            );
        _ = logs[0]
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

        _ = error
            .Message.Should()
            .Be("A Office Location with the name OfficeLocation1 already exists.");
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

        _ = officeLocations.GetArrayLength().Should().Be(1);
        _ = officeLocations[0].GetProperty("id").GetInt32().Should().Be(officeLocationId1);
        _ = officeLocations[0]
            .GetProperty("officeLocationName")
            .GetString()
            .Should()
            .Be("OfficeLocation1");

        _ = (await client.DeleteAsync($"/OfficeLocations/{officeLocationId1}"))
            .StatusCode.Should()
            .Be(HttpStatusCode.NoContent);

        var officeLocationsAfterDelete = await ToJsonElement(client.GetAsync("/OfficeLocations"));

        _ = officeLocationsAfterDelete.GetArrayLength().Should().Be(0);
    }
}
