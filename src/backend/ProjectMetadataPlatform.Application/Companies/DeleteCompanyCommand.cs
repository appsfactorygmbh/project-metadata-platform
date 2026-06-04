using MediatR;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Command to delete a Company.
/// </summary>
/// <param name="Id">Id of the company.</param>
public record DeleteCompanyCommand(int Id) : IRequest;
