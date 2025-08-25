using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Teams;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Tests.Teams;

[TestFixture]
public class GetTeamQueryHandlerTest
{
    private GetTeamQueryHandler _handler;
    private Mock<ITeamRepository> _mockTeamRepository;

    [SetUp]
    public void Setup()
    {
        _mockTeamRepository = new Mock<ITeamRepository>();
        _handler = new GetTeamQueryHandler(teamRepository: _mockTeamRepository.Object);
    }

    [Test]
    public async Task GetTeam_CallsRepositoryCorrectly()
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
            .Setup(repo => repo.GetTeamAsync(It.IsAny<int>()))
            .ReturnsAsync(returnTeam);

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
        _mockTeamRepository
            .Setup(repo => repo.GetTeamAsync(It.IsAny<int>()))
            .ThrowsAsync(new TeamNotFoundException(1));

        // Act + Assert
        var ex = Assert.ThrowsAsync<TeamNotFoundException>(async () =>
            await _handler.Handle(new GetTeamQuery(Id: 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("1"));
    }
}
