using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Custom Authentication Handler for Auth via ApiToken
/// </summary>
public class ApiTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiTokenRepository _apiTokenRepository;

    /// <summary>
    /// Consructor for ApiTokenAuthenticationHandler.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="encoder"></param>
    /// <param name="apiTokenRepository"></param>
    public ApiTokenAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        UrlEncoder encoder,
        IApiTokenRepository apiTokenRepository
    )
        : base(options, new LoggerFactory(), encoder)
    {
        _apiTokenRepository = apiTokenRepository;
    }

    /// <summary>
    /// Method for Handling Authentication via Api Token.
    /// </summary>
    /// <returns>Authentication Result</returns>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
            return AuthenticateResult.Fail("Missing Authorization Header");

        if (!authHeader.ToString().StartsWith("Bearer "))
            return AuthenticateResult.Fail("Invalid Scheme");

        var tokenString = authHeader.ToString().Replace("Bearer ", "");

        var verifiedToken = await _apiTokenRepository.GetVerifiedToken(tokenString);

        if (verifiedToken == null || verifiedToken.ExpirationDate.CompareTo(DateTime.Now) <= 0)
        {
            return AuthenticateResult.Fail("Invalid Token");
        }
        var claims = new[]
        {
            new Claim(ClaimTypes.AuthenticationMethod, "API Token"),
            new Claim(ClaimTypes.Name, verifiedToken.Name),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
