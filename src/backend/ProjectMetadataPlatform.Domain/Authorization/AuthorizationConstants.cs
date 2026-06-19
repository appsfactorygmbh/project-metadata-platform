using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Design;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.OfficeLocations;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Domain.Authorization;

public static class AuthorizationConstants
{
    public const string API_VERSION = "api.cerbos.dev/v1";
    public const string POLICY_VERSION = "default";

    public const string PRINCIPLE_USER = "User";
    public const string PRINCIPLE_TOKEN = "ApiToken";

    //TODO: Keep up to date with the API
    public enum Actions
    {
        GET,
        CREATE,
        EDIT,
        DELETE,
    }
}
