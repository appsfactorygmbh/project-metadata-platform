using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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

        var businessUnits = (await ToJsonElement(client.GetAsync("/BusinessUnits"))).GetProperty(
            "resources"
        );

        Assert.Multiple(() =>
        {
            Assert.That(businessUnits.GetArrayLength(), Is.EqualTo(2));
            Assert.That(businessUnits[0].GetProperty("id").GetInt32(), Is.EqualTo(businessUnitId1));
            Assert.That(
                businessUnits[0].GetProperty("businessUnitName").GetString(),
                Is.EqualTo("BusinessUnit1")
            );
            Assert.That(businessUnits[1].GetProperty("id").GetInt32(), Is.EqualTo(businessUnitId2));
            Assert.That(
                businessUnits[1].GetProperty("businessUnitName").GetString(),
                Is.EqualTo("BusinessUnit2")
            );
        });

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.Multiple(() =>
        {
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new business unit with properties: BusinessUnitName = BusinessUnit1"
                )
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new business unit with properties: BusinessUnitName = BusinessUnit2"
                )
            );
        });
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
        Assert.That(
            error.Message,
            Is.EqualTo("A Business Unit with the name BusinessUnit1 already exists.")
        );
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
        var businessUnits = (await ToJsonElement(client.GetAsync("/BusinessUnits"))).GetProperty(
            "resources"
        );

        Assert.That(businessUnits.GetArrayLength(), Is.EqualTo(1));
        Assert.That(businessUnits[0].GetProperty("id").GetInt32(), Is.EqualTo(businessUnitId1));
        Assert.That(
            businessUnits[0].GetProperty("businessUnitName").GetString(),
            Is.EqualTo("BusinessUnit1")
        );
        var response = await client.DeleteAsync($"/BusinessUnits/{businessUnitId1}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var businessUnitsAfterDelete = (
            await ToJsonElement(client.GetAsync("/BusinessUnits"))
        ).GetProperty("resources");

        Assert.That(businessUnitsAfterDelete.GetArrayLength(), Is.EqualTo(0));
    }
}
