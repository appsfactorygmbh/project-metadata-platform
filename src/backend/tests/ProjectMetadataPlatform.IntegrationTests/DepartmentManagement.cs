using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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

        var departments = (await ToJsonElement(client.GetAsync("/Departments"))).GetProperty(
            "resources"
        );

        Assert.Multiple(() =>
        {
            Assert.That(departments.GetArrayLength(), Is.EqualTo(2));
            Assert.That(departments[0].GetProperty("id").GetInt32(), Is.EqualTo(departmentId1));
            Assert.That(
                departments[0].GetProperty("departmentName").GetString(),
                Is.EqualTo("Department1")
            );
            Assert.That(departments[1].GetProperty("id").GetInt32(), Is.EqualTo(departmentId2));
            Assert.That(
                departments[1].GetProperty("departmentName").GetString(),
                Is.EqualTo("Department2")
            );
        });

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.Multiple(() =>
        {
            Assert.That(
                logs[1].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new department with properties: DepartmentName = Department1"
                )
            );
            Assert.That(
                logs[0].GetProperty("logMessage").GetString(),
                Is.EqualTo(
                    "admin added a new department with properties: DepartmentName = Department2"
                )
            );
        });
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
        Assert.That(
            error.Message,
            Is.EqualTo("A Department with the name Department1 already exists.")
        );
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
        var departments = (await ToJsonElement(client.GetAsync("/Departments"))).GetProperty(
            "resources"
        );

        Assert.That(departments.GetArrayLength(), Is.EqualTo(1));
        Assert.That(departments[0].GetProperty("id").GetInt32(), Is.EqualTo(departmentId1));
        Assert.That(
            departments[0].GetProperty("departmentName").GetString(),
            Is.EqualTo("Department1")
        );
        var response = await client.DeleteAsync($"/Departments/{departmentId1}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));

        var departmentsAfterDelete = (
            await ToJsonElement(client.GetAsync("/Departments"))
        ).GetProperty("resources");

        Assert.That(departmentsAfterDelete.GetArrayLength(), Is.EqualTo(0));
    }
}
