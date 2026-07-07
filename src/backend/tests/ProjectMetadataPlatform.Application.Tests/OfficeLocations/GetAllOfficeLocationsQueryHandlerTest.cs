using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.OfficeLocations;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.OfficeLocations;

namespace ProjectMetadataPlatform.Application.Tests.OfficeLocations;

[TestFixture]
public class GetAllOfficeLocationsQueryHandlerTest
{
    private GetAllOfficeLocationsQueryHandler _handler;
    private Mock<IOfficeLocationRepository> _mockOfficeLocationRepository;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockOfficeLocationRepository = new Mock<IOfficeLocationRepository>();
        _handler = new GetAllOfficeLocationsQueryHandler(
            officeLocationRepository: _mockOfficeLocationRepository.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetAllOfficeLocations_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnOfficeLocation = new OfficeLocation() { Id = 1, OfficeLocationName = "Test_1" };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<OfficeLocation>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync(
                (IQueryable<OfficeLocation> query, Dictionary<string, string>? dict) => query
            );
        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.GetOfficeLocationsAsync())
            .ReturnsAsync(new List<OfficeLocation> { returnOfficeLocation }.BuildMock());

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

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.GetOfficeLocationsAsync())
            .ReturnsAsync(returnOfficeLocation.BuildMock());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<OfficeLocation>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync(
                (IQueryable<OfficeLocation> query, Dictionary<string, string>? dict) => query
            );
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

    [Test]
    public async Task GetAllOfficeLocations_ReturnsFilteredInOrder()
    {
        // Arrange
        List<OfficeLocation> returnOfficeLocation =
        [
            new() { Id = 1, OfficeLocationName = "Test_1" },
            new() { Id = 3, OfficeLocationName = "test_3" },
            new() { Id = 2, OfficeLocationName = "TesT_2" },
            new() { Id = 4, OfficeLocationName = "Foo_2" },
        ];

        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.GetOfficeLocationsAsync())
            .ReturnsAsync(returnOfficeLocation.BuildMock());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<OfficeLocation>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<OfficeLocation>?)null);

        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<OfficeLocation>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.GET, true },
                }
            );
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
