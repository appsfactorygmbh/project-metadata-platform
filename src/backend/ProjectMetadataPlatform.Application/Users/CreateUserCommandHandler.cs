using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.UserException;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.OfficeLocations;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Handler for the <see cref="CreateUserCommand" />
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApplicationUser>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IBusinessUnitRepository _businessUnitRepository;
    private readonly IOfficeLocationRepository _officeLocationRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogRepository _logRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new instance of <see cref="CreateUserCommandHandler" />.
    /// </summary>
    /// <param name="usersRepository">Repository for accessing user data.</param>
    /// <param name="logRepository">Repository for logging data.</param>
    /// <param name="teamRepository">Repository for accessing team data.</param>
    /// <param name="departmentRepository">Repository for accessing department data.</param>
    /// <param name="businessUnitRepository">Repository for accessing bu data.</param>
    /// <param name="officeLocationRepository">Repository for accessing office location data.</param>
    /// <param name="companyRepository">Repository for accessing company data.</param>
    /// <param name="unitOfWork">Unit of work for managing transactions.</param>
    public CreateUserCommandHandler(
        IUsersRepository usersRepository,
        ILogRepository logRepository,
        ITeamRepository teamRepository,
        IDepartmentRepository departmentRepository,
        IBusinessUnitRepository businessUnitRepository,
        IOfficeLocationRepository officeLocationRepository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork
    )
    {
        _usersRepository = usersRepository;
        _logRepository = logRepository;
        _teamRepository = teamRepository;
        _businessUnitRepository = businessUnitRepository;
        _companyRepository = companyRepository;
        _officeLocationRepository = officeLocationRepository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new User with the given data.
    /// </summary>
    /// <param name="request">Request for user creation.</param>
    /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
    /// <returns>The ID of the created user.</returns>
    public async Task<ApplicationUser> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        if (await _usersRepository.CheckUserExists(request.EmployeeId))
        {
            throw new UserAlreadyExistsException("DuplicateEmployeeNumber");
        }
        if (request.Password != null)
        {
            await _usersRepository.CheckPasswordFormat(request.Password);
        }

        Collection<Team> teams = [];
        Collection<Team> teamSupport = [];

        foreach (var team in request.Teams ?? [])
        {
            var teamObject = await _teamRepository.GetTeamByNameAsync(team);

            teams.Add(teamObject);
        }

        foreach (var team in request.TeamSupport ?? [])
        {
            var teamObject = await _teamRepository.GetTeamByNameAsync(team);

            teamSupport.Add(teamObject);
        }

        Collection<Department> departments = [];
        foreach (var department in request.Departments ?? [])
        {
            var departmentObject = await GetOrCreateDepartment(department);
            departments.Add(departmentObject);
        }
        Collection<BusinessUnit> businessUnits = [];
        foreach (var bu in request.BusinessUnits ?? [])
        {
            var buObject = await GetOrCreateBusinessUnit(bu);
            businessUnits.Add(buObject);
        }
        var company = request.Company == null ? null : await GetOrCreateCompany(request.Company);
        var officeLocation =
            request.OfficeLocation == null
                ? null
                : await GetOrCreateOfficeLocation(request.OfficeLocation);
        // Uses Email as Username because: Username cant be empty + Username cant be duplicate.
        var user = new ApplicationUser
        {
            EmployeeId = request.EmployeeId,
            Email = request.Email,
            UserName = request.Email,
            IsActive = request.IsActive,
            IsScimProvisioned = request.IsScimProvisioned,
            BusinessUnits = businessUnits.Any() ? businessUnits : null,
            Company = company,
            Departments = departments?.Any() == true ? departments : null,
            Teams = teams.Any() ? teams : null,
            TeamSupport = teamSupport.Any() ? teamSupport : null,
            JobTitles = request.JobTitles?.Any() == true ? request.JobTitles : null,
            OfficeLocation = officeLocation,
        };
        await AddCreatedUserLog(user);
        _ = await _usersRepository.CreateUserAsync(user, request.Password);

        await _unitOfWork.CompleteAsync();
        return user;
    }

    private async Task<Department> GetOrCreateDepartment(string departmentName)
    {
        if (await _departmentRepository.CheckIfDepartmentNameExistsAsync(departmentName))
        {
            return await _departmentRepository.GetDepartmentByNameAsync(departmentName);
        }
        else
        {
            var department = new Department { DepartmentName = departmentName };
            await _logRepository.AddDepartmentLogForCurrentActor(
                department,
                Domain.Logs.Action.ADDED_DEPARTMENT,
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
    }

    private async Task<BusinessUnit> GetOrCreateBusinessUnit(string buName)
    {
        if (await _businessUnitRepository.CheckIfBusinessUnitNameExistsAsync(buName))
        {
            return await _businessUnitRepository.GetBusinessUnitByNameAsync(buName);
        }
        else
        {
            var bu = new BusinessUnit { BusinessUnitName = buName };
            await _logRepository.AddBusinessUnitLogForCurrentActor(
                bu,
                Domain.Logs.Action.ADDED_BUSINESS_UNIT,
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
    }

    private async Task<OfficeLocation> GetOrCreateOfficeLocation(string officeLocationName)
    {
        if (
            await _officeLocationRepository.CheckIfOfficeLocationNameExistsAsync(officeLocationName)
        )
        {
            return await _officeLocationRepository.GetOfficeLocationByNameAsync(officeLocationName);
        }
        else
        {
            var officeLocation = new OfficeLocation { OfficeLocationName = officeLocationName };
            await _logRepository.AddOfficeLocationLogForCurrentActor(
                officeLocation,
                Domain.Logs.Action.ADDED_OFFICE_LOCATION,
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
    }

    private async Task<Company> GetOrCreateCompany(string companyName)
    {
        if (await _companyRepository.CheckIfCompanyNameExistsAsync(companyName))
        {
            return await _companyRepository.GetCompanyByNameAsync(companyName);
        }
        else
        {
            var company = new Company { CompanyName = companyName };
            await _logRepository.AddCompanyLogForCurrentActor(
                company,
                Domain.Logs.Action.ADDED_COMPANY,
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
    }

    /// <summary>
    /// Adds a log entry for creating a user.
    /// </summary>
    /// <param name="user">User that was created.
    /// </param>
    /// <returns></returns>
    private async Task AddCreatedUserLog(ApplicationUser user)
    {
        var changes = new List<LogChange>
        {
            new()
            {
                OldValue = "",
                NewValue = user.Email!,
                Property = nameof(ApplicationUser.Email),
            },
        };
        if (user.EmployeeId != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = user.EmployeeId,
                    Property = nameof(ApplicationUser.EmployeeId),
                }
            );
        }
        if (user.Teams != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = String.Join(", ", user.Teams!.Select(t => t.TeamName)),
                    Property = nameof(ApplicationUser.Teams),
                }
            );
        }
        if (user.TeamSupport != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = String.Join(", ", user.TeamSupport!.Select(t => t.TeamName)),
                    Property = nameof(ApplicationUser.TeamSupport),
                }
            );
        }

        changes.Add(
            new()
            {
                OldValue = "",
                NewValue = user.IsActive.ToString()!,
                Property = nameof(ApplicationUser.IsActive),
            }
        );

        changes.Add(
            new()
            {
                OldValue = "",
                NewValue = user.IsScimProvisioned.ToString()!,
                Property = nameof(ApplicationUser.IsScimProvisioned),
            }
        );

        if (user.BusinessUnits != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = String.Join(
                        ", ",
                        user.BusinessUnits.Select(b => b.BusinessUnitName)
                    ),
                    Property = nameof(ApplicationUser.BusinessUnits),
                }
            );
        }
        if (user.JobTitles != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = String.Join(", ", user.JobTitles),
                    Property = nameof(ApplicationUser.JobTitles),
                }
            );
        }
        if (user.Departments != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = String.Join(", ", user.Departments.Select(d => d.DepartmentName)),
                    Property = nameof(ApplicationUser.Departments),
                }
            );
        }
        if (user.Company != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = user.Company.CompanyName,
                    Property = nameof(ApplicationUser.Company),
                }
            );
        }
        if (user.OfficeLocation != null)
        {
            changes.Add(
                new()
                {
                    OldValue = "",
                    NewValue = user.OfficeLocation.OfficeLocationName,
                    Property = nameof(ApplicationUser.OfficeLocation),
                }
            );
        }

        await _logRepository.AddUserLogForCurrentActor(
            user,
            Domain.Logs.Action.ADDED_USER,
            changes
        );
    }
}
