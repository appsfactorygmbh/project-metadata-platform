using System.Collections.Generic;

namespace ProjectMetadataPlatform.Domain.Billing;

/// <summary>
/// Representation of global billing information in the database.
/// </summary>
public class GlobalBilling
{
    /// <summary>
    /// Id of the billing information. Primary Key.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Name of the kind of billing information. Unique.
    /// </summary>
    public required string BillingKind { get; set; }

    /// <summary>
    /// String representing a default Currency format .
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Default budget limit.
    /// </summary>
    public decimal? BudgetLimit { get; set; }

    /// <summary>
    /// Default hosting fee.
    /// </summary>
    public decimal? HostingFee { get; set; }

    /// <summary>
    /// Default target margin percentage.
    /// </summary>
    public int? TargetMargin { get; set; }

    /// <summary>
    /// Default time frame.
    /// </summary>
    public TimeFrame? TimeFrame { get; set; }

    /// <summary>
    /// Holds the relation between global billing and project plugins.
    /// </summary>
    public ICollection<PluginBilling>? PluginBilling { get; set; }
}
