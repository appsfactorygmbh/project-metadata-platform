using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.BillingExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.PluginBilling;

/// <summary>
/// Handler for <see cref="AddPluginBillingCommand"/>
/// </summary>
public class AddPluginBillingCommandHandler : IRequestHandler<AddPluginBillingCommand, (int, int)>
{
    private readonly IBillingRepository _billingRepository;

    private readonly IPluginRepository _pluginRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Creates new Instance of <see cref="AddPluginBillingCommandHandler"/>
    /// </summary>
    /// <param name="billingRepository"></param>
    /// <param name="pluginRepository"></param>
    /// <param name="logRepository"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="authorizationService"></param>
    public AddPluginBillingCommandHandler(
        IBillingRepository billingRepository,
        IPluginRepository pluginRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork,
        IAuthorizationService authorizationService
    )
    {
        _billingRepository = billingRepository;
        _pluginRepository = pluginRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// Handles request for adding new Billing data.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="UnauthorizedException"></exception>
    /// <exception cref="PluginBillingAlreadyExistsException"></exception>
    /// <exception cref="PluginBillingDateMissingException"></exception>
    /// <exception cref="PluginBillingNotesSizeException"></exception>
    public async Task<(int, int)> Handle(
        AddPluginBillingCommand request,
        CancellationToken cancellationToken = default
    )
    {
        var plugin = await _pluginRepository.GetProjectPluginAsync(
            request.ProjectId,
            request.PluginId
        );
        var globalBilling = await _billingRepository.GetBillingByIdAsNoTrackingAsync(
            request.BillingId
        );

        var billing = new Domain.Billing.PluginBilling
        {
            ProjectId = request.ProjectId,
            PluginId = request.PluginId,
            BillingId = request.BillingId,
            DisplayName = request.DisplayName,
            Currency = request.Currency,
            BudgetLimit = request.BudgetLimit,
            HostingFee = request.HostingFee,
            TargetMargin = request.TargetMargin,
            TimeFrame = request.TimeFrame,
            Date = request.TimeFrame == Domain.Billing.TimeFrame.DATE ? request.Date : null,
            Notes = request.Notes,
            ProjectPlugin = plugin,
            GlobalBilling = globalBilling,
        };

        if (
            !await _authorizationService.CheckAccess(billing, AuthorizationConstants.Actions.CREATE)
        )
        {
            throw new UnauthorizedException();
        }

        billing.ProjectPlugin = null;
        billing.GlobalBilling = null;

        if (await _billingRepository.CheckPluginBillingExists(request.ProjectId, request.PluginId))
        {
            throw new PluginBillingAlreadyExistsException(billing.PluginId);
        }

        if (billing.TimeFrame == Domain.Billing.TimeFrame.DATE && billing.Date == null)
        {
            throw new PluginBillingDateMissingException();
        }

        var notesInfo = new StringInfo(billing.Notes ?? "");
        if (notesInfo.LengthInTextElements > 280)
        {
            throw new PluginBillingNotesSizeException(notesInfo.LengthInTextElements);
        }
        await _billingRepository.AddPluginBilling(billing);
        await AddPluginBillingLog(plugin, billing, globalBilling.BillingKind);

        await _unitOfWork.CompleteAsync();
        return (request.ProjectId, request.PluginId);
    }

    private async Task AddPluginBillingLog(
        ProjectPlugin plugin,
        Domain.Billing.PluginBilling billing,
        string globalBillingKind
    )
    {
        var billingChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "",
                NewValue = plugin.DisplayName ?? plugin.Plugin!.PluginName,
                Property = nameof(Domain.Billing.PluginBilling.ProjectPlugin),
            },
            new()
            {
                OldValue = "",
                NewValue = globalBillingKind,
                Property = nameof(Domain.Billing.PluginBilling.GlobalBilling),
            },
            new()
            {
                OldValue = "",
                NewValue = billing.BudgetLimit.ToString(CultureInfo.InvariantCulture),
                Property = nameof(Domain.Billing.PluginBilling.BudgetLimit),
            },
            new()
            {
                OldValue = "",
                NewValue = billing.HostingFee.ToString(CultureInfo.InvariantCulture),
                Property = nameof(Domain.Billing.PluginBilling.HostingFee),
            },
            new()
            {
                OldValue = "",
                NewValue = billing.Currency,
                Property = nameof(Domain.Billing.PluginBilling.Currency),
            },
            new()
            {
                OldValue = "",
                NewValue = billing.TargetMargin.ToString(CultureInfo.InvariantCulture),
                Property = nameof(Domain.Billing.PluginBilling.TargetMargin),
            },
            new()
            {
                OldValue = "",
                NewValue =
                    billing.TimeFrame == Domain.Billing.TimeFrame.DATE
                        ? billing.Date?.Date.ToString(new CultureInfo("de-DE"))!
                        : billing.TimeFrame.ToString(),
                Property = nameof(Domain.Billing.PluginBilling.TimeFrame),
            },
            new()
            {
                OldValue = "",
                NewValue = billing.Notes ?? "null",
                Property = nameof(Domain.Billing.PluginBilling.Notes),
            },
        };

        if (billing.DisplayName != null)
        {
            billingChanges.Insert(
                1,
                new()
                {
                    OldValue = "",
                    NewValue = billing.DisplayName,
                    Property = nameof(Domain.Billing.PluginBilling.DisplayName),
                }
            );
        }

        await _logRepository.AddProjectLogForCurrentActor(
            plugin.Project!,
            Action.ADDED_PROJECT_PLUGIN_BILLING,
            billingChanges
        );
    }
}
