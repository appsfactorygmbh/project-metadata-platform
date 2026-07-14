using System.Linq;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for Business Units.
/// </summary>
public interface IBusinessUnitRepository
{
    /// <summary>
    /// Returns all BUs.
    /// </summary>
    /// <returns>List of all Business Units.</returns>
    Task<IQueryable<BusinessUnit>> GetBusinessUnitsAsync();

    /// <summary>
    /// Returns a BU by its Id.
    /// </summary>
    /// <param name="id">Id of the BU.</param>
    /// <returns>Specified BU</returns>
    Task<BusinessUnit> GetBusinessUnitAsync(int id);

    /// <summary>
    /// Checks wether a Business Units with the specified Id exists.
    /// </summary>
    /// <param name="id">Id that gets checked.</param>
    /// <returns>Boolean representing the Existence of the BU</returns>
    Task<bool> CheckIfBusinessUnitExistsAsync(int id);

    /// <summary>
    /// Checks wether a Business Units with the specified Name exists.
    /// </summary>
    /// <param name="buName">Name that gets checked.</param>
    /// <returns>Boolean representing the Existence of the BU</returns>
    Task<bool> CheckIfBusinessUnitNameExistsAsync(string buName);

    /// <summary>
    /// Returns the Name for the bu with the specified id.
    /// </summary>
    /// <param name="id">Id of the bu</param>
    /// <returns>Name of the bu</returns>
    Task<string> RetrieveNameForIdAsync(int id);

    /// <summary>
    /// Returns a BU by its Name.
    /// </summary>
    /// <param name="buName">Name of the BU.</param>
    /// <returns>Specified BU</returns>
    Task<BusinessUnit> GetBusinessUnitByNameAsync(string buName);

    /// <summary>
    /// Adds a new BU.
    /// </summary>
    /// <param name="bu">new Business Unit to be added.</param>
    /// <returns></returns>
    Task AddBusinessUnitAsync(BusinessUnit bu);

    /// <summary>
    /// Updates an existing business unit.
    /// </summary>
    /// <param name="businessUnit">Updated Business Unit.</param>
    /// <returns>Updated Business Unit.</returns>
    Task<BusinessUnit> UpdateBusinessUnitAsync(BusinessUnit businessUnit);

    /// <summary>
    /// Deletes a BU.
    /// </summary>
    /// <param name="businessUnit">BU to be deleted.</param>
    /// <returns>Deleted BU.</returns>
    Task<BusinessUnit> DeleteBusinessUnitAsync(BusinessUnit businessUnit);

    /// <summary>
    /// Returns a Business Unit with all linked Teams.
    /// </summary>
    /// <param name="id">Id of the BU.</param>
    /// <returns>The Bu with a Collection of Teams.</returns>
    Task<BusinessUnit> GetBusinessUnitWithTeamsAsync(int id);
}
