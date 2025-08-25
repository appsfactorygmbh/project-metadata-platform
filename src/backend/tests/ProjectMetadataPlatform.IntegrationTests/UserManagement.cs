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
        """{ "email": "test@mail.de", "password": "1K@sekuchen" }"""
    );
    private static readonly StringContent CreateRequest2 = StringContent(
        """{ "email": "foo@bar.de", "password": "SecretP@ssw0rd" }"""
    );

    [Test]
    public async Task UserCantDeleteThemself()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        // Assert
        var adminId = (await ToJsonElement(client.GetAsync("/Users")))[0]
            .GetProperty("id")
            .GetString()!;

        var deleteResponse = await client.DeleteAsync($"/Users/{adminId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await deleteResponse.Content.ReadFromJsonAsync<ErrorResponse>())!
            .Message.Should()
            .Be("A User can't delete themself.");

        var newUserId = (
            await ToJsonElement(client.PutAsync("/Users", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("userId")
            .GetString()!;

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(
            client,
            "test@mail.de",
            "1K@sekuchen"
        );
        deleteResponse = await client.DeleteAsync($"/Users/{newUserId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await deleteResponse.Content.ReadFromJsonAsync<ErrorResponse>())!
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
            await ToJsonElement(client.PutAsync("/Users", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("userId")
            .GetString()!;
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(
            client,
            "test@mail.de",
            "1K@sekuchen"
        );
        var userId2 = (
            await ToJsonElement(client.PutAsync("/Users", CreateRequest2), HttpStatusCode.Created)
        )
            .GetProperty("userId")
            .GetString()!;

        var users = await ToJsonElement(client.GetAsync("/Users"));

        users.GetArrayLength().Should().Be(3);
        users[0].GetProperty("email").GetString().Should().Be("admin@admin.admin");
        users[1].GetProperty("id").GetString().Should().Be(userId1);
        users[1].GetProperty("email").GetString().Should().Be("test@mail.de");
        users[2].GetProperty("id").GetString().Should().Be(userId2);
        users[2].GetProperty("email").GetString().Should().Be("foo@bar.de");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        logs.GetArrayLength().Should().Be(2);

        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin added a new user with properties: Email = test@mail.de");
        logs[0]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("test added a new user with properties: Email = foo@bar.de");
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
            await ToJsonElement(client.PutAsync("/Users", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("userId")
            .GetString()!;

        var updatedUser = await ToJsonElement(
            client.PatchAsync($"/Users/{userId}", CreateRequest2)
        );

        updatedUser.GetProperty("id").GetString().Should().Be(userId);
        updatedUser.GetProperty("email").GetString().Should().Be("foo@bar.de");

        var users = await ToJsonElement(client.GetAsync("/Users"));

        users.GetArrayLength().Should().Be(2);
        users[0].GetProperty("email").GetString().Should().Be("admin@admin.admin");
        users[1].GetProperty("id").GetString().Should().Be(userId);
        users[1].GetProperty("email").GetString().Should().Be("foo@bar.de");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));
        logs.GetArrayLength().Should().Be(2);

        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin added a new user with properties: Email = test@mail.de");

        logs[0]
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
        var adminId = (await ToJsonElement(client.GetAsync("/Users")))[0]
            .GetProperty("id")
            .GetString()!;
        var userId1 = (
            await ToJsonElement(client.PutAsync("/Users", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("userId")
            .GetString()!;

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(
            client,
            "test@mail.de",
            "1K@sekuchen"
        );
        var userId2 = (
            await ToJsonElement(client.PutAsync("/Users", CreateRequest2), HttpStatusCode.Created)
        )
            .GetProperty("userId")
            .GetString()!;

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(
            client,
            "foo@bar.de",
            "SecretP@ssw0rd"
        );
        (await client.DeleteAsync($"Users/{userId1}"))
            .StatusCode.Should()
            .Be(HttpStatusCode.NoContent);

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        (await client.DeleteAsync($"Users/{userId2}"))
            .StatusCode.Should()
            .Be(HttpStatusCode.NoContent);

        var users = await ToJsonElement(client.GetAsync("/Users"));

        users.GetArrayLength().Should().Be(1);
        users[0].GetProperty("id").GetString().Should().Be(adminId);
        users[0].GetProperty("email").GetString().Should().Be("admin@admin.admin");

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        logs.GetArrayLength().Should().Be(4);

        logs[3]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin added a new user with properties: Email = test@mail.de");
        logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("test (deleted user) added a new user with properties: Email = foo@bar.de");
        logs[1]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("foo (deleted user) removed user test@mail.de");
        logs[0].GetProperty("logMessage").GetString().Should().Be("admin removed user foo@bar.de");
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
            await ToJsonElement(client.PutAsync("/Users", CreateRequest), HttpStatusCode.Created)
        )
            .GetProperty("userId")
            .GetString()!;

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(
            client,
            "test@mail.de",
            "1K@sekuchen"
        );

        (
            await client.PutAsync(
                "/Users",
                StringContent("""{ "email": "mail@m.de", "password": "1K@sekuchen" }""")
            )
        )
            .StatusCode.Should()
            .Be(HttpStatusCode.Created);

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        (await client.PatchAsync($"/Users/{userId}", CreateRequest2))
            .StatusCode.Should()
            .Be(HttpStatusCode.OK);

        (await client.DeleteAsync($"Users/{userId}"))
            .StatusCode.Should()
            .Be(HttpStatusCode.NoContent);

        var logs = await ToJsonElement(client.GetAsync("/Logs"));

        logs[3]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("admin added a new user with properties: Email = test@mail.de");
        logs[2]
            .GetProperty("logMessage")
            .GetString()
            .Should()
            .Be("test (deleted user) added a new user with properties: Email = mail@m.de");
    }

    [Test]
    public async Task EmailMustBeUnique()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        (await client.PutAsync("/Users", CreateRequest))
            .StatusCode.Should()
            .Be(HttpStatusCode.Created);
        var errorResponse = await ToErrorResponse(
            client.PutAsync("/Users", CreateRequest),
            HttpStatusCode.Conflict
        );

        // Assert
        errorResponse.Message.Should().Be("User creation Failed : DuplicateEmail");
    }

    [Test]
    public async Task PasswordMustBeValid()
    {
        // Arrange
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);

        // Act
        var errorResponse = await ToErrorResponse(
            client.PutAsync(
                "/Users",
                StringContent("""{ "email": "foo@bar.de", "password": "password" }""")
            ),
            HttpStatusCode.BadRequest
        );

        // Assert
        errorResponse
            .Message.Should()
            .Be(
                "Invalid password format: Passwords must have at least one non alphanumeric character. Passwords must have at least one digit ('0'-'9'). Passwords must have at least one uppercase ('A'-'Z')."
            );
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
        errorResponse.Message.Should().Be("The user 10 was not found.");
    }
}
