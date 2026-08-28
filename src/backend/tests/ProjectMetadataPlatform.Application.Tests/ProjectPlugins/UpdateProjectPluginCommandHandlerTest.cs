using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.ProjectPlugins;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.ProjectPlugins;

[TestFixture]
public class UpdateProjectPluginCommandHandlerTest
{
    private UpdateProjectPluginCommandHandler _handler;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IPluginRepository> _mockPluginRepository;

    private Mock<IProjectsRepository> _mockProjectsRepository;

    [SetUp]
    public void Setup()
    {
        _mockPluginRepository = new Mock<IPluginRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProjectsRepository = new Mock<IProjectsRepository>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _handler = new UpdateProjectPluginCommandHandler(
            projectsRepository: _mockProjectsRepository.Object,
            pluginRepository: _mockPluginRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task UpdateProjectPlugin_CallsRepositoryCorrectly()
    {
        _mockProjectsRepository
            .Setup(repo => repo.CheckProjectExists(It.IsAny<int>()))
            .ReturnsAsync(true);
        // Arrange
        var returnProjectPlugin = new ProjectPlugin()
        {
            Id = 1,
            PluginId = 1,
            ProjectId = 1,
            DisplayName = "Test_1",
            Url = "Url_2",
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ProjectPlugin>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _mockPluginRepository
            .Setup(repo => repo.GetProjectPluginAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(returnProjectPlugin);

        _ = _mockPluginRepository
            .Setup(repo => repo.StoreProjectPlugin(It.IsAny<ProjectPlugin>()))
            .ReturnsAsync((ProjectPlugin plugin) => plugin);

        _ = _mockPluginRepository
            .Setup(repo =>
                repo.CheckProjectPluginExists(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            )
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            new UpdateProjectPluginCommand(1, 1, "Test_2", "Url_2"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(returnProjectPlugin));
        _mockPluginRepository.Verify(
            m => m.GetProjectPluginAsync(It.Is<int>(id => id == 1), It.Is<int>(id => id == 1)),
            Times.Once
        );

        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentActor(
                    It.IsAny<Project>(),
                    Action.UPDATED_PROJECT_PLUGIN,
                    It.Is<List<LogChange>>(changes =>
                        changes[0].Property == "DisplayName"
                        && changes[0].NewValue == "Test_2"
                        && changes[0].OldValue == "Test_1"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task UpdateProjectPlugin_NoLogCreatedIfValuesAreEqual()
    {
        _mockProjectsRepository
            .Setup(repo => repo.CheckProjectExists(It.IsAny<int>()))
            .ReturnsAsync(true);
        // Arrange
        var returnProjectPlugin = new ProjectPlugin()
        {
            Id = 1,
            PluginId = 1,
            ProjectId = 1,
            DisplayName = "Test_1",
            Url = "Url_2",
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ProjectPlugin>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _mockPluginRepository
            .Setup(repo => repo.GetProjectPluginAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(returnProjectPlugin);

        _ = _mockPluginRepository
            .Setup(repo => repo.StoreProjectPlugin(It.IsAny<ProjectPlugin>()))
            .ReturnsAsync((ProjectPlugin plugin) => plugin);

        _ = _mockPluginRepository
            .Setup(repo =>
                repo.CheckProjectPluginExists(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            )
            .ReturnsAsync(false);

        // Act
        _ = await _handler.Handle(
            new UpdateProjectPluginCommand(1, 1, "Test_1", "Url_2"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentActor(
                    It.IsAny<Project>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
    }

    [Test]
    public void UpdateProjectPlugin_ThrowsDisplayNameAlreadyExistsException_IfNewDisplayNameAlreadyExists()
    {
        _mockProjectsRepository
            .Setup(repo => repo.CheckProjectExists(It.IsAny<int>()))
            .ReturnsAsync(true);
        // Arrange
        var returnProjectPlugin = new ProjectPlugin()
        {
            Id = 1,
            PluginId = 1,
            ProjectId = 1,
            DisplayName = "Test_1",
            Url = "Url_2",
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ProjectPlugin>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _mockPluginRepository
            .Setup(repo => repo.GetProjectPluginAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(returnProjectPlugin);

        _ = _mockPluginRepository
            .Setup(repo => repo.StoreProjectPlugin(It.IsAny<ProjectPlugin>()))
            .ReturnsAsync((ProjectPlugin plugin) => plugin);

        _ = _mockPluginRepository
            .Setup(repo =>
                repo.CheckProjectPluginExists(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            )
            .ReturnsAsync(true);

        // Act + Assert
        var ex = Assert.ThrowsAsync<ProjectPluginAlreadyExistsException>(async () =>
            await _handler.Handle(
                new UpdateProjectPluginCommand(1, 1, "Test_2", "Url_3"),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Url_3"));
    }

    [Test]
    public async Task EditProjectPlugin_AuthorizationFailsThrowsTest()
    {
        _mockProjectsRepository
            .Setup(repo => repo.CheckProjectExists(It.IsAny<int>()))
            .ReturnsAsync(true);
        var returnProjectPlugin = new ProjectPlugin()
        {
            Id = 1,
            PluginId = 1,
            ProjectId = 1,
            DisplayName = "Test_1",
            Url = "Url_2",
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ProjectPlugin>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(false);
        _ = _mockPluginRepository
            .Setup(repo => repo.GetProjectPluginAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(returnProjectPlugin);

        var request = new UpdateProjectPluginCommand(
            ProjectId: 1,
            ProjectpluginId: 1,
            Name: "Test_2",
            Url: "Url_2"
        );

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }

    [Test]
    public async Task EditProjectPlugin_ProjectNotFoundThrowsTest()
    {
        _mockProjectsRepository
            .Setup(repo => repo.CheckProjectExists(It.IsAny<int>()))
            .ReturnsAsync(false);

        var request = new UpdateProjectPluginCommand(
            ProjectId: 1,
            ProjectpluginId: 1,
            Name: "Test_2",
            Url: "Url_2"
        );

        _ = Assert.ThrowsAsync<ProjectNotFoundException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
