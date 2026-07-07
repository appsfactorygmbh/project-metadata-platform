using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class DeleteProjectCommandHandlerTest
{
    private DeleteProjectCommandHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteProjectCommandHandler(
            _mockProjectRepo.Object,
            _mockLogRepo.Object,
            _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task DeleteProject_Test()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Heather",
            Slug = "heather",
            ClientName = "Metatron",
            IsArchived = true,
            CompanyId = 1,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Project>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.DELETE, true },
                }
            );
        _ = _mockProjectRepo.Setup(m => m.GetProjectAsync(It.IsAny<int>())).ReturnsAsync(project);
        _ = _mockProjectRepo
            .Setup(m => m.DeleteProjectAsync(It.IsAny<Project>()))
            .ReturnsAsync(project);

        var result = await _handler.Handle(
            new DeleteProjectCommand(1),
            It.IsAny<CancellationToken>()
        );

        Assert.That(project, Is.EqualTo(result));
    }

    [Test]
    public void DeleteProject_ThrowsArgumentException_Test()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Heather",
            Slug = "heather",
            ClientName = "Metatron",
            IsArchived = false,
            CompanyId = 1,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Project>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.DELETE, true },
                }
            );
        _ = _mockProjectRepo.Setup(m => m.GetProjectAsync(It.IsAny<int>())).ReturnsAsync(project);

        var ex = Assert.ThrowsAsync<ProjectNotArchivedException>(() =>
            _handler.Handle(new DeleteProjectCommand(1), It.IsAny<CancellationToken>())
        );
        Assert.That(ex.Message, Is.EqualTo("The project 1 is not archived."));
    }

    [Test]
    public void DeleteProject_NotFound_Test()
    {
        _ = _mockProjectRepo
            .Setup(m => m.GetProjectAsync(It.IsAny<int>()))
            .ThrowsAsync(new ProjectNotFoundException("Project not found."));
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Project>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.DELETE, true },
                }
            );
        var ex = Assert.ThrowsAsync<ProjectNotFoundException>(() =>
            _handler.Handle(new DeleteProjectCommand(1), It.IsAny<CancellationToken>())
        );
        Assert.That(
            ex.Message,
            Is.EqualTo("The project with slug Project not found. was not found.")
        );
    }

    [Test]
    public void LogWhenProjectIsDeleted_Test()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Heather",
            Slug = "heather",
            ClientName = "Metatron",
            IsArchived = true,
            CompanyId = 1,
        };
        _ = _mockProjectRepo.Setup(m => m.GetProjectAsync(It.IsAny<int>())).ReturnsAsync(project);
        _ = _mockProjectRepo
            .Setup(m => m.DeleteProjectAsync(It.IsAny<Project>()))
            .ReturnsAsync(project);
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Project>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.DELETE, true },
                }
            );
        _ = _handler.Handle(new DeleteProjectCommand(1), It.IsAny<CancellationToken>());

        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentActor(
                    It.Is<Project>(p => p.ProjectName == "Heather" && p.ClientName == "Metatron"),
                    Action.REMOVED_PROJECT,
                    It.Is<List<LogChange>>(changes =>
                        changes.Any(change =>
                            change.Property == "ProjectName"
                            && change.OldValue == "Heather"
                            && change.NewValue == ""
                        )
                        && changes.Any(change =>
                            change.Property == "ClientName"
                            && change.OldValue == "Metatron"
                            && change.NewValue == ""
                        )
                    )
                ),
            Times.Once
        );
    }

    public async Task CreateProject_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Project>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.DELETE, false },
                }
            );

        var request = new DeleteProjectCommand(1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
