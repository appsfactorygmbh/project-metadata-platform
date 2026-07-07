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
using ProjectMetadataPlatform.Domain.Teams;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

[TestFixture]
public class GetAllProjectsQueryHandlerTest
{
    private GetAllProjectsQueryHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;
    private Mock<IAuthorizationService> _authorizationServiceMock;

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

    [Test]
    public async Task CallsRepositoryWithRequest()
    {
        _ = _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>()))
            .ReturnsAsync(new List<Project> { }.BuildMock());

        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<Project>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<Project> query, Dictionary<string, string>? dict) => query);
        var request = new GetAllProjectsQuery(null, "");
        _ = await _handler.Handle(request, CancellationToken.None);

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
            BusinessUnit = new() { BusinessUnitName = "Health" },
            BusinessUnitId = 1,
        };
        _ = _authorizationServiceMock
            .Setup(a =>
                a.TryGetPlanResourceQuery(
                    It.IsAny<IQueryable<Project>>(),
                    It.IsAny<Dictionary<string, string>?>()
                )
            )
            .ReturnsAsync((IQueryable<Project> query, Dictionary<string, string>? dict) => query);
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
                Company = new() { CompanyName = "Ag der Ags" },
                CompanyId = 1,
                IsmsLevel = SecurityLevel.HIGH,
            },
            new()
            {
                Id = 2,
                ProjectName = "James",
                Slug = "james",
                ClientName = "Lucifer",
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

                Company = new() { CompanyName = "Ark" },
                CompanyId = 2,
                IsmsLevel = SecurityLevel.HIGH,
            },
        };

        _ = _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>()))
            .ReturnsAsync(projects.BuildMock());
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
            .ReturnsAsync((IQueryable<Project> query, Dictionary<string, string>? dict) => query);
        _ = _mockProjectRepo
            .Setup(m => m.GetProjectsAsync(It.IsAny<GetAllProjectsQuery>()))
            .ReturnsAsync(projects.BuildMock());
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

    [Test]
    public async Task HandleGetFilteredProjects_Test()
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
        var request = new GetAllProjectsQuery(null, null);
        var result = (await _handler.Handle(request, It.IsAny<CancellationToken>())).ToList();

        Assert.That(result, Is.EquivalentTo(projects));
    }
}
