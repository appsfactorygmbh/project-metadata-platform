using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Teams;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Tests.Teams;

[TestFixture]
public class GetLinkedProjectsQueryHandlerTest
{
    private GetLinkedProjectsQueryHandler _handler;
    private Mock<ITeamRepository> _mockTeamRepository;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockTeamRepository = new Mock<ITeamRepository>();
        _handler = new GetLinkedProjectsQueryHandler(
            teamRepository: _mockTeamRepository.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    [Test]
    public async Task GetLinkedProjects_CallsRepositoryCorrectly()
    {
        // Arrange
        var returnTeam = new Team()
        {
            Id = 1,
            TeamName = "Test_1",
            BusinessUnit = new() { BusinessUnitName = "BU Test" },
            BusinessUnitId = 1,
            PTL = "Max Mustermann",
            Projects =
            [
                new()
                {
                    Id = 111,
                    ProjectName = "Projects",
                    Slug = "project_1",
                    ClientName = "Project Client",
                    CompanyId = 1,
                },
                new()
                {
                    Id = 222,
                    ProjectName = "Projects",
                    Slug = "project_2",
                    ClientName = "Project Client",
                    CompanyId = 1,
                },
            ],
        };

        _ = _mockTeamRepository
            .Setup(repo => repo.GetTeamWithProjectsAsync(It.IsAny<int>()))
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
        var result = await _handler.Handle(
            new GetLinkedProjectsQuery(Id: 1),
            It.IsAny<CancellationToken>()
        );

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
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

    [Test]
    public async Task GetLinkedProjects_AuthorizationFailsThrowsTest()
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

        var request = new GetLinkedProjectsQuery(Id: 1);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
