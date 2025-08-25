using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Teams;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Tests.Teams;

[TestFixture]
public class GetLinkedProjectsQueryHandlerTest
{
    private GetLinkedProjectsQueryHandler _handler;
    private Mock<ITeamRepository> _mockTeamRepository;

    [SetUp]
    public void Setup()
    {
        _mockTeamRepository = new Mock<ITeamRepository>();
        _handler = new GetLinkedProjectsQueryHandler(teamRepository: _mockTeamRepository.Object);
    }

    [Test]
    public async Task GetLinkedProjects_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnTeam = new Team()
        {
            Id = 1,
            TeamName = "Test_1",
            BusinessUnit = "BU Test",
            PTL = "Max Mustermann",
            Projects =
            [
                new()
                {
                    Id = 111,
                    ProjectName = "Projects",
                    Slug = "project_1",
                    ClientName = "Project Client",
                },
                new()
                {
                    Id = 222,
                    ProjectName = "Projects",
                    Slug = "project_2",
                    ClientName = "Project Client",
                },
            ],
        };

        _mockTeamRepository
            .Setup(repo => repo.GetTeamWithProjectsAsync(It.IsAny<int>()))
            .ReturnsAsync(returnTeam);

        // Act
        var result = await _handler.Handle(
            new GetLinkedProjectsQuery(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        var resultList = result.ToList();
        Assert.Multiple(() =>
        {
            Assert.That(result, Does.Contain("project_1"));
            Assert.That(result, Does.Contain("project_2"));
        });
        _mockTeamRepository.Verify(
            m => m.GetTeamWithProjectsAsync(It.Is<int>(id => id == 1)),
            Times.Once
        );
    }
}
