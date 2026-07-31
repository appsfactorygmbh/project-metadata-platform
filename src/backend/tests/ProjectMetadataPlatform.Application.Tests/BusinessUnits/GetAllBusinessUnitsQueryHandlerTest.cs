using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.BusinessUnits;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.Tests.BusinessUnits;

[TestFixture]
public class GetAllBusinessUnitsQueryHandlerTest
{
    private GetAllBusinessUnitsQueryHandler _handler;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IBusinessUnitRepository> _mockBusinessUnitRepository;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockBusinessUnitRepository = new Mock<IBusinessUnitRepository>();
        _handler = new GetAllBusinessUnitsQueryHandler(
            businessUnitRepository: _mockBusinessUnitRepository.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetAllBusinessUnits_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnBusinessUnit = new BusinessUnit() { Id = 1, BusinessUnitName = "Test_1" };

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitsAsync())
            .ReturnsAsync(new List<BusinessUnit> { returnBusinessUnit }.BuildMock());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<BusinessUnit>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync(
                (IQueryable<BusinessUnit> query, Dictionary<string, string>? dict) => query
            );
        // Act
        var result = await _handler.Handle(
            new GetAllBusinessUnitsQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Item1.Count, Is.EqualTo(1));
        Assert.That(result.Item1.First(), Is.EqualTo(returnBusinessUnit));
        _mockBusinessUnitRepository.Verify(m => m.GetBusinessUnitsAsync(), Times.Once);
    }

    [Test]
    public async Task GetAllBusinessUnits_ReturnsInOrder()
    {
        // Arrange
        List<BusinessUnit> returnBusinessUnit =
        [
            new() { Id = 1, BusinessUnitName = "Test_1" },
            new() { Id = 3, BusinessUnitName = "test_3" },
            new() { Id = 2, BusinessUnitName = "TesT_2" },
            new() { Id = 4, BusinessUnitName = "Foo_2" },
        ];

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitsAsync())
            .ReturnsAsync(returnBusinessUnit.BuildMock());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<BusinessUnit>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync(
                (IQueryable<BusinessUnit> query, Dictionary<string, string>? dict) => query
            );
        // Act
        var result = await _handler.Handle(
            new GetAllBusinessUnitsQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        var resultList = result.Item1.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result.Item1.Count, Is.EqualTo(4));
            Assert.That(resultList[0], Is.EqualTo(returnBusinessUnit[3]));
            Assert.That(resultList[1], Is.EqualTo(returnBusinessUnit[0]));
            Assert.That(resultList[2], Is.EqualTo(returnBusinessUnit[2]));
            Assert.That(resultList[3], Is.EqualTo(returnBusinessUnit[1]));
        });
    }

    [Test]
    public async Task GetAllBusinessUnits_ReturnsFilteredBUsInOrder()
    {
        // Arrange
        List<BusinessUnit> returnBusinessUnit =
        [
            new() { Id = 1, BusinessUnitName = "Test_1" },
            new() { Id = 3, BusinessUnitName = "test_3" },
            new() { Id = 2, BusinessUnitName = "TesT_2" },
            new() { Id = 4, BusinessUnitName = "Foo_2" },
        ];

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitsAsync())
            .ReturnsAsync(returnBusinessUnit.BuildMock());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<BusinessUnit>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<BusinessUnit>?)null);
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<BusinessUnit>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        // Act
        var result = await _handler.Handle(
            new GetAllBusinessUnitsQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        var resultList = result.Item1.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result.Item1.Count, Is.EqualTo(4));
            Assert.That(resultList[0], Is.EqualTo(returnBusinessUnit[3]));
            Assert.That(resultList[1], Is.EqualTo(returnBusinessUnit[0]));
            Assert.That(resultList[2], Is.EqualTo(returnBusinessUnit[2]));
            Assert.That(resultList[3], Is.EqualTo(returnBusinessUnit[1]));
        });
    }
}
