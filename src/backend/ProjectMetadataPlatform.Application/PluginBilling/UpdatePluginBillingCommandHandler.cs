using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.BillingExceptions;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.PluginBilling;

/// <summary>
/// Handler for <see cref="UpdatePluginBillingCommand"/>
/// </summary>
public class UpdatePluginBillingCommandHandler
    : IRequestHandler<UpdatePluginBillingCommand, Domain.Billing.PluginBilling>
{
    private readonly IBillingRepository _billingRepository;

    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates new Instance of <see cref="UpdatePluginBillingCommandHandler"/>
    /// </summary>
    /// <param name="billingRepository"></param>
    /// <param name="logRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="authorizationService"></param>
    public UpdatePluginBillingCommandHandler(
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
    /// Handles request to add new plugin billing.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="PluginBillingDateMissingException"></exception>
    /// <exception cref="PluginBillingNotesSizeException"></exception>
    public async Task<Domain.Billing.PluginBilling> Handle(
        UpdatePluginBillingCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var billing = await _billingRepository.GetPluginBillingByIdAsync(
            request.ProjectId,
            request.PluginId
        );

        await CheckAuthorization(billing, request);

        if (request.TimeFrame == Domain.Billing.TimeFrame.DATE && request.Date == null)
        {
            throw new PluginBillingDateMissingException();
        }

        var notesInfo = new StringInfo(request.Notes ?? "");
        if (notesInfo.LengthInTextElements > 280)
        {
            throw new PluginBillingNotesSizeException(notesInfo.LengthInTextElements);
        }
        var changes = new List<LogChange>();
        if (request.DisplayName != billing.DisplayName)
        {
            changes.Add(
                new()
                {
                    OldValue = billing.DisplayName ?? "null",
                    NewValue = request.DisplayName ?? "null",
                    Property = nameof(Domain.Billing.PluginBilling.DisplayName),
                }
            );
            billing.DisplayName = request.DisplayName;
        }
        if (request.BudgetLimit != billing.BudgetLimit)
        {
            changes.Add(
                new()
                {
                    OldValue = billing.BudgetLimit.ToString(CultureInfo.InvariantCulture),
                    NewValue = request.BudgetLimit.ToString(CultureInfo.InvariantCulture),
                    Property = nameof(Domain.Billing.PluginBilling.BudgetLimit),
                }
            );
            billing.BudgetLimit = request.BudgetLimit;
        }

        if (request.HostingFee != billing.HostingFee)
        {
            changes.Add(
                new()
                {
                    OldValue = billing.HostingFee.ToString(CultureInfo.InvariantCulture),
                    NewValue = request.HostingFee.ToString(CultureInfo.InvariantCulture),
                    Property = nameof(Domain.Billing.PluginBilling.HostingFee),
                }
            );
            billing.HostingFee = request.HostingFee;
        }
        if (request.Currency != billing.Currency)
        {
            changes.Add(
                new()
                {
                    OldValue = billing.Currency,
                    NewValue = request.Currency,
                    Property = nameof(Domain.Billing.PluginBilling.Currency),
                }
            );
            billing.Currency = request.Currency;
        }
        if (request.TargetMargin != billing.TargetMargin)
        {
            changes.Add(
                new()
                {
                    OldValue = billing.TargetMargin.ToString(CultureInfo.InvariantCulture),
                    NewValue = request.TargetMargin.ToString(CultureInfo.InvariantCulture),
                    Property = nameof(Domain.Billing.PluginBilling.TargetMargin),
                }
            );
            billing.TargetMargin = request.TargetMargin;
        }
        if (request.TimeFrame != billing.TimeFrame || request.Date != billing.Date)
        {
            changes.Add(
                new()
                {
                    OldValue =
                        billing.TimeFrame == Domain.Billing.TimeFrame.DATE
                            ? billing.Date?.Date.ToString(new CultureInfo("de-DE"))!
                            : billing.TimeFrame.ToString(),
                    NewValue =
                        request.TimeFrame == Domain.Billing.TimeFrame.DATE
                            ? request.Date?.Date.ToString(new CultureInfo("de-DE"))!
                            : request.TimeFrame.ToString(),
                    Property = nameof(Domain.Billing.PluginBilling.TimeFrame),
                }
            );
            billing.TimeFrame = request.TimeFrame;
            billing.Date = request.TimeFrame == Domain.Billing.TimeFrame.DATE ? request.Date : null;
        }
        if (request.Notes != billing.Notes)
        {
            changes.Add(
                new()
                {
                    OldValue =
                        (billing.Notes ?? "").Length > 50
                            ? billing.Notes![0..50] + "..."
                            : (billing.Notes ?? "null"),
                    NewValue = request.Notes ?? "null",
                    Property = nameof(Domain.Billing.PluginBilling.Notes),
                }
            );
            billing.Notes = request.Notes;
        }

        if (changes.Count > 0)
        {
            await _logRepository.AddProjectLogForCurrentActor(
                billing.ProjectPlugin?.Project!,
                Action.UPDATED_PROJECT_PLUGIN_BILLING,
                changes
            );
        }
        await _billingRepository.UpdatePluginBilling(billing);
        await _unitOfWork.CompleteAsync();
        return billing;
    }

    private async Task CheckAuthorization(
        Domain.Billing.PluginBilling billing,
        UpdatePluginBillingCommand request
    )
    {
        Dictionary<string, object?> updates = [];
        if (request.DisplayName != billing.DisplayName)
        {
            updates.Add(nameof(Domain.Billing.PluginBilling.DisplayName), request.DisplayName);
        }
        if (request.Currency != billing.Currency)
        {
            updates.Add(nameof(Domain.Billing.PluginBilling.Currency), request.Currency);
        }
        if (request.BudgetLimit != billing.BudgetLimit)
        {
            updates.Add(nameof(Domain.Billing.PluginBilling.BudgetLimit), request.BudgetLimit);
        }
        if (request.HostingFee != billing.HostingFee)
        {
            updates.Add(nameof(Domain.Billing.PluginBilling.HostingFee), request.HostingFee);
        }
        if (request.TargetMargin != billing.TargetMargin)
        {
            updates.Add(nameof(Domain.Billing.PluginBilling.TargetMargin), request.TargetMargin);
        }
        if (request.TimeFrame != billing.TimeFrame)
        {
            updates.Add(nameof(Domain.Billing.PluginBilling.TimeFrame), request.TimeFrame);
        }
        if (request.Date != billing.Date)
        {
            updates.Add(nameof(Domain.Billing.PluginBilling.Date), request.Date);
        }
        if (request.Notes != billing.Notes)
        {
            updates.Add(nameof(Domain.Billing.PluginBilling.Notes), request.Notes);
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
