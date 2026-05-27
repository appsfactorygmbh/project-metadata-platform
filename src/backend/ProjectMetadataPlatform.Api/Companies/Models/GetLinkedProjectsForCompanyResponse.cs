using System.Collections.Generic;

namespace ProjectMetadataPlatform.Api.Companies.Models;

public record GetLinkedProjectsForCompanyResponse(List<string> projectSlugs);
