using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Components.RenderTree;
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

        var companies = await ToJsonElement(client.GetAsync("/Companies"));

        companies.GetArrayLength().Should().Be(2);
        companies[0].GetProperty("id").GetInt32().Should().Be(companyId1);
        companies[0].GetProperty("companyName").GetString().Should().Be("Company1");
        companies[1].GetProperty("id").GetInt32().Should().Be(companyId2);
        companies[1].GetProperty("companyName").GetString().Should().Be("Company2");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        logs.GetArrayLength().Should().Be(2);

        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin added a new company with properties: CompanyName = Company1");
        logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin added a new company with properties: CompanyName = Company2");
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

        error.Message.Should().Be("A Company with the name Company1 already exists.");
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
        var companies = await ToJsonElement(client.GetAsync("/Companies"));

        companies.GetArrayLength().Should().Be(1);
        companies[0].GetProperty("id").GetInt32().Should().Be(companyId1);
        companies[0].GetProperty("companyName").GetString().Should().Be("Company1");

        (await client.DeleteAsync($"/Companies/{companyId1}"))
            .StatusCode.Should()
            .Be(HttpStatusCode.NoContent);

        var companiesAfterDelete = await ToJsonElement(client.GetAsync("/Companies"));

        companiesAfterDelete.GetArrayLength().Should().Be(0);
    }
}
