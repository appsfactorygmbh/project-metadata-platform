using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Handles the DeleteProjectCommand.
/// </summary>
public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Project?>
{
    private readonly IProjectsRepository _projectsRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteProjectCommandHandler"/> class.
    /// </summary>
    /// <param name="projectsRepository">The repository of projects.</param>
    /// <param name="logRepository">Repository for Logs</param>
    /// <param name="unitOfWork"> Used to save changes to the DbContext</param>
    public DeleteProjectCommandHandler(
        IProjectsRepository projectsRepository,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _projectsRepository = projectsRepository;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the DeleteProjectCommand.
    /// </summary>
    /// <param name="request">The DeleteProjectCommand.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deleted project, or null if the project was not archived.</returns>
    /// <exception cref="ProjectNotArchivedException">Thrown when the project is not archived.</exception>
    /// <exception cref="ProjectNotFoundException">Thrown when the project is not found.</exception>
    public async Task<Project?> Handle(
        DeleteProjectCommand request,
        CancellationToken cancellationToken
    )
    {
        var project = await _projectsRepository.GetProjectAsync(request.Id);

        switch (project)
        {
            case null:
                throw new ProjectNotFoundException(request.Id);
            case { IsArchived: false }:
                throw new ProjectNotArchivedException(project);
        }

        var deletedProject = await _projectsRepository.DeleteProjectAsync(project);

        await AddDeletedProjectLog(project);

        await _unitOfWork.CompleteAsync();
        return deletedProject;
    }

    private async Task AddDeletedProjectLog(Project project)
    {
        var changes = new List<LogChange>
        {
            new()
            {
                OldValue = project.ProjectName,
                NewValue = "",
                Property = nameof(Project.ProjectName),
            },
            new()
            {
                OldValue = project.ClientName,
                NewValue = "",
                Property = nameof(Project.ClientName),
            },
        };

        if (project.Team != null)
        {
            changes.Add(
                new()
                {
                    OldValue = project.Team.TeamName,
                    NewValue = "",
                    Property = "Team",
                }
            );
        }

        await _logRepository.AddProjectLogForCurrentUser(project, Action.REMOVED_PROJECT, changes);
    }
}
