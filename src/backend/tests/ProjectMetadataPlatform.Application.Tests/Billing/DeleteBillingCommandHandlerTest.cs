using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Billing;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Billing;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Billing;

[TestFixture]
public class DeleteBillingCommandHandlerTest
{
    private DeleteBillingCommandHandler _handler;
    private Mock<IBillingRepository> _mockBillingRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockBillingRepository = new Mock<IBillingRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteBillingCommandHandler(
            billingRepository: _mockBillingRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task DeleteBilling_WorksFine()
    {
        // Arrange
        var returnBilling = new GlobalBilling() { Id = 1, BillingKind = "Test_1" };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<GlobalBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _mockBillingRepository
            .Setup(repo => repo.GetBillingByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(returnBilling);

        // Act
        await _handler.Handle(new DeleteBillingCommand(Id: 1), It.IsAny<CancellationToken>());

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddGlobalBillingLogForCurrentActor(
                    It.IsAny<GlobalBilling>(),
                    Action.REMOVED_GLOBAL_BILLING,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockBillingRepository.Verify(
            m =>
                m.DeleteBillingAsync(
                    It.Is<GlobalBilling>(billing =>
                        billing.Id == 1 && billing.BillingKind == "Test_1"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task DeleteBilling_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<GlobalBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(false);

        var request = new DeleteBillingCommand(Id: 1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
