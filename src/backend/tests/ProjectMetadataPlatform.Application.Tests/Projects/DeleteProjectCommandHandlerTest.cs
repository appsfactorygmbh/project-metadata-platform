using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
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

    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteProjectCommandHandler(
            _mockProjectRepo.Object,
            _mockLogRepo.Object,
            _mockUnitOfWork.Object
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
        };
        _mockProjectRepo.Setup(m => m.GetProjectAsync(It.IsAny<int>())).ReturnsAsync(project);
        _mockProjectRepo
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
        };
        _mockProjectRepo.Setup(m => m.GetProjectAsync(It.IsAny<int>())).ReturnsAsync(project);

        var ex = Assert.ThrowsAsync<ProjectNotArchivedException>(() =>
            _handler.Handle(new DeleteProjectCommand(1), It.IsAny<CancellationToken>())
        );
        Assert.That(ex.Message, Is.EqualTo("The project 1 is not archived."));
    }

    [Test]
    public void DeleteProject_NotFound_Test()
    {
        _mockProjectRepo
            .Setup(m => m.GetProjectAsync(It.IsAny<int>()))
            .ThrowsAsync(new ProjectNotFoundException("Project not found."));

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
        };
        _mockProjectRepo.Setup(m => m.GetProjectAsync(It.IsAny<int>())).ReturnsAsync(project);
        _mockProjectRepo
            .Setup(m => m.DeleteProjectAsync(It.IsAny<Project>()))
            .ReturnsAsync(project);

        _ = _handler.Handle(new DeleteProjectCommand(1), It.IsAny<CancellationToken>());

        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentUser(
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
}
