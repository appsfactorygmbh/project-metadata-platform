using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.OfficeLocations;
using ProjectMetadataPlatform.Domain.Errors.OfficeLocationExceptions;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.Tests.OfficeLocations;

[TestFixture]
public class GetOfficeLocationQueryHandlerTest
{
    private GetOfficeLocationQueryHandler _handler;
    private Mock<IOfficeLocationRepository> _mockOfficeLocationRepository;

    [SetUp]
    public void Setup()
    {
        _mockOfficeLocationRepository = new Mock<IOfficeLocationRepository>();
        _handler = new GetOfficeLocationQueryHandler(
            officeLocationRepository: _mockOfficeLocationRepository.Object
        );
    }

    [Test]
    public async Task GetOfficeLocation_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnOfficeLocation = new OfficeLocation() { Id = 1, OfficeLocationName = "Test_1" };

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.GetOfficeLocationAsync(It.IsAny<int>()))
            .ReturnsAsync(returnOfficeLocation);

        // Act
        var result = await _handler.Handle(
            new GetOfficeLocationQuery(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(returnOfficeLocation));
        _mockOfficeLocationRepository.Verify(
            m => m.GetOfficeLocationAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
    }

    [Test]
    public void GetOfficeLocation_ThrowOfficeLocationNotFoundException_IfOfficeLocationNotFound()
    {
        // Arrange
        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.GetOfficeLocationAsync(It.IsAny<int>()))
            .ThrowsAsync(new OfficeLocationNotFoundException(1));

        // Act + Assert
        var ex = Assert.ThrowsAsync<OfficeLocationNotFoundException>(async () =>
            await _handler.Handle(new GetOfficeLocationQuery(Id: 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("1"));
    }
}
