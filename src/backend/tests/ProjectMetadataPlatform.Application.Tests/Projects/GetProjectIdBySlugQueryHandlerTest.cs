using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class GetProjectIdBySlugQueryHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetProjectIdBySlugQueryHandler(_mockProjectRepo.Object);
    }

    private GetProjectIdBySlugQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [Test]
    public async Task HandleGetProjectRequest_Test()
    {
        _mockProjectRepo.Setup(m => m.GetProjectIdBySlugAsync("test")).ReturnsAsync(2);

        var query = new GetProjectIdBySlugQuery("test");
        var result = await _handler.Handle(query, It.IsAny<CancellationToken>());

        Assert.That(result, Is.InstanceOf<int?>());
        Assert.That(result, Is.EqualTo(2));
    }
}
