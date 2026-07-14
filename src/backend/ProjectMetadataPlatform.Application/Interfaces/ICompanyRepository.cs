using System.Linq;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.Companies;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Repository for Companies.
/// </summary>
public interface ICompanyRepository
{
    /// <summary>
    /// Returns all companies.
    /// </summary>
    /// <returns>List of all Companies.</returns>
    Task<IQueryable<Company>> GetCompaniesAsync();

    /// <summary>
    /// Returns a Company by its Id.
    /// </summary>
    /// <param name="id">Id of the Company.</param>
    /// <returns>Specified Company</returns>
    Task<Company> GetCompanyAsync(int id);

    /// <summary>
    /// Checks wether a Companies with the specified Id exists.
    /// </summary>
    /// <param name="id">Id that gets checked.</param>
    /// <returns>Boolean representing the Existence of the Company</returns>
    Task<bool> CheckIfCompanyExistsAsync(int id);

    /// <summary>
    /// Checks wether a Companies with the specified Name exists.
    /// </summary>
    /// <param name="companyName">Name that gets checked.</param>
    /// <returns>Boolean representing the Existence of the Company</returns>
    Task<bool> CheckIfCompanyNameExistsAsync(string companyName);

    /// <summary>
    /// Returns the Name for the company with the specified id.
    /// </summary>
    /// <param name="id">Id of the company</param>
    /// <returns>Name of the company</returns>
    Task<string> RetrieveNameForIdAsync(int id);

    /// <summary>
    /// Returns a Company by its Name.
    /// </summary>
    /// <param name="companyName">Name of the Company.</param>
    /// <returns>Specified Company</returns>
    Task<Company> GetCompanyByNameAsync(string companyName);

    /// <summary>
    /// Adds a new Company.
    /// </summary>
    /// <param name="company">new Company to be added.</param>
    /// <returns></returns>
    Task AddCompanyAsync(Company company);

    /// <summary>
    /// Updates an existing company.
    /// </summary>
    /// <param name="company">Updated Company.</param>
    /// <returns>Updated Company.</returns>
    Task<Company> UpdateCompanyAsync(Company company);

    /// <summary>
    /// Deletes a Company.
    /// </summary>
    /// <param name="company">Company to be deleted.</param>
    /// <returns>Deleted Company.</returns>
    Task<Company> DeleteCompanyAsync(Company company);

    /// <summary>
    /// Returns a Company with all linked projects.
    /// </summary>
    /// <param name="id">Id of the company.</param>
    /// <returns>Company with Collection of Projection.</returns>
    Task<Company> GetCompanyWithProjectsAsync(int id);
}
