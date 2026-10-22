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
    private Mock<IPaginationHelper> _paginationHelperMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _paginationHelperMock = new Mock<IPaginationHelper>();
        _mockOfficeLocationRepository = new Mock<IOfficeLocationRepository>();
        _handler = new GetAllOfficeLocationsQueryHandler(
            officeLocationRepository: _mockOfficeLocationRepository.Object,
            authorizationService: _authorizationServiceMock.Object,
            _paginationHelperMock.Object
        );
    }

    [Test]
    public async Task GetAllOfficeLocations_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnOfficeLocation = new OfficeLocation() { Id = 1, OfficeLocationName = "Test_1" };
        _ = _paginationHelperMock
            .Setup(a =>
                a.PaginateWithAuthAsync(It.IsAny<IQueryable<OfficeLocation>>(), It.IsAny<int?>())
            )
            .ReturnsAsync((IQueryable<OfficeLocation> query, int? limit) => (query, null));
        _ = _mockOfficeLocationRepository
            .Setup(repo => repo.GetOfficeLocationsAsync(null))
            .ReturnsAsync(new List<OfficeLocation> { returnOfficeLocation }.BuildMock());

        // Act
        var result = await _handler.Handle(
            new GetAllOfficeLocationsQuery(null, null),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Item1.Count, Is.EqualTo(1));
        Assert.That(result.Item1.First(), Is.EqualTo(returnOfficeLocation));
        _mockOfficeLocationRepository.Verify(m => m.GetOfficeLocationsAsync(null), Times.Once);
    }
}
