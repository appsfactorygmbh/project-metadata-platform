using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Helper;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Projects;
using ProjectMetadataPlatform.Domain.Errors.PluginExceptions;
using ProjectMetadataPlatform.Domain.Errors.ProjectExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Projects;

public class UpdateProjectCommandHandlerTest
{
    private UpdateProjectCommandHandler _handler;
    private Mock<IProjectsRepository> _mockProjectRepo;
    private Mock<IPluginRepository> _mockPluginRepo;
    private Mock<ITeamRepository> _mockTeamRepository;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ILogRepository> _mockLogRepository;
    private Mock<ISlugHelper> _mockSlugHelper;

    [SetUp]
    public void Setup()
    {
        _mockProjectRepo = new Mock<IProjectsRepository>();
        _mockPluginRepo = new Mock<IPluginRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockTeamRepository = new Mock<ITeamRepository>();
        _mockLogRepository = new Mock<ILogRepository>();
        _mockSlugHelper = new Mock<ISlugHelper>();
        _handler = new UpdateProjectCommandHandler(
            _mockProjectRepo.Object,
            _mockPluginRepo.Object,
            _mockTeamRepository.Object,
            _mockLogRepository.Object,
            _mockUnitOfWork.Object,
            _mockSlugHelper.Object
        );
    }

