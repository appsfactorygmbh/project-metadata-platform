using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.BillingExceptions;

/// <summary>
/// Exception thrown when global billing information is not found.
/// </summary>
/// <param name="id">Id of the billing object that was searched for.</param>
public class BillingInformationNotFoundException(int id)
    : EntityNotFoundException("The billing information with id " + id + " was not found.") { }
