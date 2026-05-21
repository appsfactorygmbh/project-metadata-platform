using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;

/// <summary>
/// Exception thrown when a bu is not found.
/// </summary>
public class BusinessUnitNotFoundException : EntityNotFoundException
{
    /// <summary>
    /// Initializes a new instance of the <see cref=" BusinessUnitNotFoundException"/> class with a specified bu ID.
    /// </summary>
    /// <param name="buId">The ID of the Business Unit that was not found.</param>
    public BusinessUnitNotFoundException(int buId)
        : base("The Business Unit with id " + buId + " was not found.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref=" BusinessUnitNotFoundException"/> class with a specified bu name.
    /// </summary>
    /// <param name="bu">The name of the Business Unit that was not found.</param>
    public BusinessUnitNotFoundException(string bu)
        : base("The Business Unit with name " + bu + " was not found.") { }
}
