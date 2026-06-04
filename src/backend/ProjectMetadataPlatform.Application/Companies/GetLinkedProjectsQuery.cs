using System.Collections.Generic;
using MediatR;

namespace ProjectMetadataPlatform.Application.Companies;

/// <summary>
/// Query to get all Projects linked to a specified company.
/// </summary>
/// <param name="Id">Id of the company.</param>
public record GetLinkedProjectsQuery(int Id) : IRequest<List<string>>;
