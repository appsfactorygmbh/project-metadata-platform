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
    private Mock<IPaginationHelper> _paginationHelperMock;
    private Mock<IBusinessUnitRepository> _mockBusinessUnitRepository;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _paginationHelperMock = new Mock<IPaginationHelper>();
        _mockBusinessUnitRepository = new Mock<IBusinessUnitRepository>();
        _handler = new GetAllBusinessUnitsQueryHandler(
            businessUnitRepository: _mockBusinessUnitRepository.Object,
            authorizationService: _authorizationServiceMock.Object,
            _paginationHelperMock.Object
        );
    }

    [Test]
    public async Task GetAllBusinessUnits_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnBusinessUnit = new BusinessUnit() { Id = 1, BusinessUnitName = "Test_1" };

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitsAsync(null))
            .ReturnsAsync(new List<BusinessUnit> { returnBusinessUnit }.BuildMock());
        _ = _paginationHelperMock
            .Setup(a =>
                a.PaginateWithAuthAsync(It.IsAny<IQueryable<BusinessUnit>>(), It.IsAny<int?>())
            )
            .ReturnsAsync((IQueryable<BusinessUnit> query, int? limiz) => (query, null));
        // Act
        var result = await _handler.Handle(
            new GetAllBusinessUnitsQuery(null, null),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Item1.Count, Is.EqualTo(1));
        Assert.That(result.Item1.First(), Is.EqualTo(returnBusinessUnit));
        _mockBusinessUnitRepository.Verify(m => m.GetBusinessUnitsAsync(null), Times.Once);
    }
}
