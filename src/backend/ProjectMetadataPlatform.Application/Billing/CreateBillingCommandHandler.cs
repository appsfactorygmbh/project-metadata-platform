using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Billing;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.BillingExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Billing;

/// <summary>
/// Handler for <see cref="CreateBillingCommand"/>
/// </summary>
public class CreateBillingCommandHandler : IRequestHandler<CreateBillingCommand, int>
{
    private readonly IBillingRepository _billingRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new instance of<see cref="CreateBillingCommandHandler" />.
    /// </summary>
    /// <param name="billingRepository">The repository for managing billing.</param>
    /// <param name="logRepository">The repository for managing logs.</param>
    /// <param name="unitOfWork">The unit of work for managing transactions.</param>
    /// <param name="authorizationService"></param>
    public CreateBillingCommandHandler(
        IBillingRepository billingRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService
    )
    {
        _billingRepository = billingRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Request to create new Billing information.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    /// <exception cref="BillingKindAlreadyExistsException"></exception>
    public async Task<int> Handle(
        CreateBillingCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var billing = new GlobalBilling
        {
            BillingKind = request.BillingKind,
            Currency = request.Currency,
            BudgetLimit = request.BudgetLimit,
            HostingFee = request.HostingFee,
            TargetMargin = request.TargetMargin,
            TimeFrame = request.TimeFrame,
        };
        if (
            !await _authorizationService.CheckAccess(billing, AuthorizationConstants.Actions.CREATE)
        )
        {
            throw new UnauthorizedException();
        }

        if (await _billingRepository.CheckBillingKindExists(billing.BillingKind))
        {
            throw new BillingKindAlreadyExistsException(billing.BillingKind);
        }

        await AddCreatedBillingLog(billing, request);

        _ = await _billingRepository.StoreBillingInformation(billing);
        await _unitOfWork.CompleteAsync();

        return billing.Id;
    }

    private async Task AddCreatedBillingLog(GlobalBilling billing, CreateBillingCommand request)
    {
        var logChanges = new List<LogChange>
        {
            new()
            {
                Property = nameof(GlobalBilling.BillingKind),
                OldValue = "",
                NewValue = request.BillingKind,
            },
        };
        if (request.Currency != null)
        {
            logChanges.Add(
                new()
                {
                    Property = nameof(GlobalBilling.Currency),
                    OldValue = "",
                    NewValue = request.Currency,
                }
            );
        }
        if (request.BudgetLimit.HasValue)
        {
            logChanges.Add(
                new()
                {
                    Property = nameof(GlobalBilling.BudgetLimit),
                    OldValue = "",
                    NewValue = request.BudgetLimit.Value.ToString(CultureInfo.InvariantCulture),
                }
            );
        }
        if (request.HostingFee.HasValue)
        {
            logChanges.Add(
                new()
                {
                    Property = nameof(GlobalBilling.HostingFee),
                    OldValue = "",
                    NewValue = request.HostingFee.Value.ToString(CultureInfo.InvariantCulture),
                }
            );
        }
        if (request.TargetMargin.HasValue)
        {
            logChanges.Add(
                new()
                {
                    Property = nameof(GlobalBilling.TargetMargin),
                    OldValue = "",
                    NewValue = request.TargetMargin.Value.ToString(CultureInfo.InvariantCulture),
                }
            );
        }
        if (request.TimeFrame.HasValue)
        {
            logChanges.Add(
                new()
                {
                    Property = nameof(GlobalBilling.TimeFrame),
                    OldValue = "",
                    NewValue = request.TimeFrame.Value.ToString(),
                }
            );
        }
        await _logRepository.AddGlobalBillingLogForCurrentActor(
            billing,
            Action.ADDED_GLOBAL_BILLING,
            logChanges
        );
    }
}
