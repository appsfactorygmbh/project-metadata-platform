using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Billing;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Billing;

namespace ProjectMetadataPlatform.Application.Tests.Billing;

[TestFixture]
public class GetAllBillingQueryHandlerTest
{
    private GetAllBillingQueryHandler _handler;
    private Mock<IBillingRepository> _mockBillingRepository;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IPaginationHelper> _paginationHelperMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _paginationHelperMock = new Mock<IPaginationHelper>();
        _mockBillingRepository = new Mock<IBillingRepository>();
        _handler = new GetAllBillingQueryHandler(
            billingRepository: _mockBillingRepository.Object,
            authorizationService: _authorizationServiceMock.Object,
            _paginationHelperMock.Object
        );
    }

    [Test]
    public async Task GetAllBilling_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnBilling = new GlobalBilling() { Id = 1, BillingKind = "Test_1" };

        _ = _mockBillingRepository
            .Setup(repo => repo.GetAllGlobalBillingInformationAsync(null))
            .ReturnsAsync(new List<GlobalBilling> { returnBilling }.BuildMock());
        _ = _paginationHelperMock
            .Setup(a =>
                a.PaginateWithAuthAsync(It.IsAny<IQueryable<GlobalBilling>>(), It.IsAny<int?>())
            )
            .ReturnsAsync((IQueryable<GlobalBilling> query, int? limit) => (query, null));
        // Act
        var result = await _handler.Handle(
            new GetAllBillingQuery(null, null),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Item1.Count, Is.EqualTo(1));
        Assert.That(result.Item1.First(), Is.EqualTo(returnBilling));
        _mockBillingRepository.Verify(m => m.GetAllGlobalBillingInformationAsync(null), Times.Once);
    }

    [Test]
    public async Task GetAllBilling_ReturnsInOrder()
    {
        // Arrange
        List<GlobalBilling> returnBilling =
        [
            new() { Id = 1, BillingKind = "Test_1" },
            new() { Id = 3, BillingKind = "test_3" },
            new() { Id = 2, BillingKind = "TesT_2" },
            new() { Id = 4, BillingKind = "Foo_2" },
        ];

        _ = _mockBillingRepository
            .Setup(repo => repo.GetAllGlobalBillingInformationAsync(null))
            .ReturnsAsync(returnBilling.BuildMock());
        _ = _paginationHelperMock
            .Setup(a =>
                a.PaginateWithAuthAsync(It.IsAny<IQueryable<GlobalBilling>>(), It.IsAny<int?>())
            )
            .ReturnsAsync((IQueryable<GlobalBilling> query, int? limit) => (query, null));
        // Act
        var result = await _handler.Handle(
            new GetAllBillingQuery(null, null),
            It.IsAny<CancellationToken>()
        );

        // Assert
        var resultList = result.Item1.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result.Item1.Count, Is.EqualTo(4));
            Assert.That(resultList[0], Is.EqualTo(returnBilling[0]));
            Assert.That(resultList[1], Is.EqualTo(returnBilling[1]));
            Assert.That(resultList[2], Is.EqualTo(returnBilling[2]));
            Assert.That(resultList[3], Is.EqualTo(returnBilling[3]));
        });
    }
}
