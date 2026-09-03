using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Billing;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Billing;

/// <summary>
/// Handler for the <see cref="DeleteBillingCommand" />.
/// </summary>
public class DeleteBillingCommandHandler : IRequestHandler<DeleteBillingCommand>
{
    private readonly IBillingRepository _billingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogRepository _logRepository;

    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    ///  Creates a new instance of <see cref="DeleteBillingCommandHandler" />.
    /// </summary>
    /// <param name="billingRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="logRepository"></param>
    /// <param name="authorizationService"></param>
    public DeleteBillingCommandHandler(
        IBillingRepository billingRepository,
        IUnitOfWork unitOfWork,
        ILogRepository logRepository,
        IAuthorizationService authorizationService
    )
    {
        _billingRepository = billingRepository;
        _unitOfWork = unitOfWork;
        _logRepository = logRepository;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Deletes billing information from the database.
    /// </summary>
    /// <param name="request">request that is handled.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Handle(DeleteBillingCommand request, CancellationToken cancellationToken)
    {
        var token = await _billingRepository.GetBillingByIdAsync(request.Id);
        if (!await _authorizationService.CheckAccess(token, AuthorizationConstants.Actions.DELETE))
        {
            throw new UnauthorizedException();
        }
        await _billingRepository.DeleteBillingAsync(token);
        await AddDeleteBillingLog(token);
        await _unitOfWork.CompleteAsync();
    }

    private async Task AddDeleteBillingLog(GlobalBilling billing)
    {
        var logChanges = new List<LogChange>
        {
            new()
            {
                Property = nameof(GlobalBilling.BillingKind),
                NewValue = "",
                OldValue = billing.BillingKind,
            },
        };
        if (billing.Currency != null)
        {
            logChanges.Add(
                new()
                {
                    Property = nameof(GlobalBilling.Currency),
                    NewValue = "",
                    OldValue = billing.Currency,
                }
            );
        }
        if (billing.BudgetLimit.HasValue)
        {
            logChanges.Add(
                new()
                {
                    Property = nameof(GlobalBilling.BudgetLimit),
                    NewValue = "",
                    OldValue = billing.BudgetLimit.Value.ToString(CultureInfo.InvariantCulture),
                }
            );
        }
        if (billing.HostingFee.HasValue)
        {
            logChanges.Add(
                new()
                {
                    Property = nameof(GlobalBilling.HostingFee),
                    NewValue = "",
                    OldValue = billing.HostingFee.Value.ToString(CultureInfo.InvariantCulture),
                }
            );
        }
        if (billing.TargetMargin.HasValue)
        {
            logChanges.Add(
                new()
                {
                    Property = nameof(GlobalBilling.TargetMargin),
                    NewValue = "",
                    OldValue = billing.TargetMargin.Value.ToString(CultureInfo.InvariantCulture),
                }
            );
        }
        if (billing.TimeFrame.HasValue)
        {
            logChanges.Add(
                new()
                {
                    Property = nameof(GlobalBilling.TimeFrame),
                    NewValue = "",
                    OldValue = billing.TimeFrame.Value.ToString(),
                }
            );
        }
        await _logRepository.AddGlobalBillingLogForCurrentActor(
            billing,
            Domain.Logs.Action.REMOVED_GLOBAL_BILLING,
            logChanges
        );
    }
}
