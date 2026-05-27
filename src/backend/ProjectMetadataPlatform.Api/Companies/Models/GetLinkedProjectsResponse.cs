using System.Collections.Generic;
namespace ProjectMetadataPlatform.Api.Companies.Models;
public record GetLinkedProjectsResponse(List<string> projectSlugs);
