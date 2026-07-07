using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class GetProjectsBySearchingHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _authorizationServiceMock = new Mock<IAuthorizationService>();
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetAllProjectsQueryHandler(
            _mockProjectRepo.Object,
            authorizationService: _authorizationServiceMock.Object
        );
    }

    private Mock<IAuthorizationService> _authorizationServiceMock;
    private GetAllProjectsQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [Test]
    public async Task HandleGetProjectBySearchRequest_NonexistentProject_Test()
    {
        var emptyProjectList = Array.Empty<Project>();
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<Project>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<Project> query, Dictionary<string, string>? dict) => query);
        var query = new GetAllProjectsQuery(null, "M");
        _ = _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(query))
            .ReturnsAsync(emptyProjectList.BuildMock());

        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task HandleGetProjectRequestBySearching_Test()
    {
        var projectsResponseContent = new List<Project>
        {
            new()
            {
                Id = 2,
                ProjectName = "Regen",
                Slug = "regen",
                ClientName = "Nasa",
                CompanyId = 250,
            },
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<Project>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<Project> query, Dictionary<string, string>? dict) => query);
        var query = new GetAllProjectsQuery(null, "R");

        _ = _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(query))
            .ReturnsAsync(projectsResponseContent.BuildMock());

        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo(projectsResponseContent));
    }

    [Test]
    public async Task HandleGetProjectRequestBySearchingWithNullSearch_Test()
    {
        var projectsResponseContent = new List<Project>
        {
            new()
            {
                Id = 2,
                ProjectName = "Regen",
                Slug = "regen",
                ClientName = "Nasa",
                CompanyId = 0,
            },
            new()
            {
                Id = 3,
                ProjectName = "Sonne",
                Slug = "sonne",
                ClientName = "Nasa",
                CompanyId = -12,
            },
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<Project>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<Project> query, Dictionary<string, string>? dict) => query);
        _ = _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>()))
            .ReturnsAsync(projectsResponseContent.BuildMock());
        var query = new GetAllProjectsQuery(null, "");
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo(projectsResponseContent));
    }

    [Test]
    public async Task HandleGetFilteredProjectsBySearch_Test()
    {
        var projects = new List<Project>
        {
            new()
            {
                Id = 5,
                ProjectName = "Aapfel",
                Slug = "marika",
                ClientName = "Zatan",
                TeamId = 1,
                Company = new() { CompanyName = "Ark" },
                CompanyId = 2,
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 1,
                ProjectName = "Beta",
                Slug = "heather",
                ClientName = "Metatron",
                TeamId = 1,
                Company = new() { CompanyName = "Ag der Ags" },
                CompanyId = 1,
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 2,
                ProjectName = "Apfel",
                Slug = "james",
                ClientName = "Metatron",
                TeamId = 1,
                Company = new() { CompanyName = "Ag der Ags" },
                CompanyId = 1,
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                ClientName = "Satan",
                TeamId = 1,
                Company = new() { CompanyName = "Ark" },
                CompanyId = 2,
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 4,
                ProjectName = "Aarika",
                Slug = "marika",
                ClientName = "Satan",
                TeamId = 1,
                Company = new() { CompanyName = "Ark" },
                CompanyId = 2,
                IsmsLevel = SecurityLevel.HIGH,
            },
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<Project>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<Project>?)null);

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
        _ = _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>()))
            .ReturnsAsync(projects.BuildMock());
        var request = new GetAllProjectsQuery(null, "");
        var result = (await _handler.Handle(request, It.IsAny<CancellationToken>())).ToList();

        Assert.That(result, Is.EquivalentTo(projects));
    }
}
