using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using ProjectMetadataPlatform.IntegrationTests.Utilities;

namespace ProjectMetadataPlatform.IntegrationTests;

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

        newAuthToken.Should().NotBeSameAs(firstAuthToken);

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {newAuthToken}");
        (await client.GetAsync("/Projects"))
            .IsSuccessStatusCode.Should()
            .BeTrue(" the new access token should be valid");
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
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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
        response.Message.Should().Be("Invalid login credentials.");
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
        response.Message.Should().Be("Invalid refresh token.");
    }
}
