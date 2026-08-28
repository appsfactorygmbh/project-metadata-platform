namespace ProjectMetadataPlatform.Domain.Errors.BillingExceptions;

/// <summary>
/// Thrown if billing information with a date timeframe is given no date value.
/// </summary>
public class PluginBillingDateMissingException()
    : BillingException("Billing Information of this type needs a date.");
