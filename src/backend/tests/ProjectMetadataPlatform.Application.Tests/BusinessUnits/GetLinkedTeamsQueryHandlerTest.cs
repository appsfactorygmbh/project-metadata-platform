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

namespace ProjectMetadataPlatform.Application.Tests.BusinessUnits;

[TestFixture]
public class GetLinkedTeamsQueryHandlerTest
{
    private GetLinkedTeamsQueryHandler _handler;
    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IBusinessUnitRepository> _mockBusinessUnitRepository;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockBusinessUnitRepository = new Mock<IBusinessUnitRepository>();
        _handler = new GetLinkedTeamsQueryHandler(
            businessUnitRepository: _mockBusinessUnitRepository.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetLinkedTeams_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnBusinessUnit = new BusinessUnit()
        {
            Id = 1,
            BusinessUnitName = "Test_1",
            Teams =
            [
                new()
                {
                    Id = 111,
                    TeamName = "teams",

                    BusinessUnitId = 1,
                },
                new()
                {
                    Id = 222,
                    TeamName = "teams",

                    BusinessUnitId = 1,
                },
            ],
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<BusinessUnit>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.GetBusinessUnitWithTeamsAsync(It.IsAny<int>()))
            .ReturnsAsync(returnBusinessUnit);

        // Act
        var result = await _handler.Handle(
            new GetLinkedTeamsQuery(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(result, Does.Contain(111));
            Assert.That(result, Does.Contain(222));
        });
        _mockBusinessUnitRepository.Verify(
            m => m.GetBusinessUnitWithTeamsAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
    }

    [Test]
    public async Task CreateBusinessUnitCommand_AuthorizationFailsThrowsTest()
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

        var request = new GetLinkedTeamsQuery(Id: 1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
