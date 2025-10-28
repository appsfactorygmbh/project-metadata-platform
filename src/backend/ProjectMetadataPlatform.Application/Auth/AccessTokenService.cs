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
    /// <param name="jobTitle">Comma Separated List of JobTitles of the User</param>
    /// <param name="department">Comma Separated List of Departments, Teams and BUs of the User</param>
    /// <param name="teamSupport">Comma Separated List of Teams the User is a Supporter on</param>
    /// <param name="company">Company of the User</param>
    /// <returns>access token value as a string</returns>
    public static string CreateAccessToken(
        string email = "",
        string jobTitle = "",
        string department = "",
        string teamSupport = "",
        string company = ""
    )
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
            Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Email, email),
                    new Claim("Department", department),
                    new Claim("JobTitle", jobTitle),
                    new Claim("TeamSupport", teamSupport),
                    new Claim("Company", company),
                ]
            ),
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
