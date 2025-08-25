using System;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Information about the token descriptor.
/// </summary>
/// <param name="ValidIssuer">The valid issuer.</param>
/// <param name="ValidAudience">The valid audience.</param>
/// <param name="IssuerSigningKey">The issuer signing key.</param>
public record TokenDescriptorInformation(
    string ValidIssuer,
    string ValidAudience,
    string IssuerSigningKey
)
{
    /// <summary>
    /// Reads the token descriptor information from the corresponding environment variables.
    /// </summary>
    /// <returns>The information read from the env.</returns>
    /// <exception cref="InvalidOperationException">If not all necessary environment variables are set.</exception>
    public static TokenDescriptorInformation ReadFromEnvVariables()
    {
        var validIssuer = EnvironmentUtils.GetEnvVarOrLoadFromFile("JWT_VALID_ISSUER");
        var validAudience = EnvironmentUtils.GetEnvVarOrLoadFromFile("JWT_VALID_AUDIENCE");
        var issuerSigningKey = EnvironmentUtils.GetEnvVarOrLoadFromFile("JWT_ISSUER_SIGNING_KEY");

        return new TokenDescriptorInformation(validIssuer, validAudience, issuerSigningKey);
    }
}
