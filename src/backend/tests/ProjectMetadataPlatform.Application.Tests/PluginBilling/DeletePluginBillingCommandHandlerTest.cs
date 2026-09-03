using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.PluginBilling;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Projects;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.PluginBilling;

[TestFixture]
public class DeletePluginBillingCommandHandlerTest
{
    private DeletePluginBillingCommandHandler _handler;
    private Mock<IBillingRepository> _mockPluginBillingRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockPluginBillingRepository = new Mock<IBillingRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeletePluginBillingCommandHandler(
            billingRepository: _mockPluginBillingRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task DeletePluginBilling_WorksFine()
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
        await _handler.Handle(new DeletePluginBillingCommand(1, 1), It.IsAny<CancellationToken>());

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddProjectLogForCurrentActor(
                    It.IsAny<Project>(),
                    Action.REMOVED_PROJECT_PLUGIN_BILLING,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockPluginBillingRepository.Verify(
            m =>
                m.DeletePluginBillingAsync(
                    It.Is<Domain.Billing.PluginBilling>(billing =>
                        billing.ProjectId == 1 && billing.PluginId == 1
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task DeletePluginBilling_AuthorizationFailsThrowsTest()
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

        var request = new DeletePluginBillingCommand(1, 1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
