using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Teams;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.TeamExceptions;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Tests.Teams;

[TestFixture]
public class GetTeamQueryHandlerTest
{
    private GetTeamQueryHandler _handler;
    private Mock<ITeamRepository> _mockTeamRepository;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockTeamRepository = new Mock<ITeamRepository>();
        _handler = new GetTeamQueryHandler(
            teamRepository: _mockTeamRepository.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetTeam_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnTeam = new Team()
        {
            Id = 1,
            TeamName = "Test_1",
            BusinessUnit = new() { BusinessUnitName = "BU Test" },
            BusinessUnitId = 1,
            PTL = "Max Mustermann",
        };

        _ = _mockTeamRepository
            .Setup(repo => repo.GetTeamAsync(It.IsAny<int>()))
            .ReturnsAsync(returnTeam);
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Team>(),
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
        var result = await _handler.Handle(new GetTeamQuery(Id: 1), It.IsAny<CancellationToken>());

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(returnTeam));
        _mockTeamRepository.Verify(m => m.GetTeamAsync(It.Is<int>(id => id == 1)), Times.Once);
    }

    [Test]
    public void GetTeam_ThrowTeamNotFoundException_IfTeamNotFound()
    {
        // Arrange
        _ = _mockTeamRepository
            .Setup(repo => repo.GetTeamAsync(It.IsAny<int>()))
            .ThrowsAsync(new TeamNotFoundException(1));
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Team>(),
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
        // Act + Assert
        var ex = Assert.ThrowsAsync<TeamNotFoundException>(async () =>
            await _handler.Handle(new GetTeamQuery(Id: 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("1"));
    }

    [Test]
    public async Task CreateTeam_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Team>(),
                    It.IsAny<IEnumerable<AuthorizationConstants.Actions>>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(
                new Dictionary<AuthorizationConstants.Actions, bool>
                {
                    { AuthorizationConstants.Actions.GET, false },
                }
            );

        var request = new GetTeamQuery(Id: 1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
