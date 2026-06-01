using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.BusinessUnits;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.BusinessUnits;

namespace ProjectMetadataPlatform.Application.Tests.BusinessUnits;

[TestFixture]
public class GetAllBusinessUnitsQueryHandlerTest
{
    private GetAllBusinessUnitsQueryHandler _handler;
    private Mock<IBusinessUnitRepository> _mockBusinessUnitRepository;

    [SetUp]
    public void Setup()
    {
        _mockBusinessUnitRepository = new Mock<IBusinessUnitRepository>();
        _handler = new GetAllBusinessUnitsQueryHandler(
            businessUnitRepository: _mockBusinessUnitRepository.Object
        );
    }

    [Test]
    public async Task GetAllBusinessUnits_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnBusinessUnit = new BusinessUnit() { Id = 1, BusinessUnitName = "Test_1" };

        _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitsAsync())
            .ReturnsAsync([returnBusinessUnit]);

        // Act
        var result = await _handler.Handle(
            new GetAllBusinessUnitsQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First(), Is.EqualTo(returnBusinessUnit));
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

        _mockBusinessUnitRepository.Setup(repo => repo.GetBusinessUnitsAsync()).ReturnsAsync(returnBusinessUnit);

        // Act
        var result = await _handler.Handle(
            new GetAllBusinessUnitsQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        var resultList = result.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(resultList[0], Is.EqualTo(returnBusinessUnit[3]));
            Assert.That(resultList[1], Is.EqualTo(returnBusinessUnit[0]));
            Assert.That(resultList[2], Is.EqualTo(returnBusinessUnit[2]));
            Assert.That(resultList[3], Is.EqualTo(returnBusinessUnit[1]));
        });
    }
}
