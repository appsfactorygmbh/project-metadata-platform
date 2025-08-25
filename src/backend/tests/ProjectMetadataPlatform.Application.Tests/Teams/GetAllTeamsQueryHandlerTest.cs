using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Application.Teams;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Tests.Teams;

[TestFixture]
public class GetAllTeamsQueryHandlerTest
{
    private GetAllTeamsQueryHandler _handler;
    private Mock<ITeamRepository> _mockTeamRepository;

    [SetUp]
    public void Setup()
    {
        _mockTeamRepository = new Mock<ITeamRepository>();
        _handler = new GetAllTeamsQueryHandler(teamRepository: _mockTeamRepository.Object);
    }

    [Test]
    public async Task GetAllTeams_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnTeam = new Team()
        {
            Id = 1,
            TeamName = "Test_1",
            BusinessUnit = "BU Test",
            PTL = "Max Mustermann",
        };

        _mockTeamRepository
            .Setup(repo => repo.GetTeamsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync([returnTeam]);

        // Act
        var result = await _handler.Handle(
            new GetAllTeamsQuery(FullTextQuery: "Test Query", TeamName: "Test Name"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First(), Is.EqualTo(returnTeam));
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
                BusinessUnit = "BU Test",
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 3,
                TeamName = "test_3",
                BusinessUnit = "BU Test",
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 2,
                TeamName = "TesT_2",
                BusinessUnit = "BU Test",
                PTL = "Max Mustermann",
            },
            new()
            {
                Id = 4,
                TeamName = "Foo_2",
                BusinessUnit = "BU Test",
                PTL = "Max Mustermann",
            },
        ];

        _mockTeamRepository
            .Setup(repo => repo.GetTeamsAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(returnTeam);

        // Act
        var result = await _handler.Handle(
            new GetAllTeamsQuery(FullTextQuery: null, TeamName: null),
            It.IsAny<CancellationToken>()
        );

        // Assert
        var resultList = result.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result.Count, Is.EqualTo(4));
            Assert.That(resultList[0], Is.EqualTo(returnTeam[3]));
            Assert.That(resultList[1], Is.EqualTo(returnTeam[0]));
            Assert.That(resultList[2], Is.EqualTo(returnTeam[2]));
            Assert.That(resultList[3], Is.EqualTo(returnTeam[1]));
        });
    }
}
