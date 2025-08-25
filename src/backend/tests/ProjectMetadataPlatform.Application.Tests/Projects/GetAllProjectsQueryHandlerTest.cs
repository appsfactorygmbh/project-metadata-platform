using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class GetAllProjectsQueryHandlerTest
{
    private GetAllProjectsQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;

    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _handler = new GetAllProjectsQueryHandler(_mockProjectRepo.Object);
    }

    [Test]
    public async Task CallsRepositoryWithRequest()
    {
        _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>()))
            .ReturnsAsync([]);
        var request = new GetAllProjectsQuery(null, "");
        await _handler.Handle(request, CancellationToken.None);

        _mockProjectRepo.Verify(repository => repository.GetProjectsAsync(request), Times.Once);
        _mockProjectRepo.VerifyNoOtherCalls();
    }

    [Test]
    public async Task ReturnsProjectsFromRepository()
    {
        var request = new GetAllProjectsQuery(null, "");
        var team = new Team()
        {
            TeamName = "AF_1",
            Id = 1,
            BusinessUnit = "Health",
        };
        var projects = new List<Project>
        {
            new()
            {
                Id = 1,
                ProjectName = "Heather",
                Slug = "heather",
                ClientName = "Metatron",
                Team = team,
                TeamId = 1,
                Company = "Ag der Ags",
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                ClientName = "Lucifer",
                Company = "Ag der Ags",
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                ClientName = "Satan",
                Company = "Ark",
                IsmsLevel = SecurityLevel.HIGH,
            },
        };

        _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>()))
            .ReturnsAsync(projects);
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.EquivalentTo(projects));
    }

    [Test]
    public async Task HandleGetProjectsAlphabetical_Test()
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
                Company = "Ark",
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 1,
                ProjectName = "Beta",
                Slug = "heather",
                ClientName = "Metatron",
                TeamId = 1,
                Company = "Ag der Ags",
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 2,
                ProjectName = "Apfel",
                Slug = "james",
                ClientName = "Metatron",
                TeamId = 1,
                Company = "Ag der Ags",
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 3,
                ProjectName = "Marika",
                Slug = "marika",
                ClientName = "Satan",
                TeamId = 1,
                Company = "Ark",
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 4,
                ProjectName = "Aarika",
                Slug = "marika",
                ClientName = "Satan",
                TeamId = 1,
                Company = "Ark",
                IsmsLevel = SecurityLevel.HIGH,
            },
        };

        _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>()))
            .ReturnsAsync(projects);
        var request = new GetAllProjectsQuery(null, null);
        var result = (await _handler.Handle(request, It.IsAny<CancellationToken>())).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result[0].Id, Is.EqualTo(2));
            Assert.That(result[1].Id, Is.EqualTo(1));
            Assert.That(result[2].Id, Is.EqualTo(4));
            Assert.That(result[3].Id, Is.EqualTo(3));
            Assert.That(result[4].Id, Is.EqualTo(5));
        });
    }
}
