using MediatR;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Command  for creating a new Company.
/// </summary>
/// <param name="CompanyName">Name of the Company.</param>
public record CreateCompanyCommand(string CompanyName) : IRequest<int>;
