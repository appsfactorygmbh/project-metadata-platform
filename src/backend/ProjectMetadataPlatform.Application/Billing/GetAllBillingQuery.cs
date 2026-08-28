using System.Collections.Generic;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Application.Billing;

/// <summary>
/// Request to return all global billing objects.
/// </summary>
public record GetAllBillingQuery()
    : IRequest<(IEnumerable<GlobalBilling>, IEnumerable<AuthorizationConstants.Actions>)>;
