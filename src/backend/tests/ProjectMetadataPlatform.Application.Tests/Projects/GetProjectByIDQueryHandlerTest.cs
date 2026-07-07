using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Errors.AuthorizationExceptions;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class GetProjectByIdQueryHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetProjectQueryHandler(
            _mockProjectRepo.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    private GetProjectQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;
    private Mock<IAuthorizationService> _authorizationServiceMock;

    [Test]
    public async Task HandleGetProjectRequest_NonexistentProject_Test()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Project>(),
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
        _ = _mockProjectRepo.Setup(m => m.GetProjectAsync(2))!.ReturnsAsync((Project?)null);
        var query = new GetProjectQuery(2);
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task HandleGetProjectRequest_Test()
    {
        var projectsResponseContent = new Project
        {
            Id = 2,
            ProjectName = "Regen",
            Slug = "regen",
            ClientName = "Nasa",
            CompanyId = 1000000000,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Project>(),
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
        _ = _mockProjectRepo.Setup(m => m.GetProjectAsync(2)).ReturnsAsync(projectsResponseContent);
        var query = new GetProjectQuery(2);
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<Project>());
        Assert.That(result, Is.EqualTo(projectsResponseContent));
    }

    [Test]
    public async Task GetProject_AuthorizationFailsThrowsTest()
    {
        _ = _authorizationServiceMock
            .Setup(a =>
                a.CheckAccess(
                    It.IsAny<Project>(),
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

        var request = new GetProjectQuery(2);

        _ = Assert.ThrowsAsync<UnauthorizedException>(() =>
            _handler.Handle(request, It.IsAny<CancellationToken>())
        );
    }
}
