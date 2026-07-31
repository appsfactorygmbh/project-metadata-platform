using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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

        var officeLocations = (
            await ToJsonElement(client.GetAsync("/OfficeLocations"))
        ).GetProperty("resources");

        Assert.Multiple(() =>
        {
            Assert.That(officeLocations.GetArrayLength(), Is.EqualTo(2));
            Assert.That(
                officeLocations[0].GetProperty("id").GetInt32(),
                Is.EqualTo(officeLocationId1)
            );
            Assert.That(
                officeLocations[0].GetProperty("officeLocationName").GetString(),
                Is.EqualTo("OfficeLocation1")
            );
            Assert.That(
                officeLocations[1].GetProperty("id").GetInt32(),
                Is.EqualTo(officeLocationId2)
            );
            Assert.That(
                officeLocations[1].GetProperty("officeLocationName").GetString(),
                Is.EqualTo("OfficeLocation2")
            );
        });

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.Multiple(() =>
        {
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new office location with properties: OfficeLocationName = OfficeLocation1"
                )
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new office location with properties: OfficeLocationName = OfficeLocation2"
                )
            );
        });
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
        Assert.That(
            error.Message,
            Is.EqualTo("A Office Location with the name OfficeLocation1 already exists.")
        );
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
        var officeLocations = (
            await ToJsonElement(client.GetAsync("/OfficeLocations"))
        ).GetProperty("resources");

        Assert.That(officeLocations.GetArrayLength(), Is.EqualTo(1));
        Assert.That(officeLocations[0].GetProperty("id").GetInt32(), Is.EqualTo(officeLocationId1));
        Assert.That(
            officeLocations[0].GetProperty("officeLocationName").GetString(),
            Is.EqualTo("OfficeLocation1")
        );
        var response = await client.DeleteAsync($"/OfficeLocations/{officeLocationId1}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var officeLocationsAfterDelete = (
            await ToJsonElement(client.GetAsync("/OfficeLocations"))
        ).GetProperty("resources");

        Assert.That(officeLocationsAfterDelete.GetArrayLength(), Is.EqualTo(0));
    }
}
