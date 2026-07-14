using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class UserManagement : IntegrationTestsBase
{
    private static readonly StringContent CreateRequest = StringContent(
        """{ "userName": "test@mail.de", "password": "1K@sekuchen", "externalId": "123", "active": true, "urn:ietf:params:scim:schemas:extension:pmp:User": { "departments": ["IT Admin"]  } }"""
    );
    private static readonly StringContent CreateRequest2 = StringContent(
        """{ "userName": "foo@bar.de", "password": "SecretP@ssw0rd", "externalId": "1234", "active": true, "urn:ietf:params:scim:schemas:extension:pmp:User": { "departments": ["IT Development"]  } }"""
    );
    private static readonly StringContent CreateRequest3 = StringContent(
        """{ "userName": "max@mail.de", "password": "1K@sekuchen", "externalId": "123" }"""
    );
    private static readonly StringContent CreateRequest4 = StringContent(
        """{ "userName": "max@mail.de", "password": "1K@sekuchen", "externalId": "123", "urn:ietf:params:scim:schemas:extension:pmp:User": { "Team": ["Team"] } }"""
    );
    private static readonly StringContent CreateRequest5 = StringContent(
        """{ "userName": "test@mail.de", "password": "1K@sekuchen", "externalId": "abc" }"""
    );
    private static readonly StringContent UpdateRequest = StringContent(
        """{"Operations":[{"Op":"Replace","Path":"userName","Value":"foo@bar.de"},{"Op":"Replace","Path":"password","Value":"SecretP@ssw0rd"}]}"""
    );

    [Test]
    public async Task UserCantDeleteThemself()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var adminId = (await ToJsonElement(client.GetAsync("/Users")))
            .GetProperty("Resources")[0]
            .GetProperty("externalId")
            .GetString()!;

        var deleteResponse = await client.DeleteAsync($"/Users/{adminId}");
        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(
            (await deleteResponse.Content.ReadFromJsonAsync<ErrorResponse>())!.Message,
            Is.EqualTo("A User can't delete themself.")
        );

        var newUserId = (
            await ToJsonElement(client.PostAsync("/Users", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("externalId")
            .GetString()!;

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(
            client,
            "test@mail.de",
            "1K@sekuchen"
        );
        deleteResponse = await client.DeleteAsync($"/Users/{newUserId}");

        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        Assert.That(
            (await deleteResponse.Content.ReadFromJsonAsync<ErrorResponse>())!.Message,
            Is.EqualTo("A User can't delete themself.")
        );
    }

    [Test]
    public async Task CreateMultipleUsers()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var userId1 = (
            await ToJsonElement(client.PostAsync("/Users", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("externalId")
            .GetString()!;
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(
            client,
            "test@mail.de",
            "1K@sekuchen"
        );
        var userId2 = (
            await ToJsonElement(client.PostAsync("/Users", CreateRequest2), HttpStatusCode.Created)
        )
            .GetProperty("externalId")
            .GetString()!;

        var users = (await ToJsonElement(client.GetAsync("/Users"))).GetProperty("Resources");

        Assert.That(users.GetArrayLength(), Is.EqualTo(3));
        Assert.That(users[0].GetProperty("userName").GetString(), Is.EqualTo("admin@admin.admin"));
        Assert.That(users[2].GetProperty("externalId").GetString(), Is.EqualTo(userId1));
        Assert.That(users[2].GetProperty("userName").GetString(), Is.EqualTo("test@mail.de"));
        Assert.That(users[1].GetProperty("externalId").GetString(), Is.EqualTo(userId2));
        Assert.That(users[1].GetProperty("userName").GetString(), Is.EqualTo("foo@bar.de"));

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        Assert.That(logs.GetArrayLength(), Is.EqualTo(4));
        Assert.That(
            logs[3].GetProperty("logMessage").GetString(),
            Is.EqualTo("admin added a new department with properties: DepartmentName = IT Admin")
        );
        Assert.That(
            logs[2].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "admin added a new user with properties: Email = test@mail.de, EmployeeId = 123, IsActive = True, IsScimProvisioned = False, Departments = IT Admin"
            )
        );
        Assert.That(
            logs[1].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "test added a new department with properties: DepartmentName = IT Development"
            )
        );
        Assert.That(
            logs[0].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "test added a new user with properties: Email = foo@bar.de, EmployeeId = 1234, IsActive = True, IsScimProvisioned = False, Departments = IT Development"
            )
        );
    }

    [Test]
    public async Task CreateUserWithTeams_NeedsExistingTeam()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var errorResponse = await ToErrorResponse(
            client.PostAsync("/Users", CreateRequest4),
            HttpStatusCode.NotFound
        );

        Assert.That(errorResponse.Message, Is.EqualTo("The team with name Team was not found."));
        var buId = await CreateBusinessUnit(client, "Health");
        _ = await ToJsonElement(
            client.PutAsJsonAsync("/Teams", new { TeamName = "Team", BusinessUnitId = buId }),
            HttpStatusCode.Created
        );

        var user = await ToJsonElement(
            client.PostAsync("/Users", CreateRequest4),
            HttpStatusCode.Created
        );

        Assert.That(
            user.GetProperty("urn:ietf:params:scim:schemas:extension:pmp:User")
                .GetProperty("team")
                .GetArrayLength(),
            Is.EqualTo(1)
        );
        Assert.That(
            user.GetProperty("urn:ietf:params:scim:schemas:extension:pmp:User")
                .GetProperty("team")[0]
                .GetString(),
            Is.EqualTo("Team")
        );
    }

    [Test]
    public async Task CreateMultipleUsers_IsScimProvisionedChangedBasedOnAuth()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var userId1 = (
            await ToJsonElement(client.PostAsync("/Users", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("externalId")
            .GetString()!;
        await CreateApiTokenAndAddItToDefaultRequestHeadersOfClient(
            client,
            scopes: [Domain.Auth.TokenScopes.SCIM, Domain.Auth.TokenScopes.GET_LOG]
        );
        var userId2 = (
            await ToJsonElement(client.PostAsync("/Users", CreateRequest2), HttpStatusCode.Created)
        )
            .GetProperty("externalId")
            .GetString()!;

        var users = (await ToJsonElement(client.GetAsync("/Users"))).GetProperty("Resources");

        Assert.That(users.GetArrayLength(), Is.EqualTo(3));
        Assert.That(users[0].GetProperty("userName").GetString(), Is.EqualTo("admin@admin.admin"));
        Assert.That(users[2].GetProperty("externalId").GetString(), Is.EqualTo(userId1));
        Assert.That(users[2].GetProperty("userName").GetString(), Is.EqualTo("test@mail.de"));
        Assert.That(
            users[2]
                .GetProperty("urn:ietf:params:scim:schemas:extension:pmp:User")
                .GetProperty("isScimProvisioned")
                .GetBoolean(),
            Is.EqualTo(false)
        );
        Assert.That(users[1].GetProperty("externalId").GetString(), Is.EqualTo(userId2));
        Assert.That(users[1].GetProperty("userName").GetString(), Is.EqualTo("foo@bar.de"));
        Assert.That(
            users[1]
                .GetProperty("urn:ietf:params:scim:schemas:extension:pmp:User")
                .GetProperty("isScimProvisioned")
                .GetBoolean(),
            Is.EqualTo(true)
        );

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        Assert.That(logs.GetArrayLength(), Is.EqualTo(5));
        Assert.That(
            logs[4].GetProperty("logMessage").GetString(),
            Is.EqualTo("admin added a new department with properties: DepartmentName = IT Admin")
        );
        Assert.That(
            logs[3].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "admin added a new user with properties: Email = test@mail.de, EmployeeId = 123, IsActive = True, IsScimProvisioned = False, Departments = IT Admin"
            )
        );
        Assert.That(
            logs[2].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "admin created a new API token with properties: Name = ApiToken, Scopes = SCIM, GET_LOG"
            )
        );
        Assert.That(
            logs[1].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "ApiToken added a new department with properties: DepartmentName = IT Development"
            )
        );

        Assert.That(
            logs[0].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "ApiToken added a new user with properties: Email = foo@bar.de, EmployeeId = 1234, IsActive = True, IsScimProvisioned = True, Departments = IT Development"
            )
        );
    }

    [Test]
    public async Task UpdateUser()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var userId = (
            await ToJsonElement(client.PostAsync("/Users", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("externalId")
            .GetString()!;

        var updatedUser = await ToJsonElement(client.PatchAsync($"/Users/{userId}", UpdateRequest));

        Assert.That(updatedUser.GetProperty("externalId").GetString(), Is.EqualTo(userId));
        Assert.That(updatedUser.GetProperty("userName").GetString(), Is.EqualTo("foo@bar.de"));

        var users = (await ToJsonElement(client.GetAsync("/Users"))).GetProperty("Resources");

        Assert.That(users.GetArrayLength(), Is.EqualTo(2));
        Assert.That(users[0].GetProperty("userName").GetString(), Is.EqualTo("admin@admin.admin"));
        Assert.That(users[1].GetProperty("externalId").GetString(), Is.EqualTo(userId));
        Assert.That(users[1].GetProperty("userName").GetString(), Is.EqualTo("foo@bar.de"));

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        Assert.That(logs.GetArrayLength(), Is.EqualTo(3));
        Assert.That(
            logs[2].GetProperty("logMessage").GetString(),
            Is.EqualTo("admin added a new department with properties: DepartmentName = IT Admin")
        );
        Assert.That(
            logs[1].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "admin added a new user with properties: Email = test@mail.de, EmployeeId = 123, IsActive = True, IsScimProvisioned = False, Departments = IT Admin"
            )
        );

        Assert.That(
            logs[0].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "admin updated user test: set Email from test@mail.de to foo@bar.de, changed password"
            )
        );
    }

    [Test]
    public async Task DeleteUser()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var adminId = (await ToJsonElement(client.GetAsync("/Users")))
            .GetProperty("Resources")[0]
            .GetProperty("externalId")
            .GetString()!;
        var userId1 = (
            await ToJsonElement(client.PostAsync("/Users", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("externalId")
            .GetString()!;

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(
            client,
            "test@mail.de",
            "1K@sekuchen"
        );
        var userId2 = (
            await ToJsonElement(client.PostAsync("/Users", CreateRequest2), HttpStatusCode.Created)
        )
            .GetProperty("externalId")
            .GetString()!;

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(
            client,
            "foo@bar.de",
            "SecretP@ssw0rd"
        );
        Assert.That(
            (await client.DeleteAsync($"Users/{userId1}")).StatusCode,
            Is.EqualTo(HttpStatusCode.NoContent)
        );

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        Assert.That(
            (await client.DeleteAsync($"Users/{userId2}")).StatusCode,
            Is.EqualTo(HttpStatusCode.NoContent)
        );

        var users = (await ToJsonElement(client.GetAsync("/Users"))).GetProperty("Resources");

        Assert.That(users.GetArrayLength(), Is.EqualTo(1));
        Assert.That(users[0].GetProperty("externalId").GetString(), Is.EqualTo(adminId));
        Assert.That(users[0].GetProperty("userName").GetString(), Is.EqualTo("admin@admin.admin"));

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        Assert.That(logs.GetArrayLength(), Is.EqualTo(6));

        Assert.That(
            logs[3].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "test (deleted actor) added a new department with properties: DepartmentName = IT Development"
            )
        );
        Assert.That(
            logs[2].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "test (deleted actor) added a new user with properties: Email = foo@bar.de, EmployeeId = 1234, IsActive = True, IsScimProvisioned = False, Departments = IT Development"
            )
        );
        Assert.That(
            logs[1].GetProperty("logMessage").GetString(),
            Is.EqualTo("foo (deleted actor) removed user test@mail.de")
        );
        Assert.That(
            logs[0].GetProperty("logMessage").GetString(),
            Is.EqualTo("admin removed user foo@bar.de")
        );
    }

    [Test]
    public async Task EmailOfDeletedUserAtCreationTimeOfTheLogIsUsedInLogs()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var userId = (
            await ToJsonElement(client.PostAsync("/Users", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("externalId")
            .GetString()!;

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(
            client,
            "test@mail.de",
            "1K@sekuchen"
        );

        Assert.That(
            (
                await client.PostAsync(
                    "/Users",
                    StringContent(
                        """{ "userName": "mail@m.de", "password": "1K@sekuchen", "externalId": "a" }"""
                    )
                )
            ).StatusCode,
            Is.EqualTo(HttpStatusCode.Created)
        );

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        Assert.That(
            (await client.PatchAsync($"/Users/{userId}", UpdateRequest)).StatusCode,
            Is.EqualTo(HttpStatusCode.OK)
        );

        Assert.That(
            (await client.DeleteAsync($"Users/{userId}")).StatusCode,
            Is.EqualTo(HttpStatusCode.NoContent)
        );

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        Assert.That(
            logs[3].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "admin added a new user with properties: Email = test@mail.de, EmployeeId = 123, IsActive = True, IsScimProvisioned = False, Departments = IT Admin"
            )
        );
        Assert.That(
            logs[2].GetProperty("logMessage").GetString(),
            Is.EqualTo(
                "test (deleted actor) added a new user with properties: Email = mail@m.de, EmployeeId = a, IsActive = False, IsScimProvisioned = False"
            )
        );
    }

    [Test]
    public async Task EmailMustBeUnique()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        Assert.That(
            (await client.PostAsync("/Users", CreateRequest)).StatusCode,
            Is.EqualTo(HttpStatusCode.Created)
        );
        var errorResponse = await ToErrorResponse(
            client.PostAsync("/Users", CreateRequest5),
            HttpStatusCode.Conflict
        );

        // Assert
        Assert.That(errorResponse.Message, Is.EqualTo("User creation Failed : DuplicateEmail"));
    }

    [Test]
    public async Task EmployeeNrMustBeUnique()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        Assert.That(
            (await client.PostAsync("/Users", CreateRequest)).StatusCode,
            Is.EqualTo(HttpStatusCode.Created)
        );
        var errorResponse = await ToErrorResponse(
            client.PostAsync("/Users", CreateRequest3),
            HttpStatusCode.Conflict
        );

        // Assert
        Assert.That(
            errorResponse.Message,
            Is.EqualTo("User creation Failed : DuplicateEmployeeNumber")
        );
    }

    [Test]
    public async Task PasswordMustBeValid()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        var errorResponse = await ToErrorResponse(
            client.PostAsync(
                "/Users",
                StringContent(
                    """{ "userName": "foo@bar.de", "password": "password", "externalId":"a" }"""
                )
            ),
            HttpStatusCode.BadRequest
        );

        // Assert
        Assert.That(
            errorResponse.Message,
            Is.EqualTo(
                "Invalid password format: Passwords must have at least one non alphanumeric character. Passwords must have at least one digit ('0'-'9'). Passwords must have at least one uppercase ('A'-'Z')."
            )
        );
    }

    [Test]
    public async Task UserGetMeThrowsErrorWithApiTokenAuth()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        await CreateApiTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var errorResponse = await ToErrorResponse(
            client.GetAsync("/Users/Me"),
            HttpStatusCode.Unauthorized
        );

        Assert.That(errorResponse.Message, Is.EqualTo("User not authenticated."));
    }

    [Test]
    public async Task NotFoundIsReturnedForInvalidUserId(
        [Values("GET", "PATCH", "DELETE")] string endpoint
    )
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        var responseTask = endpoint switch
        {
            "GET" => client.GetAsync("/Users/10"),
            "PATCH" => client.PatchAsync("/Users/10", CreateRequest),
            "DELETE" => client.DeleteAsync("/Users/10"),
            _ => throw new ArgumentOutOfRangeException(nameof(endpoint), endpoint, null),
        };

        // Act
        var errorResponse = await ToErrorResponse(responseTask, HttpStatusCode.NotFound);

        // Assert
        Assert.That(errorResponse.Message, Is.EqualTo("The user 10 was not found."));
    }

    private static async Task<int> CreateBusinessUnit(HttpClient client, string name)
    {
        return (
            await ToJsonElement(
                client.PutAsync(
                    "/BusinessUnits",
                    StringContent($"{{ \"businessUnitName\": \"{name}\"}}")
                ),
                HttpStatusCode.Created
            )
        )
            .GetProperty("id")
            .GetInt32();
    }
}
