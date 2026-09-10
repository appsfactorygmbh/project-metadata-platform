using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.BillingExceptions;

/// <summary>
/// Exception thrown when a billing kind already exists in the Project Metadata Platform.
/// </summary>
/// <param name="kind">The kind that already exists.</param>
public class BillingKindAlreadyExistsException(string kind)
    : EntityAlreadyExistsException("Billing information of the kind " + kind + " already exists.");
