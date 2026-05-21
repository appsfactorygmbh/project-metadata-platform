using ProjectMetadataPlatform.Domain.Errors.BasicExceptions;

namespace ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;

/// <summary>
/// Exception thrown when a office location is not found.
/// </summary>
public class OfficeLocationNotFoundException : EntityNotFoundException
{
    /// <summary>
    /// Initializes a new instance of the <see cref=" OfficeLocationNotFoundException"/> class with a specified office location ID.
    /// </summary>
    /// <param name="locationId">The ID of the office location that was not found.</param>
    public OfficeLocationNotFoundException(int locationId)
        : base("The Office Location with id " + locationId + " was not found.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref=" OfficeLocationNotFoundException"/> class with a specified office location name.
    /// </summary>
    /// <param name="location">The name of the office location that was not found.</param>
    public OfficeLocationNotFoundException(string location)
        : base("The Office Location with name " + location + " was not found.") { }
}
