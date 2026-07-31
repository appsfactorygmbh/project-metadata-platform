using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class CompanyManagement : IntegrationTestsBase
{
    private static readonly StringContent CreateRequest = StringContent(
        """{ "companyName": "Company1"}"""
    );

    private static readonly StringContent CreateRequest2 = StringContent(
        """{ "companyName": "Company2"}"""
    );

    [Test]
    public async Task CreateMultipleCompanies()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var companyId1 = (
            await ToJsonElement(
                client.PutAsync("/Companies", CreateRequest),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var companyId2 = (
            await ToJsonElement(
                client.PutAsync("/Companies", CreateRequest2),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var companies = (await ToJsonElement(client.GetAsync("/Companies"))).GetProperty(
            "resources"
        );

        Assert.Multiple(() =>
        {
            Assert.That(companies.GetArrayLength(), Is.EqualTo(2));
            Assert.That(companies[0].GetProperty("id").GetInt32(), Is.EqualTo(companyId1));
            Assert.That(
                companies[0].GetProperty("companyName").GetString(),
                Is.EqualTo("Company1")
            );
            Assert.That(companies[1].GetProperty("id").GetInt32(), Is.EqualTo(companyId2));
            Assert.That(
                companies[1].GetProperty("companyName").GetString(),
                Is.EqualTo("Company2")
            );
        });

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.Multiple(() =>
        {
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo("admin added a new company with properties: CompanyName = Company1")
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo("admin added a new company with properties: CompanyName = Company2")
            );
        });
    }

    [Test]
    public async Task CompanyNameMustBeUnique()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        _ = (
            await ToJsonElement(
                client.PutAsync("/Companies", CreateRequest),
                HttpStatusCode.Created
            )
        );

        var error = (
            await ToErrorResponse(
                client.PutAsync("/Companies", CreateRequest),
                HttpStatusCode.Conflict
            )
        );
        Assert.That(error.Message, Is.EqualTo("A Company with the name Company1 already exists."));
    }

    [Test]
    public async Task DeleteCompanies()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var companyId1 = (
            await ToJsonElement(
                client.PutAsync("/Companies", CreateRequest),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
        var companies = (await ToJsonElement(client.GetAsync("/Companies"))).GetProperty(
            "resources"
        );

        Assert.That(companies.GetArrayLength(), Is.EqualTo(1));
        Assert.That(companies[0].GetProperty("id").GetInt32(), Is.EqualTo(companyId1));
        Assert.That(companies[0].GetProperty("companyName").GetString(), Is.EqualTo("Company1"));
        var response = await client.DeleteAsync($"/Companies/{companyId1}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var companiesAfterDelete = (await ToJsonElement(client.GetAsync("/Companies"))).GetProperty(
            "resources"
        );

        Assert.That(companiesAfterDelete.GetArrayLength(), Is.EqualTo(0));
    }
}
