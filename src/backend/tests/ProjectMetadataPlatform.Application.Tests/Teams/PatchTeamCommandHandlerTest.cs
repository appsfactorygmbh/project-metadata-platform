using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Teams;
using ProjectMetadataPlatform.Domain.Errors.TeamExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Tests.Teams;

[TestFixture]
public class PatchTeamCommandHandlerTest
{
    private PatchTeamCommandHandler _handler;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    private Mock<ITeamRepository> _mockTeamRepository;

    [SetUp]
    public void Setup()
    {
        _mockTeamRepository = new Mock<ITeamRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new PatchTeamCommandHandler(
            teamRepository: _mockTeamRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task PatchTeam_CallsRepositoryCorrectly()
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

        _mockTeamRepository
            .Setup(repo => repo.UpdateTeamAsync(It.IsAny<Team>()))
            .ReturnsAsync((Team team) => team);

        _mockTeamRepository
            .Setup(repo => repo.CheckIfTeamNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            new PatchTeamCommand(Id: 1, TeamName: "Test_2", PTL: "Test"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(returnTeam));
        _mockTeamRepository.Verify(m => m.GetTeamAsync(It.Is<int>(id => id == 1)), Times.Once);
        _mockTeamRepository.Verify(
            m =>
                m.UpdateTeamAsync(
                    It.Is<Team>(team =>
                        team.Id == 1
                        && team.BusinessUnit == "BU Test"
                        && team.PTL == "Test"
                        && team.TeamName == "Test_2"
                    )
                ),
            Times.Once
        );
        _mockLogRepo.Verify(
            m =>
                m.AddTeamLogForCurrentUser(
                    It.IsAny<Team>(),
                    Action.UPDATED_TEAM,
                    It.Is<List<LogChange>>(changes =>
                        changes[0].Property == "TeamName"
                        && changes[0].NewValue == "Test_2"
                        && changes[0].OldValue == "Test_1"
                        && changes[1].Property == "PTL"
                        && changes[1].NewValue == "Test"
                        && changes[1].OldValue == "Max Mustermann"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task PatchTeam_NoLogCreatedIfValuesAreEqual()
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

        _mockTeamRepository
            .Setup(repo => repo.UpdateTeamAsync(It.IsAny<Team>()))
            .ReturnsAsync((Team team) => team);

        _mockTeamRepository
            .Setup(repo => repo.CheckIfTeamNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            new PatchTeamCommand(Id: 1, TeamName: "Test_1", PTL: "Max Mustermann"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        _mockLogRepo.Verify(
            m =>
                m.AddTeamLogForCurrentUser(
                    It.IsAny<Team>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
    }

    [Test]
    public void PatchTeam_ThrowsTeamNameAlreadyExistsException_IfNewTeamNameAlreadyExists()
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

        _mockTeamRepository
            .Setup(repo => repo.UpdateTeamAsync(It.IsAny<Team>()))
            .ReturnsAsync((Team team) => team);

        _mockTeamRepository
            .Setup(repo => repo.CheckIfTeamNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act + Assert
        var ex = Assert.ThrowsAsync<TeamNameAlreadyExistsException>(async () =>
            await _handler.Handle(
                new PatchTeamCommand(Id: 1, TeamName: "Test_2", PTL: "Max Mustermann"),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test_2"));
    }
}
