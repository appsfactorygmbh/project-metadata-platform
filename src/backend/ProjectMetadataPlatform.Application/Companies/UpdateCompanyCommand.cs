using MediatR;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Command to Update a Company.
/// </summary>
/// <param name="Id">Id of the Company.</param>
/// <param name="CompanyName">New Name for the Company.</param>
public record UpdateCompanyCommand(int Id, string? CompanyName = null) : IRequest<Company>;
