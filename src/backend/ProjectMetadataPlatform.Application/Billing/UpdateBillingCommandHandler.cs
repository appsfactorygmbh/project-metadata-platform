using System;
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
/// Handler for <see cref="UpdateBillingCommand"/>
/// </summary>
public class UpdateBillingCommandHandler : IRequestHandler<UpdateBillingCommand, GlobalBilling>
{
    private readonly IBillingRepository _billingRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates a new Instance of <see cref="UpdateBillingCommandHandler"/>
    /// </summary>
    /// <param name="billingRepository"></param>
    /// <param name="logRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="authorizationService"></param>
    public UpdateBillingCommandHandler(
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
    /// Handles request to update global billing info.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="BillingKindAlreadyExistsException"></exception>
    public async Task<GlobalBilling> Handle(
        UpdateBillingCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var billing = await _billingRepository.GetBillingByIdAsync(request.BillingId);
        await CheckAuthorization(billing, request);
        if (
            !string.Equals(
                billing.BillingKind,
                request.BillingKind,
                StringComparison.OrdinalIgnoreCase
            ) && await _billingRepository.CheckBillingKindExists(request.BillingKind)
        )
        {
            throw new BillingKindAlreadyExistsException(request.BillingKind);
        }
        var changes = new List<LogChange>();
        if (request.BillingKind != billing.BillingKind)
        {
            changes.Add(
                new LogChange
                {
                    Property = nameof(GlobalBilling.BillingKind),
                    OldValue = billing.BillingKind,
                    NewValue = request.BillingKind,
                }
            );
            billing.BillingKind = request.BillingKind;
        }
        if (request.Currency != billing.Currency)
        {
            changes.Add(
                new LogChange
                {
                    Property = nameof(GlobalBilling.Currency),
                    OldValue = billing.Currency ?? "null",
                    NewValue = request.Currency ?? "null",
                }
            );
            billing.Currency = request.Currency;
        }
        if (request.BudgetLimit != billing.BudgetLimit)
        {
            changes.Add(
                new LogChange
                {
                    Property = nameof(GlobalBilling.BudgetLimit),
                    OldValue =
                        billing.BudgetLimit?.ToString(CultureInfo.InvariantCulture) ?? "null",
                    NewValue =
                        request.BudgetLimit?.ToString(CultureInfo.InvariantCulture) ?? "null",
                }
            );
            billing.BudgetLimit = request.BudgetLimit;
        }
        if (request.HostingFee != billing.HostingFee)
        {
            changes.Add(
                new LogChange
                {
                    Property = nameof(GlobalBilling.HostingFee),
                    OldValue = billing.HostingFee?.ToString(CultureInfo.InvariantCulture) ?? "null",
                    NewValue = request.HostingFee?.ToString(CultureInfo.InvariantCulture) ?? "null",
                }
            );
            billing.HostingFee = request.HostingFee;
        }
        if (request.TargetMargin != billing.TargetMargin)
        {
            changes.Add(
                new LogChange
                {
                    Property = nameof(GlobalBilling.TargetMargin),
                    OldValue =
                        billing.TargetMargin?.ToString(CultureInfo.InvariantCulture) ?? "null",
                    NewValue =
                        request.TargetMargin?.ToString(CultureInfo.InvariantCulture) ?? "null",
                }
            );
            billing.TargetMargin = request.TargetMargin;
        }
        if (request.TimeFrame != billing.TimeFrame)
        {
            changes.Add(
                new LogChange
                {
                    Property = nameof(GlobalBilling.TimeFrame),
                    OldValue = billing.TimeFrame?.ToString() ?? "null",
                    NewValue = request.TimeFrame?.ToString() ?? "null",
                }
            );
            billing.TimeFrame = request.TimeFrame;
        }

        var updatedBilling = await _billingRepository.StoreBillingInformation(billing);
        if (changes.Count > 0)
        {
            await _logRepository.AddGlobalBillingLogForCurrentActor(
                billing,
                Domain.Logs.Action.UPDATED_GLOBAL_BILLING,
                changes
            );
        }
        await _unitOfWork.CompleteAsync();

        return updatedBilling;
    }

    private async Task CheckAuthorization(GlobalBilling billing, UpdateBillingCommand request)
    {
        Dictionary<string, object?> updates = [];
        if (request.BillingKind != billing.BillingKind)
        {
            updates.Add(nameof(GlobalBilling.BillingKind), request.BillingKind);
        }
        if (request.Currency != billing.Currency)
        {
            updates.Add(nameof(GlobalBilling.Currency), request.Currency);
        }
        if (request.BudgetLimit != billing.BudgetLimit)
        {
            updates.Add(nameof(GlobalBilling.BudgetLimit), request.BudgetLimit);
        }
        if (request.HostingFee != billing.HostingFee)
        {
            updates.Add(nameof(GlobalBilling.HostingFee), request.HostingFee);
        }
        if (request.TargetMargin != billing.TargetMargin)
        {
            updates.Add(nameof(GlobalBilling.TargetMargin), request.TargetMargin);
        }
        if (request.TimeFrame != billing.TimeFrame)
        {
            updates.Add(nameof(GlobalBilling.TimeFrame), request.TimeFrame);
        }
        if (
            !await _authorizationService.CheckAccess(
                billing,
                AuthorizationConstants.Actions.EDIT,
                updates
            )
        )
        {
            throw new UnauthorizedException();
        }
    }
}
