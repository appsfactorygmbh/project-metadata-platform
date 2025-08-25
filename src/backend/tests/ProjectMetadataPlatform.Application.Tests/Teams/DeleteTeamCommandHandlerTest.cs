using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Teams;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Errors.TeamExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Teams;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Teams;

[TestFixture]
public class DeleteTeamCommandHandlerTest
{
    private DeleteTeamCommandHandler _handler;
    private Mock<ITeamRepository> _mockTeamRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockTeamRepository = new Mock<ITeamRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new DeleteTeamCommandHandler(
            teamRepository: _mockTeamRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task DeleteTeam_NoLinkedProjects_WorksFine()
    {
        // Arrange
        var returnTeam = new Team()
        {
            Id = 1,
            TeamName = "Test_1",
            BusinessUnit = "BU Test",
            PTL = "Max Mustermann",
            Projects = [],
        };

        _mockTeamRepository
            .Setup(repo => repo.GetTeamWithProjectsAsync(It.IsAny<int>()))
            .ReturnsAsync(returnTeam);

        // Act
        await _handler.Handle(new DeleteTeamCommand(Id: 1), It.IsAny<CancellationToken>());

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddTeamLogForCurrentUser(
                    It.IsAny<Team>(),
                    Action.REMOVED_TEAM,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockTeamRepository.Verify(
            m =>
                m.DeleteTeamAsync(
                    It.Is<Team>(team =>
                        team.Id == 1
                        && team.BusinessUnit == "BU Test"
                        && team.TeamName == "Test_1"
                        && team.PTL == "Max Mustermann"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public void DeleteTeam_StillLinkedProjects_ThrowsTeamStillLinkedToProjectsException()
    {
        // Arrange
        _mockTeamRepository
            .Setup(repo => repo.CheckIfTeamNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

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
                    Slug = "project",
                    ClientName = "Project Client",
                },
            ],
        };

        _mockTeamRepository
            .Setup(repo => repo.GetTeamWithProjectsAsync(It.IsAny<int>()))
            .ReturnsAsync(returnTeam);

        // Act + Assert
        var ex = Assert.ThrowsAsync<TeamStillLinkedToProjectsException>(async () =>
            await _handler.Handle(new DeleteTeamCommand(Id: 1), It.IsAny<CancellationToken>())
        );

        Assert.That(ex.Message, Does.Contain("111"));
    }
}
