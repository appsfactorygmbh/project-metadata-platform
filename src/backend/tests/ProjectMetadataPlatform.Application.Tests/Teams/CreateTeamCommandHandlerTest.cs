using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Teams;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Errors.BusinessUnitExceptions;
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

    private Mock<IBusinessUnitRepository> _mockBusinessUnitRepository;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockTeamRepository = new Mock<ITeamRepository>();
        _mockBusinessUnitRepository = new Mock<IBusinessUnitRepository>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateTeamCommandHandler(
            teamRepository: _mockTeamRepository.Object,
            businessUnitRepository: _mockBusinessUnitRepository.Object,
            logRepository: _mockLogRepo.Object,
            unitOfWork: _mockUnitOfWork.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task CreateTeam_NameDoesNotAlreadyExists_WorksFine()
    {
        // Arrange
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
                    { AuthorizationConstants.Actions.CREATE, true },
                }
            );
        _ = _mockTeamRepository
            .Setup(repo => repo.CheckIfTeamNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _ = _mockTeamRepository
            .Setup(repo => repo.AddTeamAsync(It.IsAny<Team>()))
            .Callback(
                (Team teamBeingAdded) =>
                {
                    teamBeingAdded.Id = 1;
                }
            )
            .Returns(Task.CompletedTask);
        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.CheckIfBusinessUnitExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(
            new CreateTeamCommand(TeamName: "Test Name", BusinessUnitId: 1, PTL: "Max Mustermann"),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Is.EqualTo(1));
        _mockLogRepo.Verify(
            m =>
                m.AddTeamLogForCurrentActor(
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
                        team.BusinessUnitId == 1
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
                    { AuthorizationConstants.Actions.CREATE, true },
                }
            );
        _ = _mockTeamRepository
            .Setup(repo => repo.CheckIfTeamNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.CheckIfBusinessUnitExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(true);
        // Act + Assert
        var ex = Assert.ThrowsAsync<TeamNameAlreadyExistsException>(async () =>
            await _handler.Handle(
                new CreateTeamCommand(
                    TeamName: "Test Name",
                    BusinessUnitId: 1,
                    PTL: "Max Mustermann"
                ),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("Test Name"));
    }

    [Test]
    public void CreateTeam_BUDoesntExists_ThrowsException()
    {
        // Arrange
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
                    { AuthorizationConstants.Actions.CREATE, true },
                }
            );
        _ = _mockTeamRepository
            .Setup(repo => repo.CheckIfTeamNameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        _ = _mockBusinessUnitRepository
            .Setup(repo => repo.CheckIfBusinessUnitExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(false);
        // Act + Assert
        var ex = Assert.ThrowsAsync<BusinessUnitNotFoundException>(async () =>
            await _handler.Handle(
                new CreateTeamCommand(
                    TeamName: "Test Name",
                    BusinessUnitId: 1,
                    PTL: "Max Mustermann"
                ),
                It.IsAny<CancellationToken>()
            )
        );

        Assert.That(ex.Message, Does.Contain("The Business Unit with id 1 was not found."));
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
                    { AuthorizationConstants.Actions.CREATE, false },
                }
            );

        var request = new CreateTeamCommand(
            TeamName: "Test Name",
            BusinessUnitId: 1,
            PTL: "Max Mustermann"
        );

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