    [Test]
    public async Task UpdateProject_Test()
    {
        var exampleProject = new Project
        {
            Id = 1,
            ProjectName = "Example Project",
            Slug = "example project",
            ClientName = "Example Client",
            OfferId = "Example OfferId",
            Company = "Example Company",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.HIGH,
            ProjectPlugins = [],
            Notes = "Example Notes",
        };
        var examplePlugin = new Plugin
        {
            Id = 100,
            IsArchived = false,
            PluginName = "Dummy",
        };
        var projectPlugin = new ProjectPlugins
        {
            Plugin = examplePlugin,
            Project = exampleProject,
            PluginId = 100,
            ProjectId = 100,
            Url = "dummy",
            DisplayName = "Dummy",
        };
        var projectPluginList = new List<ProjectPlugins> { projectPlugin };

        _mockProjectRepo.Setup(m => m.GetProjectWithPluginsAsync(1)).ReturnsAsync(exampleProject);

        _mockPluginRepo.Setup(m => m.CheckPluginExists(It.IsAny<int>())).ReturnsAsync(true);
        _mockPluginRepo
            .Setup(repo => repo.GetGlobalPluginsAsync())
            .ReturnsAsync([new Plugin { Id = 100, PluginName = "Example Plugin" }]);

        var result = await _handler.Handle(
            new UpdateProjectCommand(
                ProjectName: exampleProject.ProjectName,
                ClientName: exampleProject.ClientName,
                OfferId: exampleProject.OfferId,
                Company: exampleProject.Company,
                CompanyState: exampleProject.CompanyState,
                IsmsLevel: exampleProject.IsmsLevel,
                Id: exampleProject.Id,
                Plugins: projectPluginList,
                IsArchived: false,
                TeamId: null,
                Notes: "Example Notes"
            ),
            It.IsAny<CancellationToken>()
        );
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void UpdateProjectNotFound_Test()
    {
        var exampleProject = new Project
        {
            Id = 1,
            ProjectName = "Example Project",
            Slug = "example project",
            ClientName = "Example Client",
            OfferId = "Example OfferId",
            Company = "Example Company",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.HIGH,
            ProjectPlugins = [],
            Notes = "Example Notes",
        };
        var examplePlugin = new Plugin
        {
            Id = 100,
            IsArchived = false,
            PluginName = "Dummy",
        };
        var projectPlugin = new ProjectPlugins
        {
            Plugin = examplePlugin,
            Project = exampleProject,
            PluginId = 100,
            ProjectId = 100,
            Url = "dummy",
            DisplayName = "Dummy",
        };
        var projectPluginList = new List<ProjectPlugins> { projectPlugin };

        _mockProjectRepo.Setup(m => m.CheckProjectExists(1)).ReturnsAsync(false);

        _mockPluginRepo.Setup(m => m.CheckPluginExists(It.IsAny<int>())).ReturnsAsync(true);

        var exception = Assert.ThrowsAsync<ProjectNotFoundException>(async () =>
            await _handler.Handle(
                new UpdateProjectCommand(
                    ProjectName: exampleProject.ProjectName,
                    ClientName: exampleProject.ClientName,
                    OfferId: exampleProject.OfferId,
                    Company: exampleProject.Company,
                    CompanyState: exampleProject.CompanyState,
                    IsmsLevel: exampleProject.IsmsLevel,
                    Id: exampleProject.Id,
                    Plugins: projectPluginList,
                    IsArchived: false,
                    TeamId: null,
                    Notes: "Example Notes"
                ),
                CancellationToken.None
            )
        );
        Assert.That(exception.Message, Is.EqualTo("The project with id 1 was not found."));
    }

    [Test]
    public void UpdatePluginNotFound_Test()
    {
        var exampleProject = new Project
        {
            Id = 1,
            ProjectName = "Example Project",
            Slug = "example project",
            ClientName = "Example Client",
            OfferId = "Example OfferId",
            Company = "Example Company",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.HIGH,
            ProjectPlugins = [],
            Notes = "Example Notes",
        };
        var examplePlugin = new Plugin
        {
            Id = 200,
            IsArchived = false,
            PluginName = "Dummy",
        };
        var projectPlugin = new ProjectPlugins
        {
            Plugin = examplePlugin,
            Project = exampleProject,
            PluginId = 100,
            ProjectId = 100,
            Url = "dummy",
            DisplayName = "Dummy",
        };
        var projectPluginList = new List<ProjectPlugins> { projectPlugin };

        _mockProjectRepo.Setup(m => m.GetProjectWithPluginsAsync(1)).ReturnsAsync(exampleProject);

        _mockPluginRepo.Setup(repo => repo.GetGlobalPluginsAsync()).ReturnsAsync([]);

        var exception = Assert.ThrowsAsync<MultiplePluginsNotFoundException>(async () =>
            await _handler.Handle(
                new UpdateProjectCommand(
                    ProjectName: exampleProject.ProjectName,
                    ClientName: exampleProject.ClientName,
                    OfferId: exampleProject.OfferId,
                    Company: exampleProject.Company,
                    CompanyState: exampleProject.CompanyState,
                    IsmsLevel: exampleProject.IsmsLevel,
                    Id: exampleProject.Id,
                    Plugins: projectPluginList,
                    IsArchived: false,
                    TeamId: null,
                    Notes: "Example Notes"
                ),
                CancellationToken.None
            )
        );
        Assert.That(exception.Message, Is.EqualTo("The Plugins with these ids do not exist: 100"));
    }

    [Test]
    public async Task UpdatesProjectInformation()
    {
        //Arrange
        var project = new Project
        {
            Id = 1,
            ProjectName = "Db App",
            Slug = "db app",
            ClientName = "DB",
            OfferId = "Offer 1",
            Company = "DeutscheBahn",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.HIGH,
            ProjectPlugins = [],
            Notes = "Example Notes",
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: "DB App",
            ClientName: "Deutsche Bahn",
            OfferId: "Offer 2",
            Company: "DB",
            CompanyState: CompanyState.INTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            Id: 1,
            Plugins: [],
            IsArchived: false,
            TeamId: null,
            Notes: "Updated Notes"
        );

        _mockProjectRepo
            .Setup(repository => repository.GetProjectWithPluginsAsync(1))
            .ReturnsAsync(project);

        //Act
        await _handler.Handle(updateCommand, CancellationToken.None);

        //Assert
        _mockUnitOfWork.Verify(unitOfWork => unitOfWork.CompleteAsync());
        Assert.Multiple(() =>
        {
            Assert.That(project.ClientName, Is.EqualTo("Deutsche Bahn"));
            Assert.That(project.ProjectName, Is.EqualTo("DB App"));
            Assert.That(project.OfferId, Is.EqualTo("Offer 2"));
            Assert.That(project.Company, Is.EqualTo("DB"));
            Assert.That(project.CompanyState, Is.EqualTo(CompanyState.INTERNAL));
            Assert.That(project.IsmsLevel, Is.EqualTo(SecurityLevel.NORMAL));
            Assert.That(project.Notes, Is.EqualTo("Updated Notes"));
        });
    }

    [Test]
    public async Task UpdatesProjectPlugins()
    {
        //Arrange
        var project = new Project
        {
            Id = 1,
            ProjectName = "Db App",
            Slug = "db app",
            ClientName = "DB",
            OfferId = "Offer 1",
            Company = "DeutscheBahn",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.HIGH,
            ProjectPlugins =
            [
                new ProjectPlugins
                {
                    PluginId = 1,
                    Url = "https://example.com",
                    DisplayName = "Example Plugin",
                },
                new ProjectPlugins
                {
                    PluginId = 1,
                    Url = "https://another-example.com",
                    DisplayName = "Another Plugin",
                },
                new ProjectPlugins
                {
                    PluginId = 2,
                    Url = "https://different-example.com",
                    DisplayName = "Different Plugin",
                },
            ],
            Notes = "Example Notes",
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: "DB App",
            ClientName: "Unit 2",
            OfferId: "Offer id 2",
            Company: "DB",
            CompanyState: CompanyState.INTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            Id: 1,
            Plugins:
            [
                new ProjectPlugins
                {
                    PluginId = 1,
                    Url = "https://example.com",
                    DisplayName = "Example Plugin",
                },
                new ProjectPlugins
                {
                    PluginId = 1,
                    Url = "https://another-example.com",
                    DisplayName = "Another example Plugin",
                },
                new ProjectPlugins
                {
                    PluginId = 3,
                    Url = "https://example2.com",
                    DisplayName = "Example 2 Plugin",
                },
            ],
            Notes: "Updated Notes",
            IsArchived: false,
            TeamId: null
        );

        _mockProjectRepo
            .Setup(repository => repository.GetProjectWithPluginsAsync(1))
            .ReturnsAsync(project);
        _mockPluginRepo
            .Setup(repo => repo.GetGlobalPluginsAsync())
            .ReturnsAsync(
                [
                    new Plugin { Id = 1, PluginName = "Plugin1" },
                    new Plugin { Id = 2, PluginName = "Plugin2" },
                    new Plugin { Id = 3, PluginName = "Plugin3" },
                ]
            );

        //Act
        await _handler.Handle(updateCommand, CancellationToken.None);

        //Assert
        _mockUnitOfWork.Verify(unitOfWork => unitOfWork.CompleteAsync());

        Assert.That(project.ProjectPlugins, Has.Count.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(project.ProjectPlugins.ElementAt(0).PluginId, Is.EqualTo(1));
            Assert.That(project.ProjectPlugins.ElementAt(0).Url, Is.EqualTo("https://example.com"));
            Assert.That(
                project.ProjectPlugins.ElementAt(0).DisplayName,
                Is.EqualTo("Example Plugin")
            );
        });

        Assert.Multiple(() =>
        {
            Assert.That(project.ProjectPlugins.ElementAt(1).PluginId, Is.EqualTo(1));
            Assert.That(
                project.ProjectPlugins.ElementAt(1).Url,
                Is.EqualTo("https://another-example.com")
            );
            Assert.That(
                project.ProjectPlugins.ElementAt(1).DisplayName,
                Is.EqualTo("Another example Plugin")
            );
        });

        Assert.Multiple(() =>
        {
            Assert.That(project.ProjectPlugins.ElementAt(2).PluginId, Is.EqualTo(3));
            Assert.That(
                project.ProjectPlugins.ElementAt(2).Url,
                Is.EqualTo("https://example2.com")
            );
            Assert.That(
                project.ProjectPlugins.ElementAt(2).DisplayName,
                Is.EqualTo("Example 2 Plugin")
            );
        });
    }

