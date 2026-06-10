using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

public class UserManagement : IntegrationTestsBase
{
    private static readonly StringContent CreateRequest = StringContent(
        """{ "userName": "test@mail.de", "password": "1K@sekuchen", "externalId": "123" }"""
    );
    private static readonly StringContent CreateRequest2 = StringContent(
        """{ "userName": "foo@bar.de", "password": "SecretP@ssw0rd", "externalId": "1234" }"""
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
        _ = deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _ = (await deleteResponse.Content.ReadFromJsonAsync<ErrorResponse>())!
            .Message.Should()
            .Be("A User can't delete themself.");

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

        _ = deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        _ = (await deleteResponse.Content.ReadFromJsonAsync<ErrorResponse>())!
            .Message.Should()
            .Be("A User can't delete themself.");
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

        _ = users.GetArrayLength().Should().Be(3);
        _ = users[0].GetProperty("userName").GetString().Should().Be("admin@admin.admin");
        _ = users[2].GetProperty("externalId").GetString().Should().Be(userId1);
        _ = users[2].GetProperty("userName").GetString().Should().Be("test@mail.de");
        _ = users[1].GetProperty("externalId").GetString().Should().Be(userId2);
        _ = users[1].GetProperty("userName").GetString().Should().Be("foo@bar.de");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        _ = logs.GetArrayLength().Should().Be(2);

        _ = logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new user with properties: Email = test@mail.de, EmployeeId = 123, IsActive = False, IsScimProvisioned = False"
            );
        _ = logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "test added a new user with properties: Email = foo@bar.de, EmployeeId = 1234, IsActive = False, IsScimProvisioned = False"
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

        _ = errorResponse.Message.Should().Be("The team with name Team was not found.");
        var buId = await CreateBusinessUnit(client, "Health");
        _ = await ToJsonElement(
            client.PutAsJsonAsync("/Teams", new { TeamName = "Team", BusinessUnitId = buId }),
            HttpStatusCode.Created
        );

        var user = await ToJsonElement(
            client.PostAsync("/Users", CreateRequest4),
            HttpStatusCode.Created
        );

        _ = user.GetProperty("urn:ietf:params:scim:schemas:extension:pmp:User")
            .GetProperty("team")
            .GetArrayLength()
            .Should()
            .Be(1);
        _ = user.GetProperty("urn:ietf:params:scim:schemas:extension:pmp:User")
            .GetProperty("team")[0]
            .GetString()
            .Should()
            .Be("Team");
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
        await CreateApiTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var userId2 = (
            await ToJsonElement(client.PostAsync("/Users", CreateRequest2), HttpStatusCode.Created)
        )
            .GetProperty("externalId")
            .GetString()!;

        var users = (await ToJsonElement(client.GetAsync("/Users"))).GetProperty("Resources");

        _ = users.GetArrayLength().Should().Be(3);
        _ = users[0].GetProperty("userName").GetString().Should().Be("admin@admin.admin");
        _ = users[2].GetProperty("externalId").GetString().Should().Be(userId1);
        _ = users[2].GetProperty("userName").GetString().Should().Be("test@mail.de");
        _ = users[2]
            .GetProperty("urn:ietf:params:scim:schemas:extension:pmp:User")
            .GetProperty("isScimProvisioned")
            .GetBoolean()
            .Should()
            .Be(false);
        _ = users[1].GetProperty("externalId").GetString().Should().Be(userId2);
        _ = users[1].GetProperty("userName").GetString().Should().Be("foo@bar.de");
        _ = users[1]
            .GetProperty("urn:ietf:params:scim:schemas:extension:pmp:User")
            .GetProperty("isScimProvisioned")
            .GetBoolean()
            .Should()
            .Be(true);

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        _ = logs.GetArrayLength().Should().Be(3);

