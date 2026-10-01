using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <inheritdoc />
public class GetAllProjectsQueryHandler
    : IRequestHandler<
        GetAllProjectsQuery,
        (IEnumerable<Project>, IEnumerable<AuthorizationConstants.Actions>, ProjectCursor?)
    >
{
    private readonly IProjectsRepository _projectRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IPaginationHelper _paginationHelper;

    /// <summary>
    /// Creates a new instance of <see cref="GetAllProjectsQueryHandler" />.
    /// </summary>
    public GetAllProjectsQueryHandler(
        IProjectsRepository projectsRepository,
        IAuthorizationService authorizationService,
        IPaginationHelper paginationHelper
    )
    {
        _projectRepository = projectsRepository;
        _authorizationService = authorizationService;
        _paginationHelper = paginationHelper;
    }

    /// <inheritdoc />
    public async Task<(
        IEnumerable<Project>,
        IEnumerable<AuthorizationConstants.Actions>,
        ProjectCursor?
    )> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
    {
        var projectsQuery = await _projectRepository.GetProjectsAsync(
            request,
            request.Cursor,
            await _authorizationService.CheckSearchAttribute(
                nameof(Project),
                nameof(ProjectPlugin)
            ),
            await _authorizationService.CheckSearchAttribute(
                nameof(Project),
                nameof(Domain.Billing.PluginBilling)
            )
        );

        var permissions = await _authorizationService.GetAllowedActions<Project>(
            actions: [AuthorizationConstants.Actions.CREATE]
        );
        var (paginatedProjects, lastProject) = await _paginationHelper.PaginateWithAuthAsync(
            projectsQuery,
            request.Limit
        );
        var nextCursor =
            lastProject == null
                ? null
                : new ProjectCursor(
                    lastProject.Slug,
                    request.Request?.SortAttribute switch
                    {
                        ProjectSortCharacteristic.ProjectName => lastProject.ProjectName,
                        ProjectSortCharacteristic.Company =>
                            throw new System.NotImplementedException(),
                        ProjectSortCharacteristic.BusinessUnit => lastProject
                            .Team
                            ?.BusinessUnit
                            ?.BusinessUnitName
                            ?? "",
                        ProjectSortCharacteristic.Team => lastProject.Team?.TeamName ?? "",
                        ProjectSortCharacteristic.ClientName or null or _ => lastProject.ClientName,
                    }
                );
        return (paginatedProjects, permissions, nextCursor);
    }
}
