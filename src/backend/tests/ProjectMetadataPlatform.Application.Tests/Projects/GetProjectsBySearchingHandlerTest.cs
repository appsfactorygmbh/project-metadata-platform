using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class GetProjectsBySearchingHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetAllProjectsQueryHandler(_mockProjectRepo.Object);
    }

    private GetAllProjectsQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [Test]
    public async Task HandleGetProjectBySearchRequest_NonexistentProject_Test()
    {
        var emptyProjectList = Array.Empty<Project>();

        var query = new GetAllProjectsQuery(null, "M");
        _mockProjectRepo.Setup(m => m.GetProjectsAsync(query)).ReturnsAsync(emptyProjectList);

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
            },
        };

        var query = new GetAllProjectsQuery(null, "R");

        _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(query))
            .ReturnsAsync(projectsResponseContent);

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
            },
            new()
            {
                Id = 3,
                ProjectName = "Sonne",
                Slug = "sonne",
                ClientName = "Nasa",
            },
        };
        _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>()))
            .ReturnsAsync(projectsResponseContent);
        var query = new GetAllProjectsQuery(null, "");
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result, Is.EqualTo(projectsResponseContent));
    }
}
