using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.OfficeLocations;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.Tests.OfficeLocations;

[TestFixture]
public class GetAllOfficeLocationsQueryHandlerTest
{
    private GetAllOfficeLocationsQueryHandler _handler;
    private Mock<IOfficeLocationRepository> _mockOfficeLocationRepository;

    [SetUp]
    public void Setup()
    {
        _mockOfficeLocationRepository = new Mock<IOfficeLocationRepository>();
        _handler = new GetAllOfficeLocationsQueryHandler(
            officeLocationRepository: _mockOfficeLocationRepository.Object
        );
    }

    [Test]
    public async Task GetAllOfficeLocations_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnOfficeLocation = new OfficeLocation() { Id = 1, OfficeLocationName = "Test_1" };

        _mockOfficeLocationRepository
            .Setup(repo => repo.GetOfficeLocationsAsync())
            .ReturnsAsync([returnOfficeLocation]);

        // Act
        var result = await _handler.Handle(
            new GetAllOfficeLocationsQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First(), Is.EqualTo(returnOfficeLocation));
        _mockOfficeLocationRepository.Verify(m => m.GetOfficeLocationsAsync(), Times.Once);
    }

    [Test]
    public async Task GetAllOfficeLocations_ReturnsInOrder()
    {
        // Arrange
        List<OfficeLocation> returnOfficeLocation =
        [
            new() { Id = 1, OfficeLocationName = "Test_1" },
            new() { Id = 3, OfficeLocationName = "test_3" },
            new() { Id = 2, OfficeLocationName = "TesT_2" },
            new() { Id = 4, OfficeLocationName = "Foo_2" },
        ];

        _mockOfficeLocationRepository
            .Setup(repo => repo.GetOfficeLocationsAsync())
            .ReturnsAsync(returnOfficeLocation);

        // Act
        var result = await _handler.Handle(
            new GetAllOfficeLocationsQuery(),
            It.IsAny<CancellationToken>()
        );

        // Assert
        var resultList = result.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(resultList[0], Is.EqualTo(returnOfficeLocation[3]));
            Assert.That(resultList[1], Is.EqualTo(returnOfficeLocation[0]));
            Assert.That(resultList[2], Is.EqualTo(returnOfficeLocation[2]));
            Assert.That(resultList[3], Is.EqualTo(returnOfficeLocation[1]));
        });
    }
}
