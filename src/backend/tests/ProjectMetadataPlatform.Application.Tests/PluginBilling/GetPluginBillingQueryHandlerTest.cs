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

namespace ProjectMetadataPlatform.Application.Tests.PluginBilling;

[TestFixture]
public class GetPluginBillingQueryHandlerTest
{
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private GetPluginBillingQueryHandler _handler;
    private Mock<IBillingRepository> _mockPluginBillingRepository;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockPluginBillingRepository = new Mock<IBillingRepository>();
        _handler = new GetPluginBillingQueryHandler(
            billingRepository: _mockPluginBillingRepository.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetPluginBilling_CallsRepositoryCorrectly()
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

        _ = _mockPluginBillingRepository
            .Setup(repo => repo.GetPluginBillingByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(returnPluginBilling);
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Domain.Billing.PluginBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        // Act
        var result = await _handler.Handle(
            new GetPluginBillingQuery(1, 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Item1, Is.Not.Null);
        Assert.That(result.Item1, Is.EqualTo(returnPluginBilling));
        _mockPluginBillingRepository.Verify(
            m => m.GetPluginBillingByIdAsync(It.Is<int>(id => id == 1), It.Is<int>(id => id == 1)),
            Times.Once
        );
    }

    [Test]
    public void GetPluginBilling_ThrowPluginBillingNotFoundException_IfPluginBillingNotFound()
    {
        // Arrange
        _ = _mockPluginBillingRepository
            .Setup(repo => repo.GetPluginBillingByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new PluginBillingInformationNotFoundException(1, 1));
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Domain.Billing.PluginBilling>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        // Act + Assert
        var ex = Assert.ThrowsAsync<PluginBillingInformationNotFoundException>(async () =>
            await _handler.Handle(new GetPluginBillingQuery(1, 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("1"));
    }

    [Test]
    public async Task GetPluginBilling_AuthorizationFailsThrowsTest()
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

        var request = new GetPluginBillingQuery(1, 1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
