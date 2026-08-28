using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.ProjectPlugins;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.ProjectPlugins;

[TestFixture]
public class DeleteProjectPluginCommandHandlerTest
{
    private DeleteProjectPluginCommandHandler _handler;
    private Mock<IPluginRepository> _mockPluginRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IProjectsRepository> _mockProjectsRepository;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockPluginRepository = new Mock<IPluginRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProjectsRepository = new Mock<IProjectsRepository>();

        _handler = new DeleteProjectPluginCommandHandler(
            projectsRepository: _mockProjectsRepository.Object,
            pluginRepository: _mockPluginRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task DeleteProjectPlugin_WorksFine()
    {
        _mockProjectsRepository
            .Setup(repo => repo.CheckProjectExists(It.IsAny<int>()))
            .ReturnsAsync(true);
        // Arrange
        var returnProjectPlugin = new ProjectPlugin()
        {
            Id = 1,
            DisplayName = "Test_1",
            Url = "Url",
            ProjectId = 1,
            PluginId = 1,
            Plugin = new Plugin { PluginName = "Name" },
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

        // Act
        await _handler.Handle(new DeleteProjectPluginCommand(1, 1), It.IsAny<CancellationToken>());

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentActor(
                    It.IsAny<Project>(),
                    Action.REMOVED_PROJECT_PLUGIN,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockPluginRepository.Verify(
            m =>
                m.DeleteProjectPlugin(
                    It.Is<ProjectPlugin>(plugin => plugin.Id == 1 && plugin.DisplayName == "Test_1")
                ),
            Times.Once
        );
    }

    [Test]
    public async Task DeleteProjectPlugin_AuthorizationFailsThrowsTest()
    {
        _mockProjectsRepository
            .Setup(repo => repo.CheckProjectExists(It.IsAny<int>()))
            .ReturnsAsync(true);
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ProjectPlugin>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(false);

        var request = new DeleteProjectPluginCommand(1, 1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }

    [Test]
    public async Task DeleteProjectPlugin_ProjectNotFoundThrowsTest()
    {
        _mockProjectsRepository
            .Setup(repo => repo.CheckProjectExists(It.IsAny<int>()))
            .ReturnsAsync(false);

        var request = new DeleteProjectPluginCommand(1, 1);

        _ = Assert.ThrowsAsync<ProjectNotFoundException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
