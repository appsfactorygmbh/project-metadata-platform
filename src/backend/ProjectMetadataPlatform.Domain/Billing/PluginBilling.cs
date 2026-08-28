using System;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Domain.Billing;

/// <summary>
/// Represents billing information for a project plugin in the database.
/// </summary>
public class PluginBilling
{
    /// <summary>
    /// Id of the ProjectPlugin, part of the primary key.
    /// </summary>
    public int PluginId { get; set; }

    /// <summary>
    /// Id of the Project, part of the primary key.
    /// </summary>/
    public int ProjectId { get; set; }

    /// <summary>
    /// Id of the global billing information. Foreign Key.
    /// </summary>
    public int BillingId { get; set; }

    /// <summary>
    /// Global Billing Information stored in the relation.
    /// </summary>
    public GlobalBilling? GlobalBilling { get; set; }

    /// <summary>
    /// Project Plugin stored in the relation.
    /// </summary>
    public ProjectPlugin? ProjectPlugin { get; set; }

    /// <summary>
    /// Optional Display Name
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// String representing a Currency used in billing.
    /// </summary>
    public required string Currency { get; set; }

    /// <summary>
    /// Budget Limit for the plugin.
    /// </summary>
    public required decimal BudgetLimit { get; set; }

    /// <summary>
    /// Hosting Fee for the plugin.
    /// </summary>
    public required decimal HostingFee { get; set; }

    /// <summary>
    /// Target Margin percentage.
    /// </summary>
    public required int TargetMargin { get; set; }

    /// <summary>
    /// Time frame in which the plugin is billed.
    /// </summary>
    public required TimeFrame TimeFrame { get; set; }

    /// <summary>
    /// Optional specific date on which the plugin will be billed.
    /// </summary>
    public DateTimeOffset? Date { get; set; }

    /// <summary>
    /// Optional Notes on the billing information.
    /// </summary>
    public string? Notes { get; set; }
}
