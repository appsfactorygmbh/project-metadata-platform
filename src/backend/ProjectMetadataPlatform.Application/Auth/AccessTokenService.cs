using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Service for creating access tokens.
/// </summary>
public static class AccessTokenService
{
    /// <summary>
    /// Creates an access token for the given username.
    /// </summary>
    /// <param name="email">Email for user the access token belongs to</param>
    /// <returns>access token value as a string</returns>
    public static string CreateAccessToken(string email)
    {
        var tokenDescriptorInformation = TokenDescriptorInformation.ReadFromEnvVariables();
        var issuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(tokenDescriptorInformation.IssuerSigningKey)
        );
        var expirationTime = double.Parse(
            EnvironmentUtils.GetEnvVarOrLoadFromFile("ACCESS_TOKEN_EXPIRATION_MINUTES"),
            CultureInfo.InvariantCulture
        );
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(ClaimTypes.Email, email)]),
            Expires = DateTime.UtcNow.AddMinutes(expirationTime),
            Issuer = tokenDescriptorInformation.ValidIssuer,
            Audience = tokenDescriptorInformation.ValidAudience,
            SigningCredentials = new SigningCredentials(
                issuerSigningKey,
                SecurityAlgorithms.HmacSha256Signature
            ),
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
