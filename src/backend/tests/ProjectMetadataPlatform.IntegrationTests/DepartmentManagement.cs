using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class DepartmentManagement : IntegrationTestsBase
{
    private static readonly StringContent CreateRequest = StringContent(
        """{ "departmentName": "Department1"}"""
    );

    private static readonly StringContent CreateRequest2 = StringContent(
        """{ "departmentName": "Department2"}"""
    );

    [Test]
    public async Task CreateMultipleDepartments()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var departmentId1 = (
            await ToJsonElement(
                client.PutAsync("/Departments", CreateRequest),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var departmentId2 = (
            await ToJsonElement(
                client.PutAsync("/Departments", CreateRequest2),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();

        var departments = await ToJsonElement(client.GetAsync("/Departments"));

        _ = departments.GetArrayLength().Should().Be(2);
        _ = departments[0].GetProperty("id").GetInt32().Should().Be(departmentId1);
        _ = departments[0].GetProperty("departmentName").GetString().Should().Be("Department1");
        _ = departments[1].GetProperty("id").GetInt32().Should().Be(departmentId2);
        _ = departments[1].GetProperty("departmentName").GetString().Should().Be("Department2");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        _ = logs.GetArrayLength().Should().Be(2);

        _ = logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin added a new department with properties: DepartmentName = Department1");
        _ = logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin added a new department with properties: DepartmentName = Department2");
    }

    [Test]
    public async Task DepartmentNameMustBeUnique()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        _ = (
            await ToJsonElement(
                client.PutAsync("/Departments", CreateRequest),
                HttpStatusCode.Created
            )
        );

        var error = (
            await ToErrorResponse(
                client.PutAsync("/Departments", CreateRequest),
                HttpStatusCode.Conflict
            )
        );

        _ = error.Message.Should().Be("A Department with the name Department1 already exists.");
    }

    [Test]
    public async Task DeleteDepartments()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var departmentId1 = (
            await ToJsonElement(
                client.PutAsync("/Departments", CreateRequest),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
        var departments = await ToJsonElement(client.GetAsync("/Departments"));

        _ = departments.GetArrayLength().Should().Be(1);
        _ = departments[0].GetProperty("id").GetInt32().Should().Be(departmentId1);
        _ = departments[0].GetProperty("departmentName").GetString().Should().Be("Department1");

        _ = (await client.DeleteAsync($"/Departments/{departmentId1}"))
            .StatusCode.Should()
            .Be(HttpStatusCode.NoContent);

        var departmentsAfterDelete = await ToJsonElement(client.GetAsync("/Departments"));

        _ = departmentsAfterDelete.GetArrayLength().Should().Be(0);
    }
}
