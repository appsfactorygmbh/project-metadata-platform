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

namespace ProjectMetadataPlatform.Application.Tests.Billing;

[TestFixture]
public class GetBillingByIdQueryHandlerTest
{
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private GetBillingByIdQueryHandler _handler;
    private Mock<IBillingRepository> _mockBillingRepository;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockBillingRepository = new Mock<IBillingRepository>();
        _handler = new GetBillingByIdQueryHandler(
            billingRepository: _mockBillingRepository.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetBilling_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnBilling = new GlobalBilling() { Id = 1, BillingKind = "Test_1" };

        _ = _mockBillingRepository
            .Setup(repo => repo.GetBillingByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(returnBilling);
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<GlobalBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        // Act
        var result = await _handler.Handle(
            new GetBillingByIdQuery(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Item1, Is.Not.Null);
        Assert.That(result.Item1, Is.EqualTo(returnBilling));
        _mockBillingRepository.Verify(
            m => m.GetBillingByIdAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
    }

    [Test]
    public void GetBilling_ThrowBillingNotFoundException_IfBillingNotFound()
    {
        // Arrange
        _ = _mockBillingRepository
            .Setup(repo => repo.GetBillingByIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new BillingInformationNotFoundException(1));
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<GlobalBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        // Act + Assert
        var ex = Assert.ThrowsAsync<BillingInformationNotFoundException>(async () =>
            await _handler.Handle(new GetBillingByIdQuery(Id: 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("1"));
    }

    [Test]
    public async Task GetBilling_AuthorizationFailsThrowsTest()
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

        var request = new GetBillingByIdQuery(Id: 1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
