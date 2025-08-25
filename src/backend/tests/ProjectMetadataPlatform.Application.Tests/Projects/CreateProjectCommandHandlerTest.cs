using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class CreateProjectCommandHandlerTest
{
    private CreateProjectCommandHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;
    private Mock<IPluginRepository> _mockPluginRepo;
    private Mock<ITeamRepository> _teamRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ISlugHelper> _mockSlugHelper;

    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _mockPluginRepo = new Mock<IPluginRepository>();
        _teamRepository = new Mock<ITeamRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSlugHelper = new Mock<ISlugHelper>();
        _handler = new CreateProjectCommandHandler(
            _mockProjectRepo.Object,
            _mockPluginRepo.Object,
            _teamRepository.Object,
            _mockLogRepo.Object,
            _mockUnitOfWork.Object,
            _mockSlugHelper.Object
        );
    }

    [Test]
    public async Task CreateProject_Test()
    {
        // prepare
        var plugins = new List<ProjectPlugins>();
        plugins.Add(new ProjectPlugins { Url = "https://example.com", PluginId = 200 });
        _mockProjectRepo
            .Setup(m => m.AddProjectAsync(It.IsAny<Project>()))
            .Callback<Project>(p => p.Id = 1);
        _mockPluginRepo.Setup(m => m.CheckPluginExists(It.IsAny<int>())).ReturnsAsync(true);
        _mockSlugHelper.Setup(m => m.GenerateSlug(It.IsAny<string>())).Returns("example_project");
        _mockSlugHelper
            .Setup(m => m.GetProjectIdBySlug("example_project"))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Project with this slug does not exist: example_project"
                )
            );
        // act

        var result = await _handler.Handle(
            new CreateProjectCommand(
                ProjectName: "Example Project",
                ClientName: "Example Business Unit",
                OfferId: "1",
                Company: "Example Company",
                CompanyState: CompanyState.EXTERNAL,
                TeamId: null,
                IsmsLevel: SecurityLevel.HIGH,
                Plugins: plugins,
                Notes: "Example Notes"
            ),
            It.IsAny<CancellationToken>()
        );

        Assert.That(result, Is.EqualTo(1));
        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentUser(
                    It.IsAny<Project>(),
                    Action.ADDED_PROJECT,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentUser(
                    It.IsAny<Project>(),
                    Action.ADDED_PROJECT_PLUGIN,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
    }

    [Test]
    public void CreateProject_Test_ThrowsExceptionWhenSlugAlreadyExists()
    {
        var plugins = new List<ProjectPlugins>();
        plugins.Add(new ProjectPlugins { Url = "https://example.com", PluginId = 200 });
        _mockPluginRepo.Setup(m => m.CheckPluginExists(It.IsAny<int>())).ReturnsAsync(true);
        _mockSlugHelper.Setup(m => m.GenerateSlug(It.IsAny<string>())).Returns("example_project");
        _mockSlugHelper.Setup(m => m.GetProjectIdBySlug("example_project")).ReturnsAsync(1);
        _mockSlugHelper.Setup(m => m.CheckProjectSlugExists("example_project")).ReturnsAsync(true);

        var ex = Assert.ThrowsAsync<ProjectSlugAlreadyExistsException>(async () =>
        {
            await _handler.Handle(
                new CreateProjectCommand(
                    ProjectName: "Example Project",
                    ClientName: "Example Business Unit",
                    OfferId: "1",
                    Company: "Example Department",
                    CompanyState: CompanyState.EXTERNAL,
                    TeamId: null,
                    IsmsLevel: SecurityLevel.HIGH,
                    Plugins: plugins,
                    Notes: "Example Notes"
                ),
                It.IsAny<CancellationToken>()
            );
        });

        Assert.That(
            ex.Message,
            Is.EqualTo("A Project with this slug already exists: example_project")
        );

        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentUser(
                    It.IsAny<Project>(),
                    Action.ADDED_PROJECT,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentUser(
                    It.IsAny<Project>(),
                    Action.ADDED_PROJECT_PLUGIN,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
        _mockProjectRepo.Verify(m => m.AddProjectAsync(It.IsAny<Project>()), Times.Never);
    }

    [Test]
    public void CreateProject_Test_ThrowsExceptionWhenNotesTooLong()
    {
        var plugins = new List<ProjectPlugins>();
        plugins.Add(new ProjectPlugins { Url = "https://example.com", PluginId = 200 });
        _mockPluginRepo.Setup(m => m.CheckPluginExists(It.IsAny<int>())).ReturnsAsync(true);
        _mockSlugHelper.Setup(m => m.GenerateSlug(It.IsAny<string>())).Returns("example_project");
        _mockSlugHelper
            .Setup(m => m.GetProjectIdBySlug("example_project"))
            .ThrowsAsync(
                new InvalidOperationException(
                    "Project with this slug does not exist: example_project"
                )
            );
        _mockSlugHelper.Setup(m => m.CheckProjectSlugExists("example_project")).ReturnsAsync(false);

        var ex = Assert.ThrowsAsync<ProjectNotesSizeException>(async () =>
        {
            await _handler.Handle(
                new CreateProjectCommand(
                    ProjectName: "Example Project",
                    ClientName: "Example Business Unit",
                    OfferId: "1",
                    Company: "Example Department",
                    CompanyState: CompanyState.EXTERNAL,
                    TeamId: null,
                    IsmsLevel: SecurityLevel.HIGH,
                    Plugins: plugins,
                    Notes: new string('a', 501)
                ),
                It.IsAny<CancellationToken>()
            );
        });

        Assert.That(
            ex.Message,
            Is.EqualTo("The project notes are 501 chars long. Maximum allowed is 500 chars.")
        );

        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentUser(
                    It.IsAny<Project>(),
                    Action.ADDED_PROJECT,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentUser(
                    It.IsAny<Project>(),
                    Action.ADDED_PROJECT_PLUGIN,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
        _mockProjectRepo.Verify(m => m.AddProjectAsync(It.IsAny<Project>()), Times.Never);
    }
}
