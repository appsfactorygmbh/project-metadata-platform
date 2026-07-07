using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

[NonParallelizable]
public class AuthManagement : IntegrationTestsBase
{
    [Test]
    public async Task ObtainNewAuthTokenFromRefreshToken()
    {
        //Arrange
        var client = CreateClient();
        var loginResponse = await ToJsonElement(
            client.PostAsJsonAsync(
                "/auth/basic",
                new { Email = "admin@admin.admin", Password = "admin" }
            )
        );
        var firstAuthToken = loginResponse.GetProperty("accessToken").GetString();
        var refreshToken = loginResponse.GetProperty("refreshToken").GetString();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Refresh {refreshToken}");

        //Act
        var response = await ToJsonElement(client.GetAsync("/auth/refresh"));

        //Assert
        var newAuthToken = response.GetProperty("accessToken").GetString();

        Assert.That(newAuthToken, Is.Not.SameAs(firstAuthToken));

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {newAuthToken}");

        var getResponse = await client.GetAsync("/Projects");

        Assert.That(getResponse.IsSuccessStatusCode, Is.True);
    }

    [Test]
    public async Task ExpiredAccessTokenDoesNotWork()
    {
        //Arrange
        Environment.SetEnvironmentVariable("ACCESS_TOKEN_EXPIRATION_MINUTES", "0.05");
        Environment.SetEnvironmentVariable("PMP_JWT_CLOCK_SKEW_SECONDS", "0");

        var client = CreateClient();
        var loginResponse = await ToJsonElement(
            client.PostAsJsonAsync(
                "/auth/basic",
                new { Email = "admin@admin.admin", Password = "admin" }
            )
        );
        var accessToken = loginResponse.GetProperty("accessToken").GetString();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        //Act
        await Task.Delay(TimeSpan.FromSeconds(3));
        var response = await client.GetAsync("/Projects");

        //Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task ExpiredRefreshTokenDoesNotWork()
    {
        //Arrange
        Environment.SetEnvironmentVariable("REFRESH_TOKEN_EXPIRATION_HOURS", "0");

        var client = CreateClient();
        var loginResponse = await ToJsonElement(
            client.PostAsJsonAsync(
                "/auth/basic",
                new { Email = "admin@admin.admin", Password = "admin" }
            )
        );
        var refreshToken = loginResponse.GetProperty("refreshToken").GetString();

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Refresh {refreshToken}");

        //Act
        var response = await client.GetAsync("/auth/refresh");

        //Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task InvalidCredentialsAreNotAccepted()
    {
        //Arrange
        var client = CreateClient();

        //Act
        var response = await ToErrorResponse(
            client.PostAsJsonAsync(
                "/auth/basic",
                new { Email = "wrong@email.de", Password = "invalid" }
            ),
            HttpStatusCode.BadRequest
        );

        //Assert
        Assert.That(response.Message, Is.EqualTo("Invalid login credentials."));
    }

    [Test]
    public async Task InvalidRefreshTokenIsNotAccepted()
    {
        //Arrange
        var client = CreateClient();
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", "Refresh invalid");

        //Act
        var response = await ToErrorResponse(
            client.GetAsync("/auth/refresh"),
            HttpStatusCode.BadRequest
        );

        //Assert
        Assert.That(response.Message, Is.EqualTo("Invalid refresh token."));
    }

    [Test]
    public async Task DeletedApiTokenIsNotAccepted()
    {
        var client = CreateClient();

        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var tokenValue = (
            await ToJsonElement(
                client.PostAsJsonAsync(
                    "/auth/ApiTokens",
                    new { Name = "Token", Scopes = new string[] { "GET_PROJECT" } }
                ),
                HttpStatusCode.Created
            )
        )
            .GetProperty("token")
            .GetString();
        var tokens = await ToJsonElement(client.GetAsync("auth/ApiTokens"), HttpStatusCode.OK);
        Assert.That(tokens.GetArrayLength(), Is.EqualTo(1));

        var tokenId = tokens[0].GetProperty("id").GetInt32();

        var deleteResponse = await client.DeleteAsync($"auth/ApiTokens/{tokenId}");

        Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenValue}");
        var error = await client.GetAsync("Projects");

        Assert.That(error.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
    }

    [Test]
    public async Task OldApiTokenAfterRegenerationIsNotAccepted()
    {
        var client = CreateClient();
        await GetAuthTokenAndAddItToDefaultRequestHeadersOfClient(client);
        var tokenValue = (
            await ToJsonElement(
                client.PostAsJsonAsync(
                    "/auth/ApiTokens",
                    new { Name = "Token", Scopes = new string[] { "GET_PROJECT" } }
                ),
                HttpStatusCode.Created
            )
        )
            .GetProperty("token")
            .GetString();

        var tokens = await ToJsonElement(client.GetAsync("auth/ApiTokens"), HttpStatusCode.OK);
        Assert.That(tokens.GetArrayLength(), Is.EqualTo(1));

        var tokenId = tokens[0].GetProperty("id").GetInt32();

        var token = await ToJsonElement(client.PatchAsync($"auth/ApiTokens/{tokenId}", null));
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenValue}");
        var error = await client.GetAsync("Projects");

        Assert.That(error.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.GetProperty("token")}");
        _ = await ToJsonElement(client.GetAsync("Projects"), HttpStatusCode.OK);
    }
}
