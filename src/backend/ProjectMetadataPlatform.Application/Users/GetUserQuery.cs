using System.Collections.Generic;
using MediatR;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Query to get a user by his employee number.
/// </summary>
/// <param name="EmployeeId">Employee Number of the user.</param>
public record GetUserQuery(string EmployeeId)
    : IRequest<(ApplicationUser, IEnumerable<AuthorizationConstants.Actions>)>;
