using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
using ProjectMetadataPlatform.Domain.Errors.CompanyExceptions;
using ProjectMetadataPlatform.Domain.Errors.DepartmentExceptions;
using ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.Helper;

/// <inheritdoc/>
public class GetOrCreateHelper : IGetOrCreateHelper
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IBusinessUnitRepository _businessUnitRepository;
    private readonly IOfficeLocationRepository _officeLocationRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogRepository _logRepository;
    private readonly IApiTokenRepository _apiTokenRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Creates a new instance of <see cref="GetOrCreateHelper" />.
    /// </summary>
    public GetOrCreateHelper(
        IDepartmentRepository departmentRepository,
        IBusinessUnitRepository businessUnitRepository,
        IOfficeLocationRepository officeLocationRepository,
        ICompanyRepository companyRepository,
        ILogRepository logRepository,
        IApiTokenRepository apiTokenRepository,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _departmentRepository = departmentRepository;
        _businessUnitRepository = businessUnitRepository;
        _officeLocationRepository = officeLocationRepository;
        _companyRepository = companyRepository;
        _logRepository = logRepository;
        _apiTokenRepository = apiTokenRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Checks if the requesting actor is an Api Token with the SCIM Scope.
    /// </summary>
    /// <returns>True if the requesting actor is an Api Token with the SCIM Scope. False otherwise.</returns>
    private async Task<bool> IsActorScimToken()
    {
        if (
            _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.AuthenticationMethod)
            != "API Token"
        )
        {
            return false;
        }
        else
        {
            var name = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
            if (name == null)
            {
                return false;
            }
            return await _apiTokenRepository.IsScimToken(name);
        }
    }

    /// <inheritdoc/>
    public async Task<Department> GetOrCreateDepartment(string departmentName)
    {
        if (await _departmentRepository.CheckIfDepartmentNameExistsAsync(departmentName))
        {
            return await _departmentRepository.GetDepartmentByNameAsync(departmentName);
        }
        else if (await IsActorScimToken())
        {
            var department = new Department { DepartmentName = departmentName };
            await _logRepository.AddDepartmentLogForCurrentActor(
                department,
                Action.ADDED_DEPARTMENT,
                [
                    new LogChange
                    {
                        OldValue = "",
                        NewValue = department.DepartmentName,
                        Property = nameof(Department.DepartmentName),
                    },
                ]
            );
            await _departmentRepository.AddDepartmentAsync(department);
            return department;
        }
        else
        {
            throw new DepartmentNotFoundException(departmentName);
        }
    }

    /// <inheritdoc/>
    public async Task<BusinessUnit> GetOrCreateBusinessUnit(string buName)
    {
        if (await _businessUnitRepository.CheckIfBusinessUnitNameExistsAsync(buName))
        {
            return await _businessUnitRepository.GetBusinessUnitByNameAsync(buName);
        }
        else if (await IsActorScimToken())
        {
            var bu = new BusinessUnit { BusinessUnitName = buName };
            await _logRepository.AddBusinessUnitLogForCurrentActor(
                bu,
                Action.ADDED_BUSINESS_UNIT,
                [
                    new LogChange
                    {
                        OldValue = "",
                        NewValue = bu.BusinessUnitName,
                        Property = nameof(BusinessUnit.BusinessUnitName),
                    },
                ]
            );
            await _businessUnitRepository.AddBusinessUnitAsync(bu);
            return bu;
        }
        else
        {
            throw new BusinessUnitNotFoundException(buName);
        }
    }

    /// <inheritdoc/>
    public async Task<OfficeLocation> GetOrCreateOfficeLocation(string officeLocationName)
    {
        if (
            await _officeLocationRepository.CheckIfOfficeLocationNameExistsAsync(officeLocationName)
        )
        {
            return await _officeLocationRepository.GetOfficeLocationByNameAsync(officeLocationName);
        }
        else if (await IsActorScimToken())
        {
            var officeLocation = new OfficeLocation { OfficeLocationName = officeLocationName };
            await _logRepository.AddOfficeLocationLogForCurrentActor(
                officeLocation,
                Action.ADDED_OFFICE_LOCATION,
                [
                    new LogChange
                    {
                        OldValue = "",
                        NewValue = officeLocation.OfficeLocationName,
                        Property = nameof(OfficeLocation.OfficeLocationName),
                    },
                ]
            );
            await _officeLocationRepository.AddOfficeLocationAsync(officeLocation);
            return officeLocation;
        }
        else
        {
            throw new OfficeLocationNotFoundException(officeLocationName);
        }
    }

    /// <inheritdoc/>
    public async Task<Company> GetOrCreateCompany(string companyName)
    {
        if (await _companyRepository.CheckIfCompanyNameExistsAsync(companyName))
        {
            return await _companyRepository.GetCompanyByNameAsync(companyName);
        }
        else if (await IsActorScimToken())
        {
            var company = new Company { CompanyName = companyName };
            await _logRepository.AddCompanyLogForCurrentActor(
                company,
                Action.ADDED_COMPANY,
                [
                    new LogChange
                    {
                        OldValue = "",
                        NewValue = company.CompanyName,
                        Property = nameof(Company.CompanyName),
                    },
                ]
            );
            await _companyRepository.AddCompanyAsync(company);
            return company;
        }
        else
        {
            throw new CompanyNotFoundException(companyName);
        }
    }
}
