using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Api.Errors;
using ProjectMetadataPlatform.Api.Teams;
using ProjectMetadataPlatform.Api.Teams.Models;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Application.Teams;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Errors.TeamExceptions;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Api.Tests.Teams;

[TestFixture]
public class TeamsControllerTest
{
    private TeamsController _controller;
    private Mock<IMediator> _mediatorMock;

    [SetUp]
    public void Setup()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new TeamsController(_mediatorMock.Object);
    }

    [Test]
    public async Task CreateTeam_ValidRequest_ReturnsCreatedResultWithTeamId()
    {
        // Arrange
        var expectedTeamId = 42;
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTeamCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTeamId);

        var request = new CreateTeamRequest(
            TeamName: "Test TeamName",
            BusinessUnit: "Test BU",
            PTL: "Test PTL"
        );

        // Act
        var actionResult = await _controller.Put(request);

        // Assert
        Assert.That(actionResult.Result, Is.InstanceOf<CreatedResult>());
        var createdResult = actionResult.Result as CreatedResult;

        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));
        Assert.That(createdResult.Value, Is.InstanceOf<CreateTeamResponse>());

        var teamResponse = createdResult.Value as CreateTeamResponse;
        Assert.That(teamResponse, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(teamResponse.Id, Is.EqualTo(expectedTeamId));
            Assert.That(createdResult.Location, Is.EqualTo($"/Teams/{expectedTeamId}"));
        });

        _mediatorMock.Verify(
            m =>
                m.Send(
                    It.Is<CreateTeamCommand>(cmd =>
                        cmd.TeamName == request.TeamName
                        && cmd.BusinessUnit == request.BusinessUnit
                        && cmd.PTL == request.PTL
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public void CreateTeam_MediatorThrowsIOException_ThrowsIOException()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTeamCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new IOException("Disk full"));

        var request = new CreateTeamRequest(
            TeamName: "Test TeamName",
            BusinessUnit: "Test BU",
            PTL: "Test PTL"
        );

        // Act & Assert
        Assert.ThrowsAsync<IOException>(() => _controller.Put(request));
    }

    [Test]
    public void CreateTeam_TeamNameAlreadyExists_ThrowsTeamNameAlreadyExistsException()
    {
        // Arrange
        var existingTeamName = "Test TeamName";
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateTeamCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TeamNameAlreadyExistsException(existingTeamName));

        var request = new CreateTeamRequest(
            TeamName: existingTeamName,
            BusinessUnit: "Test BU",
            PTL: "Test PTL"
        );

        // Act & Assert
        Assert.ThrowsAsync<TeamNameAlreadyExistsException>(() => _controller.Put(request));
    }

    [Test]
    public async Task CreateTeam_EmptyTeamName_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateTeamRequest(TeamName: "", BusinessUnit: "Test BU", PTL: "Test PTL");

        // Act
        var actionResult = await _controller.Put(request);

        // Assert
        Assert.That(actionResult.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = actionResult.Result as BadRequestObjectResult;

        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.InstanceOf<ErrorResponse>());
        var errorResponse = badRequestResult.Value as ErrorResponse;
        Assert.That(errorResponse?.Message, Is.EqualTo("TeamName can't be empty or whitespaces"));
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<CreateTeamCommand>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GetTeamById_Exists_ReturnsOkResultWithTeam()
    {
        // Arrange
        var teamId = 1;
        var team = new Team
        {
            Id = teamId,
            TeamName = "Test Team",
            BusinessUnit = "BU1",
            PTL = "PTL1",
        };
        _mediatorMock
            .Setup(m =>
                m.Send(It.Is<GetTeamQuery>(q => q.Id == teamId), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(team);

        // Act
        var actionResult = await _controller.Get(teamId);

        // Assert
        Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.InstanceOf<GetTeamResponse>());
        var response = okResult.Value as GetTeamResponse;
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(team.Id));
            Assert.That(response.TeamName, Is.EqualTo(team.TeamName));
            Assert.That(response.BusinessUnit, Is.EqualTo(team.BusinessUnit));
            Assert.That(response.PTL, Is.EqualTo(team.PTL));
        });
    }

    [Test]
    public void GetTeamById_NotFound_ThrowsTeamNotFoundException()
    {
        // Arrange
        var teamId = 1;
        _mediatorMock
            .Setup(m =>
                m.Send(It.Is<GetTeamQuery>(q => q.Id == teamId), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new TeamNotFoundException(teamId));

        // Act & Assert
        Assert.ThrowsAsync<TeamNotFoundException>(() => _controller.Get(teamId));
    }

    [Test]
    public async Task GetAllTeams_ReturnsOkResultWithListOfTeams()
    {
        // Arrange
        var teams = new List<Team>
        {
            new()
            {
                Id = 1,
                TeamName = "Team Alpha",
                BusinessUnit = "AlphaBU",
                PTL = "AlphaPTL",
            },
            new()
            {
                Id = 2,
                TeamName = "Team Beta",
                BusinessUnit = "BetaBU",
                PTL = "BetaPTL",
            },
        };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllTeamsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teams);

        // Act
        var actionResult = await _controller.Get(null, null);

        // Assert
        Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as IEnumerable<GetTeamResponse>;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Count(), Is.EqualTo(teams.Count));
        var firstResponseTeam = response.First();
        var firstOriginalTeam = teams.First();
        Assert.Multiple(() =>
        {
            Assert.That(firstResponseTeam.Id, Is.EqualTo(firstOriginalTeam.Id));
            Assert.That(firstResponseTeam.TeamName, Is.EqualTo(firstOriginalTeam.TeamName));
        });
    }

    [Test]
    public async Task GetAllTeams_NoTeams_ReturnsOkResultWithEmptyList()
    {
        // Arrange
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllTeamsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>());

        // Act
        var actionResult = await _controller.Get(null, null);

        // Assert
        Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var response = okResult.Value as IEnumerable<GetTeamResponse>;
        Assert.That(response, Is.Not.Null.And.Empty);
    }

    [Test]
    public async Task GetAllTeams_WithFilters_SendsCorrectQueryToMediator()
    {
        // Arrange
        var teamNameFilter = "Alpha";
        var searchQuery = "SearchKeyword";
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllTeamsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Team>());

        // Act
        await _controller.Get(teamName: teamNameFilter, search: searchQuery);

        // Assert
        _mediatorMock.Verify(
            m =>
                m.Send(
                    It.Is<GetAllTeamsQuery>(q =>
                        q.TeamName == teamNameFilter && q.FullTextQuery == searchQuery
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task PatchTeam_ValidRequest_ReturnsOkResultWithUpdatedTeam()
    {
        // Arrange
        var teamId = 1;
        var request = new PatchTeamRequest
        {
            TeamName = "Updated Name",
            BusinessUnit = "Updated BU",
            PTL = "Updated PTL",
        };
        var updatedTeam = new Team
        {
            Id = teamId,
            TeamName = request.TeamName,
            BusinessUnit = request.BusinessUnit,
            PTL = request.PTL,
        };

        _mediatorMock
            .Setup(m =>
                m.Send(
                    It.Is<PatchTeamCommand>(cmd => cmd.Id == teamId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(updatedTeam);

        // Act
        var actionResult = await _controller.Patch(teamId, request);

        // Assert
        Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as GetTeamResponse;
        Assert.That(response, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(response.Id, Is.EqualTo(updatedTeam.Id));
            Assert.That(response.TeamName, Is.EqualTo(updatedTeam.TeamName));
            Assert.That(response.BusinessUnit, Is.EqualTo(updatedTeam.BusinessUnit));
            Assert.That(response.PTL, Is.EqualTo(updatedTeam.PTL));
        });

        _mediatorMock.Verify(
            m =>
                m.Send(
                    It.Is<PatchTeamCommand>(cmd =>
                        cmd.Id == teamId
                        && cmd.TeamName == request.TeamName
                        && cmd.BusinessUnit == request.BusinessUnit
                        && cmd.PTL == request.PTL
                    ),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    [Test]
    public void PatchTeam_TeamNotFound_ThrowsTeamNotFoundException()
    {
        // Arrange
        var teamId = 1;
        var request = new PatchTeamRequest { TeamName = "Test" };
        _mediatorMock
            .Setup(m =>
                m.Send(
                    It.Is<PatchTeamCommand>(cmd => cmd.Id == teamId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new TeamNotFoundException(teamId));

        // Act & Assert
        Assert.ThrowsAsync<TeamNotFoundException>(() => _controller.Patch(teamId, request));
    }

    [Test]
    public void PatchTeam_TeamNameAlreadyExists_ThrowsTeamNameAlreadyExistsException()
    {
        // Arrange
        var teamId = 1;
        var existingName = "Test TeamName";
        _mediatorMock
            .Setup(m =>
                m.Send(
                    It.Is<PatchTeamCommand>(cmd => cmd.Id == teamId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new TeamNameAlreadyExistsException(existingName));

        var request = new PatchTeamRequest { TeamName = existingName };

        // Act & Assert
        Assert.ThrowsAsync<TeamNameAlreadyExistsException>(() =>
            _controller.Patch(teamId, request)
        );
    }

    [Test]
    public void PatchTeam_MediatorThrowsGenericException_ThrowsGenericException()
    {
        // Arrange
        _mediatorMock
            .Setup(mediator =>
                mediator.Send(It.IsAny<PatchTeamCommand>(), It.IsAny<CancellationToken>())
            )
            .ThrowsAsync(new InvalidDataException("An error message"));
        var request = new PatchTeamRequest { TeamName = "Testing" };

        // Act & Assert
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Patch(1, request));
    }

    [Test]
    public async Task DeleteTeam_ValidId_ReturnsOkResultWithTeamId()
    {
        // Arrange
        var teamId = 1;
        _mediatorMock
            .Setup(m =>
                m.Send(
                    It.Is<DeleteTeamCommand>(cmd => cmd.Id == teamId),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);

        // Act
        var actionResult = await _controller.Delete(teamId);

        // Assert
        Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        var response = okResult.Value as DeleteTeamResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.TeamId, Is.EqualTo(teamId));
    }

    [Test]
    public async Task DeleteTeam_InvalidId_ReturnsBadRequest()
    {
        // Arrange
        var teamId = 0;

        // Act
        var actionResult = await _controller.Delete(teamId);

        // Assert
        Assert.That(actionResult.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = actionResult.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(
            (badRequestResult.Value as ErrorResponse)?.Message,
            Is.EqualTo("TeamId can't be smaller than or equal to 0")
        );
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<DeleteTeamCommand>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public void DeleteTeam_TeamNotFound_ThrowsTeamNotFoundException()
    {
        // Arrange
        var teamId = 1;
        _mediatorMock
            .Setup(m =>
                m.Send(
                    It.Is<DeleteTeamCommand>(cmd => cmd.Id == teamId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new TeamNotFoundException(teamId));

        // Act & Assert
        Assert.ThrowsAsync<TeamNotFoundException>(() => _controller.Delete(teamId));
    }

    [Test]
    public void DeleteTeam_ProjectsStillLinked_ThrowsTeamStillLinkedToProjectsException()
    {
        // Arrange
        var teamId = 1;
        var team = new Team()
        {
            Id = teamId,
            TeamName = "Test TeamName",
            BusinessUnit = "Test BU",
        };
        _mediatorMock
            .Setup(mediator =>
                mediator.Send(
                    It.Is<DeleteTeamCommand>(cmd => cmd.Id == teamId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new TeamStillLinkedToProjectsException(projectIds: [1, 2, 3], team: team));

        // Act & Assert
        Assert.ThrowsAsync<TeamStillLinkedToProjectsException>(() => _controller.Delete(teamId));
    }

    [Test]
    public void DeleteTeam_MediatorThrowsGenericException_ThrowsGenericException()
    {
        // Arrange
        var teamId = 1;
        _mediatorMock
            .Setup(mediator =>
                mediator.Send(
                    It.Is<DeleteTeamCommand>(cmd => cmd.Id == teamId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new InvalidDataException("An error message"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidDataException>(() => _controller.Delete(teamId));
    }

    [Test]
    public async Task GetLinkedProjects_ValidId_ReturnsOkWithProjectIds()
    {
        // Arrange
        var teamId = 1;
        var projectSlugs = new List<string> { "slug_1", "slug_2" };
        _mediatorMock
            .Setup(m =>
                m.Send(
                    It.Is<GetLinkedProjectsQuery>(q => q.Id == teamId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(projectSlugs);

        // Act
        var actionResult = await _controller.GetLinkedProjects(teamId);

        // Assert
        Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var response = okResult.Value as GetLinkedProjectsResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.ProjectSlugs, Is.EqualTo(projectSlugs));
    }

    [Test]
    public async Task GetLinkedProjects_NoLinkedProjects_ReturnsOkWithEmptyList()
    {
        // Arrange
        var teamId = 1;
        _mediatorMock
            .Setup(m =>
                m.Send(
                    It.Is<GetLinkedProjectsQuery>(q => q.Id == teamId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([]);

        // Act
        var actionResult = await _controller.GetLinkedProjects(teamId);

        // Assert
        Assert.That(actionResult.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = actionResult.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var response = okResult.Value as GetLinkedProjectsResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.ProjectSlugs, Is.Empty);
    }

    [Test]
    public async Task GetLinkedProjects_InvalidId_ReturnsBadRequest()
    {
        // Arrange
        var teamId = 0;

        // Act
        var actionResult = await _controller.GetLinkedProjects(teamId);

        // Assert
        Assert.That(actionResult.Result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = actionResult.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(
            (badRequestResult.Value as ErrorResponse)?.Message,
            Is.EqualTo("TeamId can't be smaller than or equal to 0")
        );
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<GetLinkedProjectsQuery>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public void GetLinkedProjects_TeamNotFound_ThrowsTeamNotFoundException()
    {
        // Arrange
        var teamId = 1;
        _mediatorMock
            .Setup(m =>
                m.Send(
                    It.Is<GetLinkedProjectsQuery>(q => q.Id == teamId),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new TeamNotFoundException(teamId));

        // Act & Assert
        Assert.ThrowsAsync<TeamNotFoundException>(() => _controller.GetLinkedProjects(teamId));
    }
}
