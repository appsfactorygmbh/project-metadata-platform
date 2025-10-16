using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ProjectMetadataPlatform.Application.Authorization.Models;

public class AuthorizationEnvironment
{
    public DateTimeOffset DateTime { get; }

    private AuthorizationEnvironment(DateTimeOffset dateTime)
    {
        DateTime = dateTime;
    }

    public static AuthorizationEnvironment CreateAuthorizationEnvironment(TimeProvider timeProvider)
    {
        return new AuthorizationEnvironment(timeProvider.GetUtcNow().ToLocalTime());
    }
}
