using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.PluginBilling;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Billing;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.BillingExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.PluginBilling;

[TestFixture]
public class AddPluginBillingCommandHandlerTest
{
    private AddPluginBillingCommandHandler _handler;
    private Mock<IBillingRepository> _mockBillingRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IPluginRepository> _mockPluginRepo;

    [SetUp]
    public void Setup()
    {
        _mockBillingRepository = new Mock<IBillingRepository>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPluginRepo = new Mock<IPluginRepository>();
        _handler = new AddPluginBillingCommandHandler(
            billingRepository: _mockBillingRepository.Object,
            pluginRepository: _mockPluginRepo.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task AddPluginBilling_DoesNotAlreadyExists_WorksFine()
    {
        // Arrange
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Domain.Billing.PluginBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);

        _mockPluginRepo
            .Setup(m => m.GetProjectPluginAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(
                new Domain.Plugins.ProjectPlugin
                {
                    DisplayName = "Plugin",
                    PluginId = 1,
                    Url = "Url",
                }
            );
        _mockBillingRepository
            .Setup(m => m.GetBillingByIdAsNoTrackingAsync(It.IsAny<int>()))
            .ReturnsAsync(new GlobalBilling() { Id = 1, BillingKind = "Test_1" });
        _ = _mockBillingRepository
            .Setup(repo => repo.CheckPluginBillingExists(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);

        _ = _mockBillingRepository
            .Setup(repo => repo.AddPluginBilling(It.IsAny<Domain.Billing.PluginBilling>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(
            new AddPluginBillingCommand(1, 1, 1, null, "", 1, 1, 1, TimeFrame.NEVER, null, null),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.EqualTo((1, 1)));
        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentActor(
                    It.IsAny<Project>(),
                    Action.ADDED_PROJECT_PLUGIN_BILLING,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockBillingRepository.Verify(
            m => m.AddPluginBilling(It.IsAny<Domain.Billing.PluginBilling>()),
            Times.Once
        );
    }

    [Test]
    public void AddPluginBilling_AlreadyExists_ThrowsBillingAlreadyExistsException()
    {
        // Arrange
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Domain.Billing.PluginBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _mockBillingRepository
            .Setup(m => m.GetBillingByIdAsNoTrackingAsync(It.IsAny<int>()))
            .ReturnsAsync(new GlobalBilling() { Id = 1, BillingKind = "Test_1" });
        _mockPluginRepo
            .Setup(m => m.GetProjectPluginAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(
                new Domain.Plugins.ProjectPlugin
                {
                    DisplayName = "Plugin",
                    PluginId = 1,
                    Url = "Url",
                }
            );
        _ = _mockBillingRepository
            .Setup(repo => repo.CheckPluginBillingExists(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);
        // Act + Assert
        var ex = Assert.ThrowsAsync<PluginBillingAlreadyExistsException>(async () =>
            await _handler.Handle(
                new AddPluginBillingCommand(
                    1,
                    1,
                    1,
                    null,
                    "",
                    1,
                    1,
                    1,
                    TimeFrame.NEVER,
                    null,
                    null
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public void AddPluginBilling_DateMissing_ThrowsDateMissingException()
    {
        // Arrange
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Domain.Billing.PluginBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _mockBillingRepository
            .Setup(m => m.GetBillingByIdAsNoTrackingAsync(It.IsAny<int>()))
            .ReturnsAsync(new GlobalBilling() { Id = 1, BillingKind = "Test_1" });
        _mockPluginRepo
            .Setup(m => m.GetProjectPluginAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(
                new Domain.Plugins.ProjectPlugin
                {
                    DisplayName = "Plugin",
                    PluginId = 1,
                    Url = "Url",
                }
            );
        _ = _mockBillingRepository
            .Setup(repo => repo.CheckPluginBillingExists(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);
        // Act + Assert
        var ex = Assert.ThrowsAsync<PluginBillingDateMissingException>(async () =>
            await _handler.Handle(
                new AddPluginBillingCommand(1, 1, 1, null, "", 1, 1, 1, TimeFrame.DATE, null, null),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public void AddPluginBilling_NotesMoreThan280Chars_ThrowsNotesSizeException()
    {
        // Arrange
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Domain.Billing.PluginBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _mockBillingRepository
            .Setup(m => m.GetBillingByIdAsNoTrackingAsync(It.IsAny<int>()))
            .ReturnsAsync(new GlobalBilling() { Id = 1, BillingKind = "Test_1" });
        _mockPluginRepo
            .Setup(m => m.GetProjectPluginAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(
                new Domain.Plugins.ProjectPlugin
                {
                    DisplayName = "Plugin",
                    PluginId = 1,
                    Url = "Url",
                }
            );
        _ = _mockBillingRepository
            .Setup(repo => repo.CheckPluginBillingExists(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(false);
        // Act + Assert
        var ex = Assert.ThrowsAsync<PluginBillingNotesSizeException>(async () =>
            await _handler.Handle(
                new AddPluginBillingCommand(
                    1,
                    1,
                    1,
                    null,
                    "",
                    1,
                    1,
                    1,
                    TimeFrame.DATE,
                    new System.DateTimeOffset(),
                    new string('a', 281)
                ),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Test]
    public async Task AddPluginBilling_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Domain.Billing.PluginBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(false);

        var request = new AddPluginBillingCommand(
            1,
            1,
            1,
            null,
            "",
            1,
            1,
            1,
            TimeFrame.NEVER,
            null,
            null
        );

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
