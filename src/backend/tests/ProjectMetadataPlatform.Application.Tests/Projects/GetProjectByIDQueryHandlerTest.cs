using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class GetProjectByIdQueryHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetProjectQueryHandler(_mockProjectRepo.Object);
    }

    private GetProjectQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [Test]
    public async Task HandleGetProjectRequest_NonexistentProject_Test()
    {
        _mockProjectRepo.Setup(m => m.GetProjectAsync(2))!.ReturnsAsync((Project?)null);
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
        };
        _mockProjectRepo.Setup(m => m.GetProjectAsync(2)).ReturnsAsync(projectsResponseContent);
        var query = new GetProjectQuery(2);
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<Project>());
        Assert.That(result, Is.EqualTo(projectsResponseContent));
    }
}