    [Test]
    public async Task UpdateProject_IsArchivedFlag_Test()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Db App",
            Slug = "db app",
            ClientName = "DB",
            OfferId = "Offer 1",
            Company = "DeutscheBahn",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.HIGH,
            ProjectPlugins = [],
            IsArchived = false,
            Notes = "Example Notes",
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: "DB App",
            ClientName: "Unit 2",
            OfferId: "Offer 2",
            Company: "DB",
            CompanyState: CompanyState.INTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            Id: 1,
            Plugins: [],
            IsArchived: true,
            TeamId: null,
            Notes: "Example Notes"
        );

        _mockProjectRepo
            .Setup(repository => repository.GetProjectWithPluginsAsync(1))
            .ReturnsAsync(project);

        await _handler.Handle(updateCommand, CancellationToken.None);

        _mockUnitOfWork.Verify(unitOfWork => unitOfWork.CompleteAsync());
        Assert.That(project.IsArchived, Is.True);
    }

    [Test]
    public async Task LogsChanges_WhenProjectPropertiesAreUpdated()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Old Project Name",
            Slug = "old project name",
            ClientName = "Old Client",
            OfferId = "Old Offer",
            Company = "Old Company",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.HIGH,
            IsArchived = false,
            TeamId = null,
            Notes = "Old Notes",
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: "New Project Name",
            ClientName: "New Client",
            OfferId: "New Offer",
            Company: "New Company",
            CompanyState: CompanyState.INTERNAL,
            IsmsLevel: SecurityLevel.NORMAL,
            Id: 1,
            Plugins: [],
            IsArchived: false,
            TeamId: null,
            Notes: "New Notes"
        );

        var slugHelper = new SlugHelper(_mockProjectRepo.Object);
        var slug = slugHelper.GenerateSlug("New Project Name");

        _mockProjectRepo.Setup(repo => repo.GetProjectWithPluginsAsync(1)).ReturnsAsync(project);
        _mockSlugHelper.Setup(s => s.GenerateSlug("New Project Name")).Returns(slug);
        _mockSlugHelper.Setup(s => s.CheckProjectSlugExists(slug)).ReturnsAsync(false);

        await _handler.Handle(updateCommand, CancellationToken.None);

        _mockLogRepository.Verify(
            logRepo =>
                logRepo.AddProjectLogForCurrentUser(
                    project,
                    Action.UPDATED_PROJECT,
                    It.Is<List<LogChange>>(changes =>
                        changes.Count == 7
                        && changes.Any(change =>
                            change.Property == "ProjectName"
                            && change.OldValue == "Old Project Name"
                            && change.NewValue == "New Project Name"
                        )
                        && changes.Any(change =>
                            change.Property == "ClientName"
                            && change.OldValue == "Old Client"
                            && change.NewValue == "New Client"
                        )
                        && changes.Any(change =>
                            change.Property == "OfferId"
                            && change.OldValue == "Old Offer"
                            && change.NewValue == "New Offer"
                        )
                        && changes.Any(change =>
                            change.Property == "Company"
                            && change.OldValue == "Old Company"
                            && change.NewValue == "New Company"
                        )
                        && changes.Any(change =>
                            change.Property == "CompanyState"
                            && change.OldValue == "EXTERNAL"
                            && change.NewValue == "INTERNAL"
                        )
                        && changes.Any(change =>
                            change.Property == "IsmsLevel"
                            && change.OldValue == "HIGH"
                            && change.NewValue == "NORMAL"
                        )
                        && changes.Any(change =>
                            change.Property == "Notes"
                            && change.OldValue == "Old Notes"
                            && change.NewValue == "New Notes"
                        )
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task NoLogging_WhenNoPropertiesAreChanged()
    {
        var exampleProject = new Project
        {
            Id = 1,
            ProjectName = "No Change Project",
            Slug = "no change project",
            ClientName = "Client A",

            OfferId = "Offer A",
            Company = "Company A",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
            ProjectPlugins = [],
            IsArchived = false,
            Notes = "No Change Notes",
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: exampleProject.ProjectName,
            ClientName: exampleProject.ClientName,
            OfferId: exampleProject.OfferId,
            Company: exampleProject.Company,
            CompanyState: exampleProject.CompanyState,
            IsmsLevel: exampleProject.IsmsLevel,
            Id: exampleProject.Id,
            Plugins: exampleProject.ProjectPlugins.ToList(),
            IsArchived: false,
            TeamId: null,
            Notes: exampleProject.Notes
        );
        _mockProjectRepo
            .Setup(repo => repo.GetProjectWithPluginsAsync(exampleProject.Id))
            .ReturnsAsync(exampleProject);

        await _handler.Handle(updateCommand, CancellationToken.None);

        _mockLogRepository.Verify(
            logRepo =>
                logRepo.AddProjectLogForCurrentUser(
                    It.IsAny<Project>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
    }

    [Test]
    public async Task LogsOnlyChangedProperties()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Partial Update",
            Slug = "partial update",
            ClientName = "Client A",
            OfferId = "Offer A",
            Company = "Company A",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
            ProjectPlugins = [],
            IsArchived = false,
            TeamId = null,
            Notes = "Example Notes",
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: "Partial Update",
            ClientName: "Updated Client",
            OfferId: "Updated Offer",
            Company: "Company A",
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.VERY_HIGH,
            Id: 1,
            Plugins: project.ProjectPlugins.ToList(),
            IsArchived: false,
            TeamId: null,
            Notes: "Example Notes"
        );

        _mockProjectRepo
            .Setup(repo => repo.GetProjectWithPluginsAsync(project.Id))
            .ReturnsAsync(project);

        await _handler.Handle(updateCommand, CancellationToken.None);

        _mockLogRepository.Verify(
            logRepo =>
                logRepo.AddProjectLogForCurrentUser(
                    project,
                    Action.UPDATED_PROJECT,
                    It.Is<List<LogChange>>(changes =>
                        changes.Count == 2
                        && changes.Any(change =>
                            change.Property == "ClientName"
                            && change.OldValue == "Client A"
                            && change.NewValue == "Updated Client"
                        )
                        && changes.Any(change =>
                            change.Property == "OfferId"
                            && change.OldValue == "Offer A"
                            && change.NewValue == "Updated Offer"
                        )
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public void LogsChanges_HandlesLogRepositoryException()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Project With Exception",
            Slug = "project with exception",
            ClientName = "Client C",
            OfferId = "Offer A",
            Company = "Company A",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
            IsArchived = false,
            Notes = "Old Notes",
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: "New Project Name",
            ClientName: "New Client",
            OfferId: "Updated Offer",
            Company: "Company A",
            CompanyState: CompanyState.EXTERNAL,
            IsmsLevel: SecurityLevel.VERY_HIGH,
            Id: project.Id,
            Plugins: [],
            IsArchived: false,
            TeamId: null,
            Notes: "New Notes"
        );

        _mockProjectRepo
            .Setup(repo => repo.GetProjectWithPluginsAsync(project.Id))
            .ReturnsAsync(project);

        _mockLogRepository
            .Setup(logRepo =>
                logRepo.AddProjectLogForCurrentUser(
                    It.IsAny<Project>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                )
            )
            .Throws(new Exception("Logging error"));

        var exception = Assert.ThrowsAsync<Exception>(async () =>
            await _handler.Handle(updateCommand, CancellationToken.None)
        );
        Assert.That(exception.Message, Is.EqualTo("Logging error"));

        _mockLogRepository.Verify(
            logRepo =>
                logRepo.AddProjectLogForCurrentUser(
                    It.IsAny<Project>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                ),
            Times.Once
        );
    }

    [Test]
    public async Task ArchivesProject_WhenIsArchivedFlagIsTrue()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            Slug = "test project",
            ClientName = "Test Client",
            OfferId = "Offer A",
            Company = "Company A",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
            ProjectPlugins = [],
            IsArchived = false,
            Notes = "Example Notes",
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: project.ProjectName,
            ClientName: project.ClientName,
            OfferId: project.OfferId,
            Company: project.Company,
            CompanyState: project.CompanyState,
            IsmsLevel: project.IsmsLevel,
            Id: project.Id,
            Plugins: [],
            IsArchived: true,
            TeamId: null,
            Notes: project.Notes
        );

        _mockProjectRepo
            .Setup(repo => repo.GetProjectWithPluginsAsync(project.Id))
            .ReturnsAsync(project);

        await _handler.Handle(updateCommand, CancellationToken.None);

        Assert.That(project.IsArchived, Is.True);

        _mockLogRepository.Verify(
            logRepo =>
                logRepo.AddProjectLogForCurrentUser(
                    project,
                    Action.ARCHIVED_PROJECT, // Expect Action.ARCHIVED_PROJECT since the project was archived
                    It.Is<List<LogChange>>(changes =>
                        changes.Count == 1
                        && changes.Any(change =>
                            change.Property == "IsArchived"
                            && change.OldValue == "False"
                            && change.NewValue == "True"
                        )
                    )
                ),
            Times.Once
        );
        _mockLogRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task UnArchivesProject_WhenIsArchivedFlagIsFalse()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Archived Project",
            Slug = "archived project",
            ClientName = "Test Client",
            OfferId = "Offer A",
            Company = "Company A",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
            ProjectPlugins = [],
            IsArchived = true,
            Notes = "Example Notes",
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: project.ProjectName,
            ClientName: project.ClientName,
            OfferId: project.OfferId,
            Company: project.Company,
            CompanyState: project.CompanyState,
            IsmsLevel: project.IsmsLevel,
            Id: project.Id,
            Plugins: [],
            IsArchived: false,
            TeamId: null, // Assuming TeamId can be null
            Notes: project.Notes
        );

        _mockProjectRepo
            .Setup(repo => repo.GetProjectWithPluginsAsync(project.Id))
            .ReturnsAsync(project);

        await _handler.Handle(updateCommand, CancellationToken.None);

        Assert.That(project.IsArchived, Is.False);

        _mockLogRepository.Verify(
            logRepo =>
                logRepo.AddProjectLogForCurrentUser(
                    project,
                    Action.UNARCHIVED_PROJECT,
                    It.Is<List<LogChange>>(changes =>
                        changes.Count == 1
                        && changes.Any(change =>
                            change.Property == "IsArchived"
                            && change.OldValue == "True"
                            && change.NewValue == "False"
                        )
                    )
                ),
            Times.Once
        );
        _mockLogRepository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task DoesNotLogWhenIsArchivedStatusDoesNotChange()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            Slug = "test project",
            ClientName = "Test Client",
            OfferId = "Offer A",
            Company = "Company A",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
            ProjectPlugins = [],
            IsArchived = true,
            Notes = "Example Notes",
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: project.ProjectName,
            ClientName: project.ClientName,
            OfferId: project.OfferId,
            Company: project.Company,
            CompanyState: project.CompanyState,
            IsmsLevel: project.IsmsLevel,
            Id: project.Id,
            Plugins: [],
            IsArchived: true,
            TeamId: null, // Assuming TeamId can be null,
            Notes: project.Notes
        );

        _mockProjectRepo
            .Setup(repo => repo.GetProjectWithPluginsAsync(project.Id))
            .ReturnsAsync(project);

        await _handler.Handle(updateCommand, CancellationToken.None);

        Assert.That(project.IsArchived, Is.True);

        _mockLogRepository.Verify(
            logRepo =>
                logRepo.AddProjectLogForCurrentUser(
                    project,
                    Action.UPDATED_PROJECT,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
    }

    [Test]
    public async Task LogsWhenProjectPluginIsRemoved()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            Slug = "test project",
            ClientName = "Test Client",
            OfferId = "Offer A",
            Company = "Company A",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
            ProjectPlugins =
            [
                new()
                {
                    PluginId = 1,
                    Url = "https://example.com",
                    DisplayName = "Example Plugin",
                },
            ],
            Notes = "Example Notes",
            IsArchived = false,
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: project.ProjectName,
            ClientName: project.ClientName,
            OfferId: project.OfferId,
            Company: project.Company,
            CompanyState: project.CompanyState,
            IsmsLevel: project.IsmsLevel,
            Id: project.Id,
            Plugins: [],
            Notes: project.Notes,
            IsArchived: false,
            TeamId: null // Assuming TeamId can be null
        );

        _mockProjectRepo
            .Setup(repo => repo.GetProjectWithPluginsAsync(project.Id))
            .ReturnsAsync(project);
        _mockPluginRepo
            .Setup(repo => repo.GetGlobalPluginsAsync())
            .ReturnsAsync([new Plugin { Id = 1, PluginName = "ExamplePlugin" }]);

        await _handler.Handle(updateCommand, CancellationToken.None);

        _mockLogRepository.Verify(
            logRepo =>
                logRepo.AddProjectLogForCurrentUser(
                    project,
                    Action.REMOVED_PROJECT_PLUGIN,
                    It.Is<List<LogChange>>(changes =>
                        changes.Any(change =>
                            change.Property == "Plugin"
                            && change.OldValue == "ExamplePlugin"
                            && change.NewValue == String.Empty
                        )
                        && changes.Any(change =>
                            change.Property == "Url"
                            && change.OldValue == "https://example.com"
                            && change.NewValue == String.Empty
                        )
                        && changes.Any(change =>
                            change.Property == "DisplayName"
                            && change.OldValue == "Example Plugin"
                            && change.NewValue == String.Empty
                        )
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task LogsWhenProjectPluginIsAdded()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            Slug = "test project",
            ClientName = "Test Client",
            OfferId = "Offer A",
            Company = "Company A",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
            Notes = "Example Notes",
            ProjectPlugins = [],
            IsArchived = false,
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: project.ProjectName,
            ClientName: project.ClientName,
            OfferId: project.OfferId,
            Company: project.Company,
            CompanyState: project.CompanyState,
            IsmsLevel: project.IsmsLevel,
            Id: project.Id,
            Plugins:
            [
                new ProjectPlugins
                {
                    PluginId = 1,
                    Url = "https://example.com",
                    DisplayName = "Example Plugin",
                },
            ],
            Notes: project.Notes,
            IsArchived: false,
            TeamId: null // Assuming TeamId can be null
        );

        _mockProjectRepo
            .Setup(repo => repo.GetProjectWithPluginsAsync(project.Id))
            .ReturnsAsync(project);
        _mockPluginRepo
            .Setup(repo => repo.GetGlobalPluginsAsync())
            .ReturnsAsync([new Plugin { Id = 1, PluginName = "ExamplePlugin" }]);

        await _handler.Handle(updateCommand, CancellationToken.None);

        _mockLogRepository.Verify(
            logRepo =>
                logRepo.AddProjectLogForCurrentUser(
                    project,
                    Action.ADDED_PROJECT_PLUGIN,
                    It.Is<List<LogChange>>(changes =>
                        changes.Any(change =>
                            change.Property == "Plugin"
                            && change.OldValue == String.Empty
                            && change.NewValue == "ExamplePlugin"
                        )
                        && changes.Any(change =>
                            change.Property == "Url"
                            && change.OldValue == String.Empty
                            && change.NewValue == "https://example.com"
                        )
                        && changes.Any(change =>
                            change.Property == "DisplayName"
                            && change.OldValue == String.Empty
                            && change.NewValue == "Example Plugin"
                        )
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task LogsWhenProjectPluginIsUpdated()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            Slug = "test project",
            ClientName = "Test Client",
            OfferId = "Offer A",
            Company = "Company A",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
            ProjectPlugins =
            [
                new()
                {
                    PluginId = 1,
                    Url = "https://example.com",
                    DisplayName = "Example Plugin",
                },
            ],
            IsArchived = false,
            Notes = "Example Notes",
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: project.ProjectName,
            ClientName: project.ClientName,
            OfferId: project.OfferId,
            Company: project.Company,
            CompanyState: project.CompanyState,
            IsmsLevel: project.IsmsLevel,
            Id: project.Id,
            Plugins:
            [
                new ProjectPlugins
                {
                    PluginId = 1,
                    Url = "https://example.com",
                    DisplayName = "Updated Plugin",
                },
            ],
            IsArchived: false,
            TeamId: null,
            Notes: project.Notes
        );

        _mockProjectRepo
            .Setup(repo => repo.GetProjectWithPluginsAsync(project.Id))
            .ReturnsAsync(project);
        _mockPluginRepo
            .Setup(repo => repo.GetGlobalPluginsAsync())
            .ReturnsAsync([new Plugin { Id = 1, PluginName = "Example Plugin" }]);

        await _handler.Handle(updateCommand, CancellationToken.None);

        _mockLogRepository.Verify(
            logRepo =>
                logRepo.AddProjectLogForCurrentUser(
                    project,
                    Action.UPDATED_PROJECT_PLUGIN,
                    It.Is<List<LogChange>>(changes =>
                        changes.Any(change =>
                            change.Property == "DisplayName"
                            && change.OldValue == "Example Plugin"
                            && change.NewValue == "Updated Plugin"
                        )
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task NoLogsWhenNoProjectPluginChanged()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Test Project",
            Slug = "test project",
            ClientName = "Test Client",
            OfferId = "Offer A",
            Company = "Company A",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
            ProjectPlugins =
            [
                new()
                {
                    PluginId = 1,
                    Url = "https://example.com",
                    DisplayName = "Example Plugin",
                },
            ],
            IsArchived = false,
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: project.ProjectName,
            ClientName: project.ClientName,
            OfferId: project.OfferId,
            Company: project.Company,
            CompanyState: project.CompanyState,
            IsmsLevel: project.IsmsLevel,
            Id: project.Id,
            Plugins:
            [
                new ProjectPlugins
                {
                    PluginId = 1,
                    Url = "https://example.com",
                    DisplayName = "Example Plugin",
                },
            ],
            Notes: project.Notes,
            IsArchived: false,
            TeamId: null // Assuming TeamId can be null
        );

        _mockProjectRepo
            .Setup(repo => repo.GetProjectWithPluginsAsync(project.Id))
            .ReturnsAsync(project);
        _mockPluginRepo
            .Setup(repo => repo.GetGlobalPluginsAsync())
            .ReturnsAsync([new Plugin { Id = 1, PluginName = "Example Plugin" }]);

        await _handler.Handle(updateCommand, CancellationToken.None);

        _mockLogRepository.Verify(
            logRepo =>
                logRepo.AddProjectLogForCurrentUser(
                    It.IsAny<Project>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
    }

    [Test]
    public async Task AlreadyExitingSlug_TestAsync()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Example Project",
            Slug = "example project",
            ClientName = "Example Client",
            OfferId = "Offer A",
            Company = "Company A",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
            ProjectPlugins = [],
            Notes = "Example Notes",
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: "New Project",
            ClientName: project.ClientName,
            OfferId: project.OfferId,
            Company: project.Company,
            CompanyState: project.CompanyState,
            IsmsLevel: project.IsmsLevel,
            Id: project.Id,
            Plugins: [],
            Notes: project.Notes,
            IsArchived: false,
            TeamId: null // Assuming TeamId can be null
        );

        _mockProjectRepo.Setup(m => m.GetProjectWithPluginsAsync(1)).ReturnsAsync(project);

        var result = await _handler.Handle(updateCommand, CancellationToken.None);
        Assert.That(result, Is.EqualTo(project.Id));
    }

    [Test]
    public void ProjectNotesToLong_Test()
    {
        var project = new Project
        {
            Id = 1,
            ProjectName = "Example Project",
            Slug = "example project",
            ClientName = "Example Client",
            OfferId = "Offer A",
            Company = "Company A",
            CompanyState = CompanyState.EXTERNAL,
            IsmsLevel = SecurityLevel.VERY_HIGH,
            ProjectPlugins = [],
            Notes = "Example Notes",
        };

        var updateCommand = new UpdateProjectCommand(
            ProjectName: "New Project",
            ClientName: project.ClientName,
            OfferId: project.OfferId,
            Company: project.Company,
            CompanyState: project.CompanyState,
            IsmsLevel: project.IsmsLevel,
            Id: project.Id,
            Plugins: [],
            IsArchived: false,
            Notes: new string('a', 501),
            TeamId: null // Assuming TeamId can be null
        );

        _mockProjectRepo.Setup(m => m.GetProjectWithPluginsAsync(1)).ReturnsAsync(project);
        _mockSlugHelper.Setup(m => m.GenerateSlug(It.IsAny<string>())).Returns("new project");
        _mockSlugHelper.Setup(m => m.CheckProjectSlugExists("new project")).ReturnsAsync(false);

        var ex = Assert.ThrowsAsync<ProjectNotesSizeException>(async () =>
        {
            await _handler.Handle(updateCommand, CancellationToken.None);
        });

        Assert.That(
            ex.Message,
            Is.EqualTo("The project notes are 501 chars long. Maximum allowed is 500 chars.")
        );
    }
}
