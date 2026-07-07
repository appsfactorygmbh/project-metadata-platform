using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for Office Locations.
/// </summary>
public interface IOfficeLocationRepository
{
    /// <summary>
    /// Returns all Office Locations.
    /// </summary>
    /// <returns>List of all Office Locations.</returns>
    Task<IQueryable<OfficeLocation>> GetOfficeLocationsAsync();

    /// <summary>
    /// Returns a Office Location by its Id.
    /// </summary>
    /// <param name="id">Id of the Office Location.</param>
    /// <returns>Specified Office Location</returns>
    Task<OfficeLocation> GetOfficeLocationAsync(int id);

    /// <summary>
    /// Checks wether a Office Locations with the specified Id exists.
    /// </summary>
    /// <param name="id">Id that gets checked.</param>
    /// <returns>Boolean representing the Existence of the Office Location</returns>
    Task<bool> CheckIfOfficeLocationExistsAsync(int id);

    /// <summary>
    /// Checks wether a Office Locations with the specified Name exists.
    /// </summary>
    /// <param name="officeLocationName">Name that gets checked.</param>
    /// <returns>Boolean representing the Existence of the Office Location</returns>
    Task<bool> CheckIfOfficeLocationNameExistsAsync(string officeLocationName);

    /// <summary>
    /// Returns a Office Location by its Name.
    /// </summary>
    /// <param name="officeLocationName">Name of the Office Location.</param>
    /// <returns>Specified Office Location</returns>
    Task<OfficeLocation> GetOfficeLocationByNameAsync(string officeLocationName);

    /// <summary>
    /// Adds a new Office Location.
    /// </summary>
    /// <param name="location">new Office Location to be added.</param>
    /// <returns></returns>
    Task AddOfficeLocationAsync(OfficeLocation location);

    /// <summary>
    /// Updates an existing officeLocation.
    /// </summary>
    /// <param name="location">Updated Office Location.</param>
    /// <returns>Updated Office Location.</returns>
    Task<OfficeLocation> UpdateOfficeLocationAsync(OfficeLocation location);

    /// <summary>
    /// Deletes a Office Location.
    /// </summary>
    /// <param name="location">Office Location to be deleted.</param>
    /// <returns>Deleted Office Location.</returns>
    Task<OfficeLocation> DeleteOfficeLocationAsync(OfficeLocation location);
}
