using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Application.Billing;

/// <summary>
/// Request to return global billing information by id.
/// </summary>
/// <param name="Id">Id of the billing information.</param>
public record GetBillingByIdQuery(int Id)
    : IRequest<(GlobalBilling, IEnumerable<AuthorizationConstants.Actions>)>;
