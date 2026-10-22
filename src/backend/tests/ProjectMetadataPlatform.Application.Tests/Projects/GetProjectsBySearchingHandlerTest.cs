using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MockQueryable;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Helper.Models;
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
        _paginationHelperMock = new Mock<IPaginationHelper>();
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetAllProjectsQueryHandler(
            _mockProjectRepo.Object,
            authorizationService: _authorizationServiceMock.Object,
            _paginationHelperMock.Object
        );
    }

    private Mock<IAuthorizationService> _authorizationServiceMock;
    private Mock<IPaginationHelper> _paginationHelperMock;
    private GetAllProjectsQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [Test]
    public async Task HandleGetProjectBySearchRequest_NonexistentProject_Test()
    {
        var emptyProjectList = Array.Empty<Project>();
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Project>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Project> query, int? limit) => (query, null));
        var query = new GetAllProjectsQuery(null, "M", null, null);
        _ = _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(query, null))
            .ReturnsAsync(emptyProjectList.BuildMock());

        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());
        Assert.That(result.Item1, Is.Empty);
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
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Project>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Project> query, int? limit) => (query, null));
        var query = new GetAllProjectsQuery(null, "R", null, null);

        _ = _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(query, null))
            .ReturnsAsync(projectsResponseContent.BuildMock());

        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result.Item1, Is.EqualTo(projectsResponseContent));
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
        _ = _paginationHelperMock
            .Setup(a => a.PaginateWithAuthAsync(It.IsAny<IQueryable<Project>>(), It.IsAny<int?>()))
            .ReturnsAsync((IQueryable<Project> query, int? limit) => (query, null));
        _ = _mockProjectRepo
            .Setup(m =>
                m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>(), It.IsAny<ProjectCursor?>())
            )
            .ReturnsAsync(projectsResponseContent.BuildMock());
        var query = new GetAllProjectsQuery(null, "", null, null);
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result.Item1, Is.EqualTo(projectsResponseContent));
    }
}
