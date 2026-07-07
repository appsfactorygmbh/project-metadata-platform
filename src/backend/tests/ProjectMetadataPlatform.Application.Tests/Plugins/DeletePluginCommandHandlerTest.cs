using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;

namespace ProjectMetadataPlatform.Application.Tests.Plugins;

[TestFixture]
public class DeletePluginCommandHandlerTest
{
    private DeleteGlobalPluginCommandHandler _handler;
    private Mock<IPluginRepository> _mockPluginRepo;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockPluginRepo = new Mock<IPluginRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteGlobalPluginCommandHandler(
            _mockPluginRepo.Object,
            _mockLogRepo.Object,
            _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task DeleteGlobalPlugin_Test()
    {
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Flat-Earth",
            IsArchived = true,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Plugin>(),
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
        _ = _mockPluginRepo.Setup(m => m.StorePlugin(It.IsAny<Plugin>())).ReturnsAsync(plugin);
        _ = _mockPluginRepo.Setup(m => m.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _ = _mockPluginRepo.Setup(m => m.DeleteGlobalPlugin(plugin)).ReturnsAsync(true);

        var result = await _handler.Handle(
            new DeleteGlobalPluginCommand(42),
            It.IsAny<CancellationToken>()
        );
        Assert.That(result, Is.EqualTo(true));
    }

    [Test]
    public void DeleteGlobalPluginNotArchived_Test()
    {
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Flat-Earth",
            IsArchived = false,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Plugin>(),
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
        _ = _mockPluginRepo.Setup(m => m.StorePlugin(It.IsAny<Plugin>())).ReturnsAsync(plugin);
        _ = _mockPluginRepo.Setup(m => m.GetPluginByIdAsync(42)).ReturnsAsync(plugin);

        _ = Assert.ThrowsAsync<PluginNotArchivedException>(() =>
            _handler.Handle(new DeleteGlobalPluginCommand(42), It.IsAny<CancellationToken>())
        );
    }

    [Test]
    public void DeleteGlobalPluginNullPointerException_Test()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Plugin>(),
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
        _ = _mockPluginRepo.Setup(m => m.GetPluginByIdAsync(42)).ReturnsAsync((Plugin)null!);
        _ = Assert.ThrowsAsync<PluginNotFoundException>(() =>
            _handler.Handle(new DeleteGlobalPluginCommand(42), It.IsAny<CancellationToken>())
        );
    }

    [Test]
    public async Task DeleteGlobalPlugin_LogsAction_WhenPluginIsArchived()
    {
        // Arrange
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Flat-Earth",
            IsArchived = true,
        };
        var changes = new List<LogChange>();
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Plugin>(),
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
        _ = _mockPluginRepo.Setup(m => m.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _ = _mockPluginRepo.Setup(m => m.DeleteGlobalPlugin(plugin)).ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(
            new DeleteGlobalPluginCommand(42),
            CancellationToken.None
        );

        // Assert
        Assert.That(result, Is.EqualTo(true));
        var addLogCall = _mockLogRepo.Invocations.FirstOrDefault(i =>
            i.Method.Name == nameof(ILogRepository.AddGlobalPluginLogForCurrentActor)
        );
        Assert.That(addLogCall, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(addLogCall.Arguments[0], Is.EqualTo(plugin));
            Assert.That(addLogCall.Arguments[1], Is.EqualTo(Action.REMOVED_GLOBAL_PLUGIN));
            Assert.That(addLogCall.Arguments[2], Is.EqualTo(changes));
        });
    }

    [Test]
    public void DeleteGlobalPlugin_DoesNotLogAction_WhenPluginIsNotArchived()
    {
        // Arrange
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Flat-Earth",
            IsArchived = false,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Plugin>(),
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
        _ = _mockPluginRepo.Setup(m => m.GetPluginByIdAsync(42)).ReturnsAsync(plugin);

        // Act
        _ = Assert.ThrowsAsync<PluginNotArchivedException>(() =>
            _handler.Handle(new DeleteGlobalPluginCommand(42), CancellationToken.None)
        );
        var addLogCall = _mockLogRepo.Invocations.FirstOrDefault(i =>
            i.Method.Name == nameof(ILogRepository.AddGlobalPluginLogForCurrentActor)
        );
        Assert.That(addLogCall, Is.Null);
    }

    [Test]
    public void DeleteGlobalPlugin_DoesNotLogAction_WhenPluginIsNull()
    {
        // Arrange
        _ = _mockPluginRepo.Setup(m => m.GetPluginByIdAsync(42)).ReturnsAsync((Plugin)null!);
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Plugin>(),
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
        // Act
        _ = Assert.ThrowsAsync<PluginNotFoundException>(() =>
            _handler.Handle(new DeleteGlobalPluginCommand(42), CancellationToken.None)
        );
        var addLogCall = _mockLogRepo.Invocations.FirstOrDefault(i =>
            i.Method.Name == nameof(ILogRepository.AddGlobalPluginLogForCurrentActor)
        );
        Assert.That(addLogCall, Is.Null);
    }

    [Test]
    public async Task DeletePlugin_AuthorizationFailsThrowsTest()
    {
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Flat-Earth",
            IsArchived = false,
        };
        _ = _mockPluginRepo.Setup(m => m.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Plugin>(),
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

        var request = new DeleteGlobalPluginCommand(42);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
