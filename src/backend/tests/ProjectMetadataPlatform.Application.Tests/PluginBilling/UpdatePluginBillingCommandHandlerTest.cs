using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.PluginBilling;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.BillingExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.PluginBilling;

[TestFixture]
public class UpdatePluginBillingCommandHandlerTest
{
    private UpdatePluginBillingCommandHandler _handler;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IBillingRepository> _mockPluginBillingRepository;

    [SetUp]
    public void Setup()
    {
        _mockPluginBillingRepository = new Mock<IBillingRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _handler = new UpdatePluginBillingCommandHandler(
            billingRepository: _mockPluginBillingRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task UpdatePluginBilling_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnPluginBilling = new Domain.Billing.PluginBilling()
        {
            PluginId = 1,
            ProjectId = 1,
            Currency = "",
            BudgetLimit = 1,
            HostingFee = 1,
            TargetMargin = 0,
            TimeFrame = Domain.Billing.TimeFrame.NEVER,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Domain.Billing.PluginBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);

        _ = _mockPluginBillingRepository
            .Setup(repo => repo.GetPluginBillingByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(returnPluginBilling);

        // Act
        var result = await _handler.Handle(
            new UpdatePluginBillingCommand(
                1,
                1,
                "A",
                "",
                1,
                1,
                1,
                Domain.Billing.TimeFrame.NEVER,
                null,
                null
            ),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(returnPluginBilling));
        _mockPluginBillingRepository.Verify(
            m => m.GetPluginBillingByIdAsync(It.Is<int>(id => id == 1), It.Is<int>(id => id == 1)),
            Times.Once
        );
        _mockPluginBillingRepository.Verify(
            m =>
                m.UpdatePluginBilling(
                    It.Is<Domain.Billing.PluginBilling>(billing =>
                        billing.ProjectId == 1 && billing.PluginId == 1
                    )
                ),
            Times.Once
        );
        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentActor(
                    It.IsAny<Project>(),
                    Action.UPDATED_PROJECT_PLUGIN_BILLING,
                    It.Is<List<LogChange>>(changes =>
                        changes[0].Property == "DisplayName"
                        && changes[0].NewValue == "A"
                        && changes[0].OldValue == "null"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task UpdatePluginBilling_NoLogCreatedIfValuesAreEqual()
    {
        // Arrange
        var returnPluginBilling = new Domain.Billing.PluginBilling()
        {
            PluginId = 1,
            ProjectId = 1,
            Currency = "",
            BudgetLimit = 1,
            HostingFee = 1,
            TargetMargin = 0,
            TimeFrame = Domain.Billing.TimeFrame.NEVER,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Domain.Billing.PluginBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _mockPluginBillingRepository
            .Setup(repo => repo.GetPluginBillingByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(returnPluginBilling);

        // Act
        _ = await _handler.Handle(
            new UpdatePluginBillingCommand(
                1,
                1,
                null,
                "",
                1,
                1,
                0,
                Domain.Billing.TimeFrame.NEVER,
                null,
                null
            ),
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
    public async Task EditPluginBilling_DateMissing_ThrowsTest()
    {
        var returnPluginBilling = new Domain.Billing.PluginBilling()
        {
            PluginId = 1,
            ProjectId = 1,
            Currency = "",
            BudgetLimit = 1,
            HostingFee = 1,
            TargetMargin = 0,
            TimeFrame = Domain.Billing.TimeFrame.NEVER,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Domain.Billing.PluginBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _mockPluginBillingRepository
            .Setup(repo => repo.GetPluginBillingByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(returnPluginBilling);

        var request = new UpdatePluginBillingCommand(
            1,
            1,
            null,
            "",
            1,
            1,
            1,
            Domain.Billing.TimeFrame.DATE,
            null,
            null
        );

        _ = Assert.ThrowsAsync<PluginBillingDateMissingException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }

    [Test]
    public async Task EditPluginBilling_NotesMoreThan280Chars_ThrowsTest()
    {
        var returnPluginBilling = new Domain.Billing.PluginBilling()
        {
            PluginId = 1,
            ProjectId = 1,
            Currency = "",
            BudgetLimit = 1,
            HostingFee = 1,
            TargetMargin = 0,
            TimeFrame = Domain.Billing.TimeFrame.NEVER,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Domain.Billing.PluginBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _mockPluginBillingRepository
            .Setup(repo => repo.GetPluginBillingByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(returnPluginBilling);

        var request = new UpdatePluginBillingCommand(
            1,
            1,
            null,
            "",
            1,
            1,
            1,
            Domain.Billing.TimeFrame.QUARTERLY,
            null,
            new string('a', 281)
        );

        _ = Assert.ThrowsAsync<PluginBillingNotesSizeException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }

    [Test]
    public async Task EditPluginBilling_AuthorizationFailsThrowsTest()
    {
        var returnPluginBilling = new Domain.Billing.PluginBilling()
        {
            PluginId = 1,
            ProjectId = 1,
            Currency = "",
            BudgetLimit = 1,
            HostingFee = 1,
            TargetMargin = 0,
            TimeFrame = Domain.Billing.TimeFrame.NEVER,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Domain.Billing.PluginBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(false);
        _ = _mockPluginBillingRepository
            .Setup(repo => repo.GetPluginBillingByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(returnPluginBilling);

        var request = new UpdatePluginBillingCommand(
            1,
            1,
            null,
            "",
            1,
            1,
            1,
            Domain.Billing.TimeFrame.NEVER,
            null,
            null
        );

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
