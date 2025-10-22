using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ProjectMetadataPlatform.Application.Authorization.Models;

/// <summary>
/// Class representing the environment of a request.
/// </summary>
public class AuthorizationEnvironment
{
    /// <summary>
    /// Time of the request.
    /// </summary>
    public DateTimeOffset DateTime { get; }

    private AuthorizationEnvironment(DateTimeOffset dateTime)
    {
        DateTime = dateTime;
    }

    /// <summary>
    /// Creates a new Environment.
    /// </summary>
    /// <param name="timeProvider">Used to get the current time</param>
    /// <returns></returns>
    public static AuthorizationEnvironment CreateAuthorizationEnvironment(TimeProvider timeProvider)
    {
        return new AuthorizationEnvironment(timeProvider.GetUtcNow().ToLocalTime());
    }
}
