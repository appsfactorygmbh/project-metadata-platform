using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Plugins;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Plugins;

[TestFixture]
public class PatchGlobalPluginCommandHandlerTest
{
    private PatchGlobalPluginCommandHandler _handler;
    private Mock<IPluginRepository> _mockPluginRepo;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockPluginRepo = new Mock<IPluginRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _handler = new PatchGlobalPluginCommandHandler(
            _mockPluginRepo.Object,
            _mockLogRepo.Object,
            _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task PatchGlobalPlugin_UpdatePluginName_Test()
    {
        // Arrange
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Mercury Redstone",
            IsArchived = false,
        };

        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo
            .Setup(repo => repo.CheckGlobalPluginNameExists("Mercury Atlas"))
            .ReturnsAsync(false);
        _mockPluginRepo
            .Setup(repo => repo.StorePlugin(It.IsAny<Plugin>()))
            .ReturnsAsync((Plugin p) => p);

        List<LogChange>? capturedLogChanges = null;

        _mockLogRepo
            .Setup(m =>
                m.AddGlobalPluginLogForCurrentUser(
                    It.IsAny<Plugin>(),
                    Action.UPDATED_GLOBAL_PLUGIN,
                    It.IsAny<List<LogChange>>()
                )
            )
            .Callback<Plugin, Action, List<LogChange>>(
                (_, _, logChanges) => capturedLogChanges = logChanges
            );

        // Act
        var result = await _handler.Handle(
            new PatchGlobalPluginCommand(42, "Mercury Atlas"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(capturedLogChanges, Is.Not.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(result.PluginName, Is.EqualTo("Mercury Atlas"));
            Assert.That(result.IsArchived, Is.EqualTo(plugin.IsArchived));
            Assert.That(result.Id, Is.EqualTo(plugin.Id));

            _mockLogRepo.Verify(
                m =>
                    m.AddGlobalPluginLogForCurrentUser(
                        It.IsAny<Plugin>(),
                        Action.UPDATED_GLOBAL_PLUGIN,
                        It.IsAny<List<LogChange>>()
                    ),
                Times.Once
            );

            Assert.That(capturedLogChanges, Has.Count.EqualTo(1));
            var firstLogChange = capturedLogChanges!.First();
            Assert.That(firstLogChange, Is.Not.Null);
            Assert.That(firstLogChange.Property, Is.EqualTo(nameof(plugin.PluginName)));
            Assert.That(firstLogChange.OldValue, Is.EqualTo("Mercury Redstone"));
            Assert.That(firstLogChange.NewValue, Is.EqualTo("Mercury Atlas"));

            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        });

        _mockPluginRepo.Verify(r => r.CheckGlobalPluginNameExists("Mercury Atlas"), Times.Once);
    }

    [Test]
    public async Task PatchGlobalPlugin_ArchivePlugin_Test()
    {
        // Arrange
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Mercury Redstone",
            IsArchived = false,
        };

        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo
            .Setup(repo => repo.StorePlugin(It.IsAny<Plugin>()))
            .ReturnsAsync((Plugin p) => p);

        List<LogChange>? capturedLogChanges = null;

        _mockLogRepo
            .Setup(m =>
                m.AddGlobalPluginLogForCurrentUser(
                    It.IsAny<Plugin>(),
                    Action.ARCHIVED_GLOBAL_PLUGIN,
                    It.IsAny<List<LogChange>>()
                )
            )
            .Callback<Plugin, Action, List<LogChange>>(
                (_, _, logChanges) => capturedLogChanges = logChanges
            );

        // Act
        var result = await _handler.Handle(
            new PatchGlobalPluginCommand(42, null, true),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(capturedLogChanges, Is.Not.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(result.IsArchived, Is.True);
            Assert.That(result.PluginName, Is.EqualTo(plugin.PluginName));
            Assert.That(result.Id, Is.EqualTo(plugin.Id));

            _mockLogRepo.Verify(
                m =>
                    m.AddGlobalPluginLogForCurrentUser(
                        It.IsAny<Plugin>(),
                        Action.ARCHIVED_GLOBAL_PLUGIN,
                        It.IsAny<List<LogChange>>()
                    ),
                Times.Once
            );

            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);

            Assert.That(capturedLogChanges, Is.Empty);
        });
    }

    [Test]
    public async Task PatchGlobalPlugin_ChangeNothing_Test()
    {
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Mercury Redstone",
            IsArchived = false,
        };

        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo
            .Setup(repo => repo.StorePlugin(It.IsAny<Plugin>()))
            .ReturnsAsync((Plugin p) => p);

        List<LogChange>? capturedLogChanges = null;

        _mockLogRepo
            .Setup(m =>
                m.AddGlobalPluginLogForCurrentUser(
                    It.IsAny<Plugin>(),
                    Action.UPDATED_GLOBAL_PLUGIN,
                    It.IsAny<List<LogChange>>()
                )
            )
            .Callback<Plugin, Action, List<LogChange>>(
                (_, _, logChanges) => capturedLogChanges = logChanges
            );

        var result = await _handler.Handle(
            new PatchGlobalPluginCommand(42),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.Not.Null);

        await TestContext.Out.WriteLineAsync(
            $"[Test Debug] Result PluginName: {result.PluginName}, IsArchived: {result.IsArchived}"
        );

        Assert.Multiple(() =>
        {
            Assert.That(result.IsArchived, Is.EqualTo(plugin.IsArchived));
            Assert.That(result.PluginName, Is.EqualTo(plugin.PluginName));
            Assert.That(result.Id, Is.EqualTo(plugin.Id));

            Assert.That(capturedLogChanges, Is.Null, "Expected no log changes to be captured.");
        });
    }

    [Test]
    public async Task PatchGlobalPlugin_UnarchivePlugin_Test()
    {
        // Arrange
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Mercury Redstone",
            IsArchived = true,
        };

        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo
            .Setup(repo => repo.StorePlugin(It.IsAny<Plugin>()))
            .ReturnsAsync((Plugin p) => p);

        List<LogChange>? capturedLogChanges = null;

        _mockLogRepo
            .Setup(m =>
                m.AddGlobalPluginLogForCurrentUser(
                    It.IsAny<Plugin>(),
                    Action.UNARCHIVED_GLOBAL_PLUGIN,
                    It.IsAny<List<LogChange>>()
                )
            )
            .Callback<Plugin, Action, List<LogChange>>(
                (_, _, logChanges) => capturedLogChanges = logChanges
            );

        // Act
        var result = await _handler.Handle(
            new PatchGlobalPluginCommand(42, null, false),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(capturedLogChanges, Is.Not.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(result.IsArchived, Is.False);
            Assert.That(result.PluginName, Is.EqualTo(plugin.PluginName));
            Assert.That(result.Id, Is.EqualTo(plugin.Id));

            _mockLogRepo.Verify(
                m =>
                    m.AddGlobalPluginLogForCurrentUser(
                        It.IsAny<Plugin>(),
                        Action.UNARCHIVED_GLOBAL_PLUGIN,
                        It.IsAny<List<LogChange>>()
                    ),
                Times.Once
            );

            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);

            Assert.That(capturedLogChanges, Is.Empty);
        });
    }

    [Test]
    public void PatchGlobalPlugin_NotFound_Test()
    {
        // Arrange
        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync((Plugin?)null);

        // Assert
        Assert.ThrowsAsync<PluginNotFoundException>(() =>
            _handler.Handle(new PatchGlobalPluginCommand(42), It.IsAny<CancellationToken>())
        );
    }

    [Test]
    public async Task PatchGlobalPlugin_UpdateBaserUrl_Test()
    {
        // Arrange
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Mercury Redstone",
            IsArchived = false,
            BaseUrl = "https://mercuryredstone.com",
        };

        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo
            .Setup(repo => repo.StorePlugin(It.IsAny<Plugin>()))
            .ReturnsAsync((Plugin p) => p);

        List<LogChange>? capturedLogChanges = null;

        _mockLogRepo
            .Setup(m =>
                m.AddGlobalPluginLogForCurrentUser(
                    It.IsAny<Plugin>(),
                    Action.UPDATED_GLOBAL_PLUGIN,
                    It.IsAny<List<LogChange>>()
                )
            )
            .Callback<Plugin, Action, List<LogChange>>(
                (_, _, logChanges) => capturedLogChanges = logChanges
            );

        // Act
        var result = await _handler.Handle(
            new PatchGlobalPluginCommand(42, null, null, "https://mercuryatlas.com"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(capturedLogChanges, Is.Not.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(result.BaseUrl, Is.EqualTo("https://mercuryatlas.com"));
            Assert.That(result.PluginName, Is.EqualTo(plugin.PluginName));
            Assert.That(result.IsArchived, Is.EqualTo(plugin.IsArchived));
            Assert.That(result.Id, Is.EqualTo(plugin.Id));

            _mockLogRepo.Verify(
                m =>
                    m.AddGlobalPluginLogForCurrentUser(
                        It.IsAny<Plugin>(),
                        Action.UPDATED_GLOBAL_PLUGIN,
                        It.IsAny<List<LogChange>>()
                    ),
                Times.Once
            );

            Assert.That(capturedLogChanges, Has.Count.EqualTo(1));
            var firstLogChange = capturedLogChanges!.First();
            Assert.That(firstLogChange, Is.Not.Null);
            Assert.That(firstLogChange.Property, Is.EqualTo(nameof(plugin.BaseUrl)));
            Assert.That(firstLogChange.OldValue, Is.EqualTo("https://mercuryredstone.com"));
            Assert.That(firstLogChange.NewValue, Is.EqualTo("https://mercuryatlas.com"));

            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        });
    }

    [Test]
    public void PatchGlobalPlugin_NameUpdatedButAlreadyUsedInAnotherPlugin_Test()
    {
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Mercury Redstone",
            IsArchived = false,
            BaseUrl = "https://mercuryredstone.com",
        };
        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);

        _mockPluginRepo
            .Setup(repo => repo.CheckGlobalPluginNameExists("Atlas Agena"))
            .ReturnsAsync(true);
        Assert.ThrowsAsync<PluginNameAlreadyExistsException>(() =>
            _handler.Handle(
                new PatchGlobalPluginCommand(42, "Atlas Agena"),
                It.IsAny<CancellationToken>()
            )
        );
        _mockPluginRepo.Verify(repo => repo.CheckGlobalPluginNameExists("Atlas Agena"), Times.Once);
    }

    [Test]
    public async Task PatchGlobalPlugin_UpdatePlugin_NameNotChangedButProvided_Test()
    {
        // Arrange
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Mercury Redstone",
            IsArchived = false,
        };

        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo
            .Setup(repo => repo.StorePlugin(It.IsAny<Plugin>()))
            .ReturnsAsync((Plugin p) => p);

        List<LogChange>? capturedLogChanges = null;

        _mockLogRepo
            .Setup(m =>
                m.AddGlobalPluginLogForCurrentUser(
                    It.IsAny<Plugin>(),
                    Action.UPDATED_GLOBAL_PLUGIN,
                    It.IsAny<List<LogChange>>()
                )
            )
            .Callback<Plugin, Action, List<LogChange>>(
                (_, _, logChanges) => capturedLogChanges = logChanges
            );

        // Act
        var result = await _handler.Handle(
            new PatchGlobalPluginCommand(42, "Mercury Redstone"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(capturedLogChanges, Is.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(result.PluginName, Is.EqualTo("Mercury Redstone"));
            Assert.That(result.IsArchived, Is.EqualTo(plugin.IsArchived));
            Assert.That(result.Id, Is.EqualTo(plugin.Id));

            _mockLogRepo.Verify(
                m =>
                    m.AddGlobalPluginLogForCurrentUser(
                        It.IsAny<Plugin>(),
                        Action.UPDATED_GLOBAL_PLUGIN,
                        It.IsAny<List<LogChange>>()
                    ),
                Times.Never
            );

            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        });

        _mockPluginRepo.Verify(r => r.CheckGlobalPluginNameExists("Mercury Redstone"), Times.Never);
    }

    [Test]
    public async Task PatchGlobalPlugin_UpdatePlugin_NameChangeInCasing_Test()
    {
        // Arrange
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Vega c",
            IsArchived = false,
        };

        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo
            .Setup(repo => repo.StorePlugin(It.IsAny<Plugin>()))
            .ReturnsAsync((Plugin p) => p);

        List<LogChange>? capturedLogChanges = null;

        _mockLogRepo
            .Setup(m =>
                m.AddGlobalPluginLogForCurrentUser(
                    It.IsAny<Plugin>(),
                    Action.UPDATED_GLOBAL_PLUGIN,
                    It.IsAny<List<LogChange>>()
                )
            )
            .Callback<Plugin, Action, List<LogChange>>(
                (_, _, logChanges) => capturedLogChanges = logChanges
            );

        // Act
        var result = await _handler.Handle(
            new PatchGlobalPluginCommand(42, "VEGA C"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(capturedLogChanges, Is.Not.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(result.PluginName, Is.EqualTo("VEGA C"));
            Assert.That(result.IsArchived, Is.EqualTo(plugin.IsArchived));
            Assert.That(result.Id, Is.EqualTo(plugin.Id));

            _mockLogRepo.Verify(
                m =>
                    m.AddGlobalPluginLogForCurrentUser(
                        It.IsAny<Plugin>(),
                        Action.UPDATED_GLOBAL_PLUGIN,
                        It.IsAny<List<LogChange>>()
                    ),
                Times.Once
            );

            Assert.That(capturedLogChanges, Has.Count.EqualTo(1));
            var firstLogChange = capturedLogChanges!.First();
            Assert.That(firstLogChange, Is.Not.Null);
            Assert.That(firstLogChange.Property, Is.EqualTo(nameof(plugin.PluginName)));
            Assert.That(firstLogChange.OldValue, Is.EqualTo("Vega c"));
            Assert.That(firstLogChange.NewValue, Is.EqualTo("VEGA C"));

            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        });

        _mockPluginRepo.Verify(r => r.CheckGlobalPluginNameExists("VEGA C"), Times.Never);
    }

    [Test]
    public async Task PatchGlobalPlugin_UpdatePlugin_UrlNotChangedButProvided_Test()
    {
        // Arrange
        var plugin = new Plugin
        {
            Id = 42,
            PluginName = "Mercury Redstone",
            BaseUrl = "https://mercury.redstone",
            IsArchived = false,
        };

        _mockPluginRepo.Setup(repo => repo.GetPluginByIdAsync(42)).ReturnsAsync(plugin);
        _mockPluginRepo
            .Setup(repo => repo.StorePlugin(It.IsAny<Plugin>()))
            .ReturnsAsync((Plugin p) => p);

        List<LogChange>? capturedLogChanges = null;

        _mockLogRepo
            .Setup(m =>
                m.AddGlobalPluginLogForCurrentUser(
                    It.IsAny<Plugin>(),
                    Action.UPDATED_GLOBAL_PLUGIN,
                    It.IsAny<List<LogChange>>()
                )
            )
            .Callback<Plugin, Action, List<LogChange>>(
                (_, _, logChanges) => capturedLogChanges = logChanges
            );

        // Act
        var result = await _handler.Handle(
            new PatchGlobalPluginCommand(42, null, false, "https://mercury.redstone"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(capturedLogChanges, Is.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(result.PluginName, Is.EqualTo("Mercury Redstone"));
            Assert.That(result.IsArchived, Is.EqualTo(plugin.IsArchived));
            Assert.That(result.Id, Is.EqualTo(plugin.Id));
            Assert.That(result.BaseUrl, Is.EqualTo(plugin.BaseUrl));

            _mockLogRepo.Verify(
                m =>
                    m.AddGlobalPluginLogForCurrentUser(
                        It.IsAny<Plugin>(),
                        Action.UPDATED_GLOBAL_PLUGIN,
                        It.IsAny<List<LogChange>>()
                    ),
                Times.Never
            );

            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        });
    }
}
