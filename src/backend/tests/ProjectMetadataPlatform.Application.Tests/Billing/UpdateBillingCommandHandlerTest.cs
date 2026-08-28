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

namespace ProjectMetadataPlatform.Application.Tests.Billing;

[TestFixture]
public class UpdateBillingCommandHandlerTest
{
    private UpdateBillingCommandHandler _handler;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IBillingRepository> _mockBillingRepository;

    [SetUp]
    public void Setup()
    {
        _mockBillingRepository = new Mock<IBillingRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _handler = new UpdateBillingCommandHandler(
            billingRepository: _mockBillingRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task UpdateBilling_CallsRepositoryCorrectly()
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

        _ = _mockBillingRepository
            .Setup(repo => repo.StoreBillingInformation(It.IsAny<GlobalBilling>()))
            .ReturnsAsync((GlobalBilling billing) => billing);

        _ = _mockBillingRepository
            .Setup(repo => repo.CheckBillingKindExists(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            new UpdateBillingCommand(1, BillingKind: "Test_2", null, null, null, null, null),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(returnBilling));
        _mockBillingRepository.Verify(
            m => m.GetBillingByIdAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
        _mockBillingRepository.Verify(
            m =>
                m.StoreBillingInformation(
                    It.Is<GlobalBilling>(billing =>
                        billing.Id == 1 && billing.BillingKind == "Test_2"
                    )
                ),
            Times.Once
        );
        _mockLogRepo.Verify(
            m =>
                m.AddGlobalBillingLogForCurrentActor(
                    It.IsAny<GlobalBilling>(),
                    Action.UPDATED_GLOBAL_BILLING,
                    It.Is<List<LogChange>>(changes =>
                        changes[0].Property == "BillingKind"
                        && changes[0].NewValue == "Test_2"
                        && changes[0].OldValue == "Test_1"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task UpdateBilling_NoLogCreatedIfValuesAreEqual()
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

        _ = _mockBillingRepository
            .Setup(repo => repo.StoreBillingInformation(It.IsAny<GlobalBilling>()))
            .ReturnsAsync((GlobalBilling billing) => billing);

        _ = _mockBillingRepository
            .Setup(repo => repo.CheckBillingKindExists(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        _ = await _handler.Handle(
            new UpdateBillingCommand(1, BillingKind: "Test_1", null, null, null, null, null),
            It.IsAny<CancellationToken>()
        );

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddGlobalBillingLogForCurrentActor(
                    It.IsAny<GlobalBilling>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
    }

    [Test]
    public void UpdateBilling_ThrowsBillingKindAlreadyExistsException_IfNewBillingKindAlreadyExists()
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

        _ = _mockBillingRepository
            .Setup(repo => repo.StoreBillingInformation(It.IsAny<GlobalBilling>()))
            .ReturnsAsync((GlobalBilling billing) => billing);

        _ = _mockBillingRepository
            .Setup(repo => repo.CheckBillingKindExists(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act + Assert
        var ex = Assert.ThrowsAsync<BillingKindAlreadyExistsException>(async () =>
            await _handler.Handle(
                new UpdateBillingCommand(1, BillingKind: "Test_2", null, null, null, null, null),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test_2"));
    }

    [Test]
    public async Task EditBilling_AuthorizationFailsThrowsTest()
    {
        var returnBilling = new GlobalBilling() { Id = 1, BillingKind = "Test_1" };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<GlobalBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(false);
        _ = _mockBillingRepository
            .Setup(repo => repo.GetBillingByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(returnBilling);

        var request = new UpdateBillingCommand(
            1,
            BillingKind: "Test_2",
            "null",
            1,
            1,
            1,
            TimeFrame.YEARLY
        );

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
