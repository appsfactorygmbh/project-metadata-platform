using System.Collections.Generic;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Api.Companies.Models;

/// <summary>
/// Represents a Response containing a single Company.
/// </summary>
/// <param name="Id">Id of the Company</param>
/// <param name="CompanyName">Name of the Company.</param>
/// <param name="Permissions">Permissions on the Company.</param>
public record GetCompanyResponse(
    int Id,
    string CompanyName,
    List<AuthorizationConstants.Actions>? Permissions = null
);
