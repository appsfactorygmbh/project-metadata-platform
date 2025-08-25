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
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Teams;

[TestFixture]
public class CreateTeamCommandHandlerTest
{
    private CreateTeamCommandHandler _handler;
    private Mock<ITeamRepository> _mockTeamRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;

    [SetUp]
    public void Setup()
    {
        _mockTeamRepository = new Mock<ITeamRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateTeamCommandHandler(
            teamRepository: _mockTeamRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object
        );
    }

    [Test]
    public async Task CreateTeam_NameDoesNotAlreadyExists_WorksFine()
    {
        // Arrange
        _mockTeamRepository
            .Setup(repo => repo.CheckIfTeamNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _mockTeamRepository
            .Setup(repo => repo.AddTeamAsync(It.IsAny<Team>()))
            .Callback(
                (Team teamBeingAdded) =>
                {
                    teamBeingAdded.Id = 1;
                }
            )
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(
            new CreateTeamCommand(
                TeamName: "Test Name",
                BusinessUnit: "Test BU",
                PTL: "Max Mustermann"
            ),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.EqualTo(1));
        _mockLogRepo.Verify(
            m =>
                m.AddTeamLogForCurrentUser(
                    It.IsAny<Team>(),
                    Action.ADDED_TEAM,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
        _mockTeamRepository.Verify(
            m =>
                m.AddTeamAsync(
                    It.Is<Team>(team =>
                        team.BusinessUnit == "Test BU"
                        && team.TeamName == "Test Name"
                        && team.PTL == "Max Mustermann"
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public void CreateTeam_NameAlreadyExists_ThrowsTeamNameAlreadyExistsException()
    {
        // Arrange
        _mockTeamRepository
            .Setup(repo => repo.CheckIfTeamNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act + Assert
        var ex = Assert.ThrowsAsync<TeamNameAlreadyExistsException>(async () =>
            await _handler.Handle(
                new CreateTeamCommand(
                    TeamName: "Test Name",
                    BusinessUnit: "Test BU",
                    PTL: "Max Mustermann"
                ),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test Name"));
    }
}
