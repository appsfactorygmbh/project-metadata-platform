using System.Threading.Tasks;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.Interfaces;

/// <summary>
/// Helper function for getting or creating objects.
/// </summary>
public interface IGetOrCreateHelper
{
    /// <summary>
    /// Either Returns the Department with the specified Name of (if the requester is a SCIM Api Token) creates a new Department with the specified name.
    /// </summary>
    /// <param name="departmentName">Requested Department Name.</param>
    /// <returns>Department with the specified name.</returns>
    Task<Department> GetOrCreateDepartment(string departmentName);

    /// <summary>
    /// Either Returns the BusinessUnit with the specified Name of (if the requester is a SCIM Api Token) creates a new BusinessUnit with the specified name.
    /// </summary>
    /// <param name="buName">Requested BusinessUnit Name.</param>
    /// <returns>BusinessUnit with the specified name.</returns>
    Task<BusinessUnit> GetOrCreateBusinessUnit(string buName);

    /// <summary>
    /// Either Returns the OfficeLocation with the specified Name of (if the requester is a SCIM Api Token) creates a new OfficeLocation with the specified name.
    /// </summary>
    /// <param name="officeLocationName">Requested OfficeLocation Name.</param>
    /// <returns>OfficeLocation with the specified name.</returns>
    Task<OfficeLocation> GetOrCreateOfficeLocation(string officeLocationName);

    /// <summary>
    /// Either Returns the Company with the specified Name of (if the requester is a SCIM Api Token) creates a new Company with the specified name.
    /// </summary>
    /// <param name="companyName">Requested Company Name.</param>
    /// <returns>Company with the specified name.</returns>
    Task<Company> GetOrCreateCompany(string companyName);
};
