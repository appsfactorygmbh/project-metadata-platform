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
using ProjectMetadataPlatform.Domain.Errors.BillingExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Billing;

[TestFixture]
public class CreateBillingCommandHandlerTest
{
    private CreateBillingCommandHandler _handler;
    private Mock<IBillingRepository> _mockBillingRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _mockBillingRepository = new Mock<IBillingRepository>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateBillingCommandHandler(
            billingRepository: _mockBillingRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task CreateBilling_KindDoesNotAlreadyExists_WorksFine()
    {
        // Arrange
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
            .Setup(repo => repo.CheckBillingKindExists(It.IsAny<string>()))
            .ReturnsAsync(false);

        _ = _mockBillingRepository
            .Setup(repo => repo.StoreBillingInformation(It.IsAny<GlobalBilling>()))
            .Callback(
                (GlobalBilling billingBeingAdded) =>
                {
                    billingBeingAdded.Id = 1;
                }
            )
            .ReturnsAsync(
                (GlobalBilling billing) =>
                {
                    return billing;
                }
            );

        // Act
        var result = await _handler.Handle(
            new CreateBillingCommand(BillingKind: "Test Name", "", 1, 1, 1, TimeFrame.MONTHLY),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.EqualTo(1));
        _mockLogRepo.Verify(
            m =>
                m.AddGlobalBillingLogForCurrentActor(
                    It.IsAny<GlobalBilling>(),
                    Action.ADDED_GLOBAL_BILLING,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockBillingRepository.Verify(
            m =>
                m.StoreBillingInformation(
                    It.Is<GlobalBilling>(billing => billing.BillingKind == "Test Name")
                ),
            Times.Once
        );
    }

    [Test]
    public void CreateBilling_KindAlreadyExists_ThrowsBillingKindAlreadyExistsException()
    {
        // Arrange
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
            .Setup(repo => repo.CheckBillingKindExists(It.IsAny<string>()))
            .ReturnsAsync(true);
        // Act + Assert
        var ex = Assert.ThrowsAsync<BillingKindAlreadyExistsException>(async () =>
            await _handler.Handle(
                new CreateBillingCommand(BillingKind: "Test Name", null, null, null, null, null),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test Name"));
    }

    [Test]
    public async Task CreateBilling_AuthorizationFailsThrowsTest()
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

        var request = new CreateBillingCommand(
            BillingKind: "Test Name",
            null,
            null,
            null,
            null,
            null
        );
        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
