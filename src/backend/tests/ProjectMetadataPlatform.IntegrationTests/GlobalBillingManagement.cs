using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Azure;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class GlobalBillingManagement : IntegrationTestsBase
{
    private static readonly StringContent CreateRequest = StringContent(
        """{ "billingKind": "gitLab", "currency": "de-de", "budgetLimit": 500, "hostingFee": 300, "targetMargin": 30, "timeFrame": "NEVER" }"""
    );
    private static readonly StringContent CreateRequest2 = StringContent(
        """{ "billingKind": "devOps" }"""
    );

    [Test]
    public async Task CreateMultipleBillingObjects()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var billingId1 = (
            await ToJsonElement(client.PutAsync("/Billing", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();
        var billingId2 = (
            await ToJsonElement(client.PutAsync("/Billing", CreateRequest2), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();

        var billing = (await ToJsonElement(client.GetAsync("/Billing"))).GetProperty("resources");

        Assert.Multiple(() =>
        {
            Assert.That(billing.GetArrayLength(), Is.EqualTo(2));
            Assert.That(billing[0].GetProperty("id").GetInt32(), Is.EqualTo(billingId1));
            Assert.That(billing[0].GetProperty("billingKind").GetString(), Is.EqualTo("gitLab"));
            Assert.That(billing[0].GetProperty("currency").GetString(), Is.EqualTo("de-de"));
            Assert.That(billing[0].GetProperty("budgetLimit").GetDecimal(), Is.EqualTo(500));
            Assert.That(billing[0].GetProperty("hostingFee").GetDecimal(), Is.EqualTo(300));
            Assert.That(billing[0].GetProperty("targetMargin").GetInt32(), Is.EqualTo(30));
            Assert.That(billing[0].GetProperty("timeFrame").GetString(), Is.EqualTo("NEVER"));

            Assert.That(billing[1].GetProperty("id").GetInt32(), Is.EqualTo(billingId2));
            Assert.That(billing[1].GetProperty("billingKind").GetString(), Is.EqualTo("devOps"));
        });

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        Assert.Multiple(() =>
        {
            Assert.That(logs.GetArrayLength(), Is.EqualTo(2));
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added new global billing information with properties: BillingKind = gitLab, Currency = de-de, BudgetLimit = 500, HostingFee = 300, TargetMargin = 30, TimeFrame = NEVER"
                )
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added new global billing information with properties: BillingKind = devOps"
                )
            );
        });
    }

    [Test]
    public async Task UpdateBilling()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var billingId = (
            await ToJsonElement(client.PutAsync("/Billing", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();

        var updatedBilling = await ToJsonElement(
            client.PatchAsync($"/Billing/{billingId}", CreateRequest2)
        );
        Assert.Multiple(() =>
        {
            Assert.That(updatedBilling.GetProperty("id").GetInt32(), Is.EqualTo(billingId));
            Assert.That(
                updatedBilling.GetProperty("billingKind").GetString(),
                Is.EqualTo("devOps")
            );
        });

        var billing = (await ToJsonElement(client.GetAsync("/Billing"))).GetProperty("resources");
        Assert.Multiple(() =>
        {
            Assert.That(billing[0].GetProperty("id").GetInt32(), Is.EqualTo(billingId));
            Assert.That(billing[0].GetProperty("billingKind").GetString(), Is.EqualTo("devOps"));
        });

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.Multiple(() =>
        {
            Assert.That(logs.GetArrayLength(), Is.EqualTo(2));
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added new global billing information with properties: BillingKind = gitLab, Currency = de-de, BudgetLimit = 500, HostingFee = 300, TargetMargin = 30, TimeFrame = NEVER"
                )
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin updated global billing information devOps: set BillingKind from gitLab to devOps, set Currency from de-de to null, set BudgetLimit from 500 to null, set HostingFee from 300 to null, set TargetMargin from 30 to null, set TimeFrame from NEVER to null"
                )
            );
        });
    }

    [Test]
    public async Task DeleteBilling()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var billingId = (
            await ToJsonElement(client.PutAsync("/Billing", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("id")
            .GetInt32();

        var response = await client.DeleteAsync($"/Billing/{billingId}");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        var billing = (await ToJsonElement(client.GetAsync("/Billing"))).GetProperty("resources");

        Assert.That(billing.GetArrayLength(), Is.EqualTo(0));
        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.Multiple(() =>
        {
            Assert.That(logs.GetArrayLength(), Is.EqualTo(2));
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added new global billing information with properties: BillingKind = gitLab, Currency = de-de, BudgetLimit = 500, HostingFee = 300, TargetMargin = 30, TimeFrame = NEVER"
                )
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo("admin removed global billing information gitLab")
            );
        });
    }

    [Test]
    public async Task BillingKindMustBeUnique()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        var response = await client.PutAsync("/Billing", CreateRequest);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var errorResponse = await ToErrorResponse(
            client.PutAsync("/Billing", CreateRequest),
            HttpStatusCode.Conflict
        );

        // Assert
        Assert.That(
            errorResponse.Message,
            Is.EqualTo("Billing information of the kind gitLab already exists.")
        );
    }

    [Test]
    public async Task BillingKindCantBeWhitespace()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var errorResponse = await ToErrorResponse(
            client.PutAsJsonAsync("/Billing", new { BillingKind = "" }),
            HttpStatusCode.BadRequest
        );

        // Assert
        Assert.That(errorResponse.Message, Is.EqualTo("BillingKind can't be empty or whitespaces"));
        // Act
        var response = await client.PutAsync("/Billing", CreateRequest);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

        var errorResponse2 = await ToErrorResponse(
            client.PatchAsJsonAsync(
                "/Billing/1",
                new { BillingKind = "                                                     " }
            ),
            HttpStatusCode.BadRequest
        );

        // Assert
        Assert.That(
            errorResponse2.Message,
            Is.EqualTo("BillingKind can't be empty or whitespaces")
        );
    }

    [Test]
    public async Task NotFoundIsReturnedForInvalidBillingId([Values] bool patch)
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var responseTask = patch
            ? client.PatchAsync("/Billing/1", CreateRequest)
            : client.DeleteAsync("/Billing/1");

        // Act
        var errorResponse = await ToErrorResponse(responseTask, HttpStatusCode.NotFound);

        // Assert
        Assert.That(
            errorResponse.Message,
            Is.EqualTo("The billing information with id 1 was not found.")
        );
    }
}