        _ = logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new user with properties: Email = test@mail.de, EmployeeId = 123, IsActive = False, IsScimProvisioned = False"
            );
        _ = logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin created a new API token with properties: Name = ApiToken");

        _ = logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "ApiToken added a new user with properties: Email = foo@bar.de, EmployeeId = 1234, IsActive = False, IsScimProvisioned = True"
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

        _ = updatedUser.GetProperty("externalId").GetString().Should().Be(userId);
        _ = updatedUser.GetProperty("userName").GetString().Should().Be("foo@bar.de");

        var users = (await ToJsonElement(client.GetAsync("/Users"))).GetProperty("Resources");

        _ = users.GetArrayLength().Should().Be(2);
        _ = users[0].GetProperty("userName").GetString().Should().Be("admin@admin.admin");
        _ = users[1].GetProperty("externalId").GetString().Should().Be(userId);
        _ = users[1].GetProperty("userName").GetString().Should().Be("foo@bar.de");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        _ = logs.GetArrayLength().Should().Be(2);

        _ = logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new user with properties: Email = test@mail.de, EmployeeId = 123, IsActive = False, IsScimProvisioned = False"
            );

        _ = logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin updated user test: set Email from test@mail.de to foo@bar.de, changed password"
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
        _ = (await client.DeleteAsync($"Users/{userId1}"))
            .StatusCode.Should()
            .Be(HttpStatusCode.NoContent);

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        _ = (await client.DeleteAsync($"Users/{userId2}"))
            .StatusCode.Should()
            .Be(HttpStatusCode.NoContent);

        var users = (await ToJsonElement(client.GetAsync("/Users"))).GetProperty("Resources");

        _ = users.GetArrayLength().Should().Be(1);
        _ = users[0].GetProperty("externalId").GetString().Should().Be(adminId);
        _ = users[0].GetProperty("userName").GetString().Should().Be("admin@admin.admin");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        _ = logs.GetArrayLength().Should().Be(4);

        _ = logs[3]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new user with properties: Email = test@mail.de, EmployeeId = 123, IsActive = False, IsScimProvisioned = False"
            );
        _ = logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "test (deleted actor) added a new user with properties: Email = foo@bar.de, EmployeeId = 1234, IsActive = False, IsScimProvisioned = False"
            );
        _ = logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("foo (deleted actor) removed user test@mail.de");
        _ = logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin removed user foo@bar.de");
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

        _ = (
            await client.PostAsync(
                "/Users",
                StringContent(
                    """{ "userName": "mail@m.de", "password": "1K@sekuchen", "externalId": "a" }"""
                )
            )
        )
            .StatusCode.Should()
            .Be(HttpStatusCode.Created);

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        _ = (await client.PatchAsync($"/Users/{userId}", UpdateRequest))
            .StatusCode.Should()
            .Be(HttpStatusCode.OK);

        _ = (await client.DeleteAsync($"Users/{userId}"))
            .StatusCode.Should()
            .Be(HttpStatusCode.NoContent);

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        _ = logs[3]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "admin added a new user with properties: Email = test@mail.de, EmployeeId = 123, IsActive = False, IsScimProvisioned = False"
            );
        _ = logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be(
                "test (deleted actor) added a new user with properties: Email = mail@m.de, EmployeeId = a, IsActive = False, IsScimProvisioned = False"
            );
    }

    [Test]
    public async Task EmailMustBeUnique()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        _ = (await client.PostAsync("/Users", CreateRequest))
            .StatusCode.Should()
            .Be(HttpStatusCode.Created);
        var errorResponse = await ToErrorResponse(
            client.PostAsync("/Users", CreateRequest5),
            HttpStatusCode.Conflict
        );

        // Assert
        _ = errorResponse.Message.Should().Be("User creation Failed : DuplicateEmail");
    }

    [Test]
    public async Task EmployeeNrMustBeUnique()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        _ = (await client.PostAsync("/Users", CreateRequest))
            .StatusCode.Should()
            .Be(HttpStatusCode.Created);
        var errorResponse = await ToErrorResponse(
            client.PostAsync("/Users", CreateRequest3),
            HttpStatusCode.Conflict
        );

        // Assert
        _ = errorResponse.Message.Should().Be("User creation Failed : DuplicateEmployeeNumber");
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
        _ = errorResponse
            .Message.Should()
            .Be(
                "Invalid password format: Passwords must have at least one non alphanumeric character. Passwords must have at least one digit ('0'-'9'). Passwords must have at least one uppercase ('A'-'Z')."
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

        _ = errorResponse.Message.Should().Be("User not authenticated.");
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
        _ = errorResponse.Message.Should().Be("The user 10 was not found.");
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
