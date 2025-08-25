namespace ProjectMetadataPlatform.Api.Auth.Models;

/// <summary>
/// Response for logging in.
/// </summary>
/// <param name="AccessToken">jwt access token</param>
/// <param name="RefreshToken">jwt refresh token</param>
public record LoginResponse(string AccessToken, string RefreshToken);
