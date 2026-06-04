using System.Collections.Generic;

namespace ProjectMetadataPlatform.Api.Companies.Models;

/// <summary>
/// Represents a Response containing slugs of projects linked to a id.
/// </summary>
/// <param name="ProjectSlugs">List of Project Slugs.</param>
public record GetLinkedProjectsForCompanyResponse(List<string> ProjectSlugs);
