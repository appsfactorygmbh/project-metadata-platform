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
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.ProjectPlugins;

[TestFixture]
public class AddProjectPluginCommandHandlerTest
{
    private AddProjectPluginCommandHandler _handler;
    private Mock<IProjectsRepository> _mockProjectsRepository;
    private Mock<IPluginRepository> _mockPluginRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _mockPluginRepository = new Mock<IPluginRepository>();
        _mockProjectsRepository = new Mock<IProjectsRepository>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new AddProjectPluginCommandHandler(
            projectsRepository: _mockProjectsRepository.Object,
            pluginRepository: _mockPluginRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task AddProjectPlugin_UrlDoesNotAlreadyExists_WorksFine()
    {
        // Arrange
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
            .Setup(repo =>
                repo.CheckProjectPluginExists(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            )
            .ReturnsAsync(false);

        _ = _mockPluginRepository
            .Setup(repo => repo.StoreProjectPlugin(It.IsAny<ProjectPlugin>()))
            .Callback(
                (ProjectPlugin pluginBeingAdded) =>
                {
                    pluginBeingAdded.Id = 1;
                }
            )
            .ReturnsAsync(
                (ProjectPlugin plugin) =>
                {
                    return plugin;
                }
            );

        // Act
        var result = await _handler.Handle(
            new AddProjectPluginCommand(1, 1, "Test Name", "Test Url"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.EqualTo(1));
        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentActor(
                    It.IsAny<Project>(),
                    Action.ADDED_PROJECT_PLUGIN,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockPluginRepository.Verify(
            m =>
                m.StoreProjectPlugin(
                    It.Is<ProjectPlugin>(plugin => plugin.DisplayName == "Test Name")
                ),
            Times.Once
        );
    }

    [Test]
    public void AddProjectPlugin_UrlAlreadyExists_ProjectPluginAlreadyExistsException()
    {
        // Arrange
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
            .Setup(repo =>
                repo.CheckProjectPluginExists(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            )
            .ReturnsAsync(true);
        // Act + Assert
        var ex = Assert.ThrowsAsync<ProjectPluginAlreadyExistsException>(async () =>
            await _handler.Handle(
                new AddProjectPluginCommand(1, 1, "Test Name", "Test Url"),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test Url"));
    }

    [Test]
    public async Task AddProjectPlugin_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<ProjectPlugin>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(false);

        var request = new AddProjectPluginCommand(1, 1, "Test Name", "Test Url");

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
