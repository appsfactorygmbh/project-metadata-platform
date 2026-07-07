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
public class CreatePluginCommandHandlerTest
{
    private CreatePluginCommandHandler _handler;
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
        _handler = new CreatePluginCommandHandler(
            _mockPluginRepo.Object,
            _mockLogRepo.Object,
            _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task CreatePlugin_Test()
    {
        _ = _mockPluginRepo
            .Setup(m => m.StorePlugin(It.IsAny<Plugin>()))
            .Callback<Plugin>(p => p.Id = 13);
        _ = _mockPluginRepo
            .Setup(m => m.CheckGlobalPluginNameExists("Airlock"))
            .ReturnsAsync(false);
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
                    { AuthorizationConstants.Actions.CREATE, true },
                }
            );
        var result = await _handler.Handle(
            new CreatePluginCommand("Airlock", true, [], "https://airlock.com"),
            It.IsAny<CancellationToken>()
        );
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(13));
        });

        _mockLogRepo.Verify(
            logRepo =>
                logRepo.AddGlobalPluginLogForCurrentActor(
                    It.Is<Plugin>(plugin =>
                        plugin.PluginName == "Airlock" && plugin.IsArchived && plugin.Id == 13
                    ),
                    Action.ADDED_GLOBAL_PLUGIN,
                    It.Is<List<LogChange>>(changes =>
                        changes.Any(change =>
                            change.Property == "PluginName"
                            && change.OldValue == ""
                            && change.NewValue == "Airlock"
                        )
                        && changes.Any(change =>
                            change.Property == "IsArchived"
                            && change.OldValue == ""
                            && change.NewValue == "True"
                        )
                        && changes.Any(change =>
                            change.Property == "BaseUrl"
                            && change.OldValue == ""
                            && change.NewValue == "https://airlock.com"
                        )
                    )
                ),
            Times.Once
        );
        _mockPluginRepo.Verify(r => r.CheckGlobalPluginNameExists("Airlock"), Times.Once);
    }

    [Test]
    public void CreatePlugin_NameConflict_Test()
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
                    { AuthorizationConstants.Actions.CREATE, true },
                }
            );
        _ = _mockPluginRepo.Setup(m => m.CheckGlobalPluginNameExists("Airlock")).ReturnsAsync(true);

        _ = Assert.ThrowsAsync<PluginNameAlreadyExistsException>(() =>
            _handler.Handle(
                new CreatePluginCommand("Airlock", true, [], "https://airlock.com"),
                It.IsAny<CancellationToken>()
            )
        );
        _mockPluginRepo.Verify(r => r.CheckGlobalPluginNameExists("Airlock"), Times.Once);
    }

    [Test]
    public async Task CreatePlugin_AuthorizationFailsThrowsTest()
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
                    { AuthorizationConstants.Actions.CREATE, false },
                }
            );

        var request = new CreatePluginCommand("Airlock", true, [], "https://airlock.com");

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
