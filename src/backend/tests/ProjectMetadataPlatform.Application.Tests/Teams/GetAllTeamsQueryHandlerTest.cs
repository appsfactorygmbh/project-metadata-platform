using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Teams;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Tests.Teams;

[TestFixture]
public class GetAllTeamsQueryHandlerTest
{
    private GetAllTeamsQueryHandler _handler;
    private Mock<ITeamRepository> _mockTeamRepository;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockTeamRepository = new Mock<ITeamRepository>();
        _handler = new GetAllTeamsQueryHandler(
            teamRepository: _mockTeamRepository.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetAllTeams_CallsRepositoryCorrectly()
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
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<Team>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<Team> query, Dictionary<string, string>? dict) => query);
        _ = _mockTeamRepository
            .Setup(repo => repo.GetTeamsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new List<Team> { returnTeam }.BuildMock());

        // Act
        var result = await _handler.Handle(
            new GetAllTeamsQuery(FullTextQuery: "Test Query", TeamName: "Test Name"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Item1.Count, Is.EqualTo(1));
        Assert.That(result.Item1.First(), Is.EqualTo(returnTeam));
        _mockTeamRepository.Verify(
            m =>
                m.GetTeamsAsync(
                    It.Is<string>(query => query == "Test Query"),
                    It.Is<string>(teamName => teamName == "Test Name")
                ),
            Times.Once
        );
    }

    [Test]
    public async Task GetAllTeams_ReturnsInOrder()
    {
        // Arrange
        List<Team> returnTeam =
        [
            new()
            {
                Id = 1,
                TeamName = "Test_1",
                BusinessUnit = new() { BusinessUnitName = "BU Test" },
                BusinessUnitId = 1,
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 3,
                TeamName = "test_3",
                BusinessUnit = new() { BusinessUnitName = "BU Test" },
                BusinessUnitId = 1,
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 2,
                TeamName = "TesT_2",
                BusinessUnit = new() { BusinessUnitName = "BU Test" },
                BusinessUnitId = 1,
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 4,
                TeamName = "Foo_2",
                BusinessUnit = new() { BusinessUnitName = "BU Test" },
                BusinessUnitId = 1,
                PTL = "Max Mustermann",
            },
        ];

        _ = _mockTeamRepository
            .Setup(repo => repo.GetTeamsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(returnTeam.BuildMock());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<Team>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<Team> query, Dictionary<string, string>? dict) => query);
        // Act
        var result = await _handler.Handle(
            new GetAllTeamsQuery(FullTextQuery: null, TeamName: null),
            It.IsAny<CancellationToken>()
        );

        // Assert
        var resultList = result.Item1.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result.Item1.Count, Is.EqualTo(4));
            Assert.That(resultList[0], Is.EqualTo(returnTeam[3]));
            Assert.That(resultList[1], Is.EqualTo(returnTeam[0]));
            Assert.That(resultList[2], Is.EqualTo(returnTeam[2]));
            Assert.That(resultList[3], Is.EqualTo(returnTeam[1]));
        });
    }

    [Test]
    public async Task GetAllTeams_ReturnsFiltered()
    {
        // Arrange
        List<Team> returnTeam =
        [
            new()
            {
                Id = 1,
                TeamName = "Test_1",
                BusinessUnit = new() { BusinessUnitName = "BU Test" },
                BusinessUnitId = 1,
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 3,
                TeamName = "test_3",
                BusinessUnit = new() { BusinessUnitName = "BU Test" },
                BusinessUnitId = 1,
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 2,
                TeamName = "TesT_2",
                BusinessUnit = new() { BusinessUnitName = "BU Test" },
                BusinessUnitId = 1,
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 4,
                TeamName = "Foo_2",
                BusinessUnit = new() { BusinessUnitName = "BU Test" },
                BusinessUnitId = 1,
                PTL = "Max Mustermann",
            },
        ];

        _ = _mockTeamRepository
            .Setup(repo => repo.GetTeamsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(returnTeam.BuildMock());
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<Team>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<Team>?)null);

        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Team>(),
                    It.IsAny<AuthorizationConstants.Actions>(),
                    It.IsAny<Dictionary<string, object?>?>()
                )
            )
            .ReturnsAsync(true);
        // Act
        var result = await _handler.Handle(
            new GetAllTeamsQuery(FullTextQuery: null, TeamName: null),
            It.IsAny<CancellationToken>()
        );

        Assert.That(result.Item1, Is.EquivalentTo(returnTeam));
    }
}
