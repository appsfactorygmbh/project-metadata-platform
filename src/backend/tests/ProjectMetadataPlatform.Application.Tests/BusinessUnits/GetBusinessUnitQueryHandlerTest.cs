using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.BusinessUnits;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;

namespace ProjectMetadataPlatform.Application.Tests.BusinessUnits;

[TestFixture]
public class GetBusinessUnitQueryHandlerTest
{
    private GetBusinessUnitQueryHandler _handler;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IBusinessUnitRepository> _mockBusinessUnitRepository;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockBusinessUnitRepository = new Mock<IBusinessUnitRepository>();
        _handler = new GetBusinessUnitQueryHandler(
            businessUnitRepository: _mockBusinessUnitRepository.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetBusinessUnit_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnBusinessUnit = new BusinessUnit() { Id = 1, BusinessUnitName = "Test_1" };

        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitAsync(It.IsAny<int>()))
            .ReturnsAsync(returnBusinessUnit);
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
            new GetBusinessUnitQuery(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Item1, Is.Not.Null);
        Assert.That(result.Item1, Is.EqualTo(returnBusinessUnit));
        _mockBusinessUnitRepository.Verify(
            m => m.GetBusinessUnitAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );

        _authorizationServiceMock.Verify(
            a => a.CheckAccess(It.IsAny<BusinessUnit>(), AuthorizationConstants.Actions.GET, null),
            Times.Once
        );
    }

    [Test]
    public void GetBusinessUnit_ThrowBusinessUnitNotFoundException_IfBusinessUnitNotFound()
    {
        // Arrange
        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitAsync(It.IsAny<int>()))
            .ThrowsAsync(new BusinessUnitNotFoundException(1));
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<BusinessUnit>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        // Act + Assert
        var ex = Assert.ThrowsAsync<BusinessUnitNotFoundException>(async () =>
            await _handler.Handle(new GetBusinessUnitQuery(Id: 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("1"));
    }

    [Test]
    public async Task GetBusinessUnit_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<BusinessUnit>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(false);

        var request = new GetBusinessUnitQuery(Id: 1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
