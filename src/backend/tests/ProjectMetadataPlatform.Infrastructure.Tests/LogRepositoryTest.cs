using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.BusinessUnits;
using ProjectMetadataPlatform.Domain.Companies;
using ProjectMetadataPlatform.Domain.Departments;
using ProjectMetadataPlatform.Domain.Errors.LogExceptions;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.OfficeLocations;
using ProjectMetadataPlatform.Domain.Plugins;
using ProjectMetadataPlatform.Domain.Projects;
using ProjectMetadataPlatform.Domain.Users;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Logs;
using Action = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class LogRepositoryTest : TestsWithDatabase
{
    private ProjectMetadataPlatformDbContext _context;
    private LogRepository _loggingRepository;
    private Mock<IUsersRepository> _mockUserRepository;

    private Mock<IApiTokenRepository> _mockApiTokenRepository;
    private Mock<IHttpContextAccessor> _httpContextAccessorMock;

    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.Email, "camo"),
                new Claim(ClaimTypes.AuthenticationMethod, "JWT Token"),
            },
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity); //add claims as needed
        var httpContext = new DefaultHttpContext { User = contextUser };
        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(httpContext);

        _mockUserRepository = new Mock<IUsersRepository>();
        _mockApiTokenRepository = new Mock<IApiTokenRepository>();
        _loggingRepository = new LogRepository(
            _context,
            _httpContextAccessorMock.Object,
            _mockUserRepository.Object,
            _mockApiTokenRepository.Object
        );
    }

    [TearDown]
    public void TearDown()
    {
        ClearData(_context);
        _context?.Dispose();
    }

    [Test]
    public async Task CreateProjectLogTest()
    {
        var exampleProject = new Project
        {
            ProjectName = "Example Project",
            Slug = "example_project",
            ClientName = "Example Client",
            CompanyId = 1,
        };
        await _context.Projects.AddAsync(exampleProject);

        var user = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "42",
            Email = "camo",
            IsActive = true,
            IsScimProvisioned = false,
        };
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "",
                NewValue = "Example Project",
                Property = "ProjectName",
            },
            new()
            {
                OldValue = "",
                NewValue = "Example Client",
                Property = "ClientName",
            },
        };

        _mockUserRepository
            .Setup(repository => repository.GetUserByEmailAsync("camo"))
            .ReturnsAsync(user);

        await _loggingRepository.AddProjectLogForCurrentActor(
            exampleProject,
            Action.ADDED_PROJECT,
            logChanges
        );
        await _context.SaveChangesAsync();
        var dbLog = await _context
            .Logs.Include(log => log.Project)
            .Include(log => log.Changes)
            .Include(log => log.Author)
            .Include(log => log.Project)
            .Include(log => log.Changes)
            .FirstOrDefaultAsync();

        Assert.That(dbLog, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dbLog.Id, Is.EqualTo(1));
            Assert.That(dbLog.Action, Is.EqualTo(Action.ADDED_PROJECT));
            Assert.That(dbLog.AuthorName, Is.EqualTo("camo"));
            Assert.That(dbLog.AuthorId, Is.EqualTo("42"));
            Assert.That(dbLog.Author, Is.EqualTo(user));
            Assert.That(dbLog.ProjectId, Is.EqualTo(exampleProject.Id));
            Assert.That(dbLog.Project, Is.EqualTo(exampleProject));
            Assert.That(dbLog.Changes, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public async Task UpdateUserLogTest()
    {
        var author = new ApplicationUser
        {
            EmployeeId = "abc",
            Id = "42",
            Email = "camo",
            IsActive = true,
            IsScimProvisioned = false,
        };

        var affectedUser = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "12",
            Email = "gagarin@vostok.su",
            IsActive = true,
            IsScimProvisioned = false,
        };
        await _context.Users.AddAsync(author);
        await _context.Users.AddAsync(affectedUser);

        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "no",
                NewValue = "yes",
                Property = "isAstronaut",
            },
        };

        _mockUserRepository
            .Setup(repository => repository.GetUserByEmailAsync("camo"))
            .ReturnsAsync(author);

        await _loggingRepository.AddUserLogForCurrentActor(
            affectedUser,
            Action.UPDATED_USER,
            logChanges
        );
        await _context.SaveChangesAsync();
        var dbLog = await _context
            .Logs.Include(log => log.Changes)
            .Include(log => log.Author)
            .Include(log => log.Changes)
            .Include(log => log.AffectedUser)
            .FirstOrDefaultAsync();
        Assert.That(dbLog, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dbLog.Id, Is.EqualTo(1));
            Assert.That(dbLog.Action, Is.EqualTo(Action.UPDATED_USER));
            Assert.That(dbLog.AuthorName, Is.EqualTo("camo"));
            Assert.That(dbLog.AuthorId, Is.EqualTo("42"));
            Assert.That(dbLog.Author, Is.EqualTo(author));
            Assert.That(dbLog.AffectedUserId, Is.EqualTo("12"));
            Assert.That(dbLog.AffectedUser, Is.EqualTo(affectedUser));
            Assert.That(dbLog.Changes, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task UpdateGlobalPluginLogTest()
    {
        var author = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "42",
            Email = "camo",
            IsActive = true,
            IsScimProvisioned = false,
        };
        await _context.Users.AddAsync(author);

        var globalPlugin = new Plugin { PluginName = "Canadarm2", Id = 13 };
        await _context.Plugins.AddAsync(globalPlugin);

        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "in storage",
                NewValue = "installed",
                Property = "status",
            },
        };

        _mockUserRepository
            .Setup(repository => repository.GetUserByEmailAsync("camo"))
            .ReturnsAsync(author);

        await _loggingRepository.AddGlobalPluginLogForCurrentActor(
            globalPlugin,
            Action.UPDATED_GLOBAL_PLUGIN,
            logChanges
        );
        await _context.SaveChangesAsync();
        var dbLog = await _context
            .Logs.Include(log => log.GlobalPlugin)
            .Include(log => log.Changes)
            .Include(log => log.Author)
            .Include(log => log.Changes)
            .Include(log => log.GlobalPlugin)
            .FirstOrDefaultAsync();
        Assert.That(dbLog, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dbLog.Id, Is.EqualTo(1));
            Assert.That(dbLog.Action, Is.EqualTo(Action.UPDATED_GLOBAL_PLUGIN));
            Assert.That(dbLog.AuthorName, Is.EqualTo("camo"));
            Assert.That(dbLog.AuthorId, Is.EqualTo("42"));
            Assert.That(dbLog.Author, Is.EqualTo(author));
            Assert.That(dbLog.GlobalPluginId, Is.EqualTo(13));
            Assert.That(dbLog.GlobalPlugin, Is.EqualTo(globalPlugin));
            Assert.That(dbLog.GlobalPluginName, Is.EqualTo("Canadarm2"));
            Assert.That(dbLog.Changes, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task AddApiTokenLogTest()
    {
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.Name, "camo"),
                new Claim(ClaimTypes.AuthenticationMethod, "API Token"),
            },
            "TestAuth"
        );
        var contextUser = new ClaimsPrincipal(identity); //add claims as needed
        var httpContext = new DefaultHttpContext { User = contextUser };
        _httpContextAccessorMock.Setup(accessor => accessor.HttpContext).Returns(httpContext);
        var authorToken = new ApiToken
        {
            Id = 1,
            Name = "camo",
            Token = "TokenHash",
        };
        await _context.ApiTokens.AddAsync(authorToken);

        var token = new ApiToken
        {
            Id = 2,
            Name = "AnotherToken",
            Token = "TokenHash",
        };
        await _context.ApiTokens.AddAsync(token);

        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "in storage",
                NewValue = "installed",
                Property = "status",
            },
        };

        _mockApiTokenRepository
            .Setup(repository => repository.GetApiTokenByName("camo"))
            .ReturnsAsync(authorToken);

        await _loggingRepository.AddApiTokenLogForCurrentActor(
            token,
            Action.ADDED_API_TOKEN,
            logChanges
        );

        await _context.SaveChangesAsync();

        var dbLog = await _context
            .Logs.Include(log => log.AffectedToken)
            .Include(log => log.Changes)
            .Include(log => log.AuthorToken)
            .FirstOrDefaultAsync();
        Assert.That(dbLog, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dbLog.Id, Is.EqualTo(1));
            Assert.That(dbLog.Action, Is.EqualTo(Action.ADDED_API_TOKEN));
            Assert.That(dbLog.AuthorName, Is.EqualTo("camo"));
            Assert.That(dbLog.AuthorTokenId, Is.EqualTo(1));
            Assert.That(dbLog.AuthorToken, Is.EqualTo(authorToken));
            Assert.That(dbLog.AffectedTokenId, Is.EqualTo(2));
            Assert.That(dbLog.AffectedToken, Is.EqualTo(token));
            Assert.That(dbLog.Changes, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task RemoveApiTokenLogTest()
    {
        var author = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "42",
            Email = "camo",
            IsActive = true,
            IsScimProvisioned = false,
        };
        await _context.Users.AddAsync(author);

        var apiToken = new ApiToken
        {
            Id = 1,
            Name = "Token",
            Token = "TokenHash",
        };
        await _context.ApiTokens.AddAsync(apiToken);

        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "in storage",
                NewValue = "installed",
                Property = "status",
            },
        };

        _mockUserRepository
            .Setup(repository => repository.GetUserByEmailAsync("camo"))
            .ReturnsAsync(author);

        await _loggingRepository.AddApiTokenLogForCurrentActor(
            apiToken,
            Action.REMOVED_API_TOKEN,
            logChanges
        );
        await _context.SaveChangesAsync();
        var dbLog = await _context
            .Logs.Include(log => log.AffectedToken)
            .Include(log => log.Changes)
            .Include(log => log.Author)
            .FirstOrDefaultAsync();
        Assert.That(dbLog, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(dbLog.Id, Is.EqualTo(1));
            Assert.That(dbLog.Action, Is.EqualTo(Action.REMOVED_API_TOKEN));
            Assert.That(dbLog.AuthorName, Is.EqualTo("camo"));
            Assert.That(dbLog.AuthorId, Is.EqualTo("42"));
            Assert.That(dbLog.Author, Is.EqualTo(author));
            Assert.That(dbLog.AffectedTokenId, Is.EqualTo(1));
            Assert.That(dbLog.AffectedToken, Is.EqualTo(apiToken));
            Assert.That(dbLog.AffectedTokenName, Is.EqualTo("Token"));
            Assert.That(dbLog.Changes, Has.Count.EqualTo(1));
        });
    }

    [TestCase(Action.ADDED_USER)]
    [TestCase(Action.UPDATED_USER)]
    [TestCase(Action.REMOVED_USER)]
    [TestCase(Action.ADDED_GLOBAL_PLUGIN)]
    [TestCase(Action.UPDATED_GLOBAL_PLUGIN)]
    [TestCase(Action.ARCHIVED_GLOBAL_PLUGIN)]
    [TestCase(Action.UNARCHIVED_GLOBAL_PLUGIN)]
    [TestCase(Action.REMOVED_GLOBAL_PLUGIN)]
    public async Task ProjectLogTest_RejectsActionNotInWhitelist(Action action)
    {
        var exampleProject = new Project
        {
            ProjectName = "Example Project",
            Slug = "example_project",
            ClientName = "Example Client",
            CompanyId = 1,
        };
        await _context.Projects.AddAsync(exampleProject);

        var user = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "42",
            Email = "camo",
            IsActive = true,
            IsScimProvisioned = false,
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "",
                NewValue = "Example Project",
                Property = "ProjectName",
            },
        };

        Assert.ThrowsAsync<LogActionNotSupportedException>(() =>
            _loggingRepository.AddProjectLogForCurrentActor(exampleProject, action, logChanges)
        );
    }

    [TestCase(Action.ADDED_PROJECT)]
    [TestCase(Action.ADDED_PROJECT_PLUGIN)]
    [TestCase(Action.UPDATED_PROJECT)]
    [TestCase(Action.UPDATED_PROJECT_PLUGIN)]
    [TestCase(Action.REMOVED_PROJECT_PLUGIN)]
    [TestCase(Action.ARCHIVED_PROJECT)]
    [TestCase(Action.UNARCHIVED_PROJECT)]
    [TestCase(Action.REMOVED_PROJECT)]
    [TestCase(Action.ADDED_GLOBAL_PLUGIN)]
    [TestCase(Action.UPDATED_GLOBAL_PLUGIN)]
    [TestCase(Action.ARCHIVED_GLOBAL_PLUGIN)]
    [TestCase(Action.UNARCHIVED_GLOBAL_PLUGIN)]
    [TestCase(Action.REMOVED_GLOBAL_PLUGIN)]
    public async Task UserLogTest_RejectsActionNotInWhitelist(Action action)
    {
        var author = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "42",
            Email = "camo",
            IsActive = true,
            IsScimProvisioned = false,
        };

        var affectedUser = new ApplicationUser
        {
            EmployeeId = "125",
            Id = "12",
            Email = "gagarin@vostok.su",
            IsActive = true,
            IsScimProvisioned = false,
        };
        await _context.Users.AddAsync(author);
        await _context.Users.AddAsync(affectedUser);

        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "no",
                NewValue = "yes",
                Property = "isAstronaut",
            },
        };

        Assert.ThrowsAsync<LogActionNotSupportedException>(() =>
            _loggingRepository.AddUserLogForCurrentActor(affectedUser, action, logChanges)
        );
    }

    [TestCase(Action.ADDED_USER)]
    [TestCase(Action.UPDATED_USER)]
    [TestCase(Action.REMOVED_USER)]
    [TestCase(Action.ADDED_PROJECT)]
    [TestCase(Action.ADDED_PROJECT_PLUGIN)]
    [TestCase(Action.UPDATED_PROJECT)]
    [TestCase(Action.UPDATED_PROJECT_PLUGIN)]
    [TestCase(Action.REMOVED_PROJECT_PLUGIN)]
    [TestCase(Action.ARCHIVED_PROJECT)]
    [TestCase(Action.UNARCHIVED_PROJECT)]
    [TestCase(Action.REMOVED_PROJECT)]
    public async Task GlobalPluginLogTest_RejectsActionNotInWhitelist(Action action)
    {
        var author = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "42",
            Email = "camo",
            IsActive = true,
            IsScimProvisioned = false,
        };
        await _context.Users.AddAsync(author);

        var globalPlugin = new Plugin { PluginName = "Canadarm2", Id = 13 };
        await _context.Plugins.AddAsync(globalPlugin);

        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "in storage",
                NewValue = "installed",
                Property = "status",
            },
        };

        Assert.ThrowsAsync<LogActionNotSupportedException>(() =>
            _loggingRepository.AddGlobalPluginLogForCurrentActor(globalPlugin, action, logChanges)
        );
    }

    [TestCase(Action.ADDED_USER)]
    [TestCase(Action.UPDATED_USER)]
    [TestCase(Action.REMOVED_USER)]
    [TestCase(Action.ADDED_PROJECT)]
    [TestCase(Action.ADDED_PROJECT_PLUGIN)]
    [TestCase(Action.UPDATED_PROJECT)]
    [TestCase(Action.UPDATED_PROJECT_PLUGIN)]
    [TestCase(Action.REMOVED_PROJECT_PLUGIN)]
    [TestCase(Action.ARCHIVED_PROJECT)]
    [TestCase(Action.UNARCHIVED_PROJECT)]
    [TestCase(Action.REMOVED_PROJECT)]
    public async Task ApiTokenLogTest_RejectsActionNotInWhitelist(Action action)
    {
        var author = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "42",
            Email = "camo",
            IsActive = true,
            IsScimProvisioned = false,
        };
        await _context.Users.AddAsync(author);

        var apiToken = new ApiToken { Name = "Token", Token = "Value" };
        await _context.ApiTokens.AddAsync(apiToken);

        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "in storage",
                NewValue = "installed",
                Property = "status",
            },
        };

        Assert.ThrowsAsync<LogActionNotSupportedException>(() =>
            _loggingRepository.AddApiTokenLogForCurrentActor(apiToken, action, logChanges)
        );
    }

    [TestCase(Action.ADDED_USER)]
    [TestCase(Action.UPDATED_USER)]
    [TestCase(Action.REMOVED_USER)]
    [TestCase(Action.ADDED_PROJECT)]
    [TestCase(Action.ADDED_PROJECT_PLUGIN)]
    [TestCase(Action.UPDATED_PROJECT)]
    [TestCase(Action.UPDATED_PROJECT_PLUGIN)]
    [TestCase(Action.REMOVED_PROJECT_PLUGIN)]
    [TestCase(Action.ARCHIVED_PROJECT)]
    [TestCase(Action.UNARCHIVED_PROJECT)]
    [TestCase(Action.REMOVED_PROJECT)]
    public async Task CompanyLogTest_RejectsActionNotInWhitelist(Action action)
    {
        var author = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "42",
            Email = "camo",
            IsActive = true,
            IsScimProvisioned = false,
        };
        await _context.Users.AddAsync(author);

        var company = new Company { CompanyName = "Test" };
        await _context.Companies.AddAsync(company);

        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "in storage",
                NewValue = "installed",
                Property = "status",
            },
        };

        Assert.ThrowsAsync<LogActionNotSupportedException>(() =>
            _loggingRepository.AddCompanyLogForCurrentActor(company, action, logChanges)
        );
    }

    [TestCase(Action.ADDED_USER)]
    [TestCase(Action.UPDATED_USER)]
    [TestCase(Action.REMOVED_USER)]
    [TestCase(Action.ADDED_PROJECT)]
    [TestCase(Action.ADDED_PROJECT_PLUGIN)]
    [TestCase(Action.UPDATED_PROJECT)]
    [TestCase(Action.UPDATED_PROJECT_PLUGIN)]
    [TestCase(Action.REMOVED_PROJECT_PLUGIN)]
    [TestCase(Action.ARCHIVED_PROJECT)]
    [TestCase(Action.UNARCHIVED_PROJECT)]
    [TestCase(Action.REMOVED_PROJECT)]
    public async Task DepartmentLogTest_RejectsActionNotInWhitelist(Action action)
    {
        var author = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "42",
            Email = "camo",
            IsActive = true,
            IsScimProvisioned = false,
        };
        await _context.Users.AddAsync(author);

        var department = new Department { DepartmentName = "Test" };
        await _context.Departments.AddAsync(department);

        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "in storage",
                NewValue = "installed",
                Property = "status",
            },
        };

        Assert.ThrowsAsync<LogActionNotSupportedException>(() =>
            _loggingRepository.AddDepartmentLogForCurrentActor(department, action, logChanges)
        );
    }

    [TestCase(Action.ADDED_USER)]
    [TestCase(Action.UPDATED_USER)]
    [TestCase(Action.REMOVED_USER)]
    [TestCase(Action.ADDED_PROJECT)]
    [TestCase(Action.ADDED_PROJECT_PLUGIN)]
    [TestCase(Action.UPDATED_PROJECT)]
    [TestCase(Action.UPDATED_PROJECT_PLUGIN)]
    [TestCase(Action.REMOVED_PROJECT_PLUGIN)]
    [TestCase(Action.ARCHIVED_PROJECT)]
    [TestCase(Action.UNARCHIVED_PROJECT)]
    [TestCase(Action.REMOVED_PROJECT)]
    public async Task OfficeLocationLogTest_RejectsActionNotInWhitelist(Action action)
    {
        var author = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "42",
            Email = "camo",
            IsActive = true,
            IsScimProvisioned = false,
        };
        await _context.Users.AddAsync(author);

        var officeLocation = new OfficeLocation { OfficeLocationName = "Test" };
        await _context.OfficeLocations.AddAsync(officeLocation);

        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "in storage",
                NewValue = "installed",
                Property = "status",
            },
        };

        Assert.ThrowsAsync<LogActionNotSupportedException>(() =>
            _loggingRepository.AddOfficeLocationLogForCurrentActor(
                officeLocation,
                action,
                logChanges
            )
        );
    }

    [TestCase(Action.ADDED_USER)]
    [TestCase(Action.UPDATED_USER)]
    [TestCase(Action.REMOVED_USER)]
    [TestCase(Action.ADDED_PROJECT)]
    [TestCase(Action.ADDED_PROJECT_PLUGIN)]
    [TestCase(Action.UPDATED_PROJECT)]
    [TestCase(Action.UPDATED_PROJECT_PLUGIN)]
    [TestCase(Action.REMOVED_PROJECT_PLUGIN)]
    [TestCase(Action.ARCHIVED_PROJECT)]
    [TestCase(Action.UNARCHIVED_PROJECT)]
    [TestCase(Action.REMOVED_PROJECT)]
    public async Task BusinessUnitLogTest_RejectsActionNotInWhitelist(Action action)
    {
        var author = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "42",
            Email = "camo",
            IsActive = true,
            IsScimProvisioned = false,
        };
        await _context.Users.AddAsync(author);

        var businessUnit = new BusinessUnit { BusinessUnitName = "Test" };
        await _context.BusinessUnits.AddAsync(businessUnit);

        await _context.SaveChangesAsync();

        var logChanges = new List<LogChange>
        {
            new()
            {
                OldValue = "in storage",
                NewValue = "installed",
                Property = "status",
            },
        };

        Assert.ThrowsAsync<LogActionNotSupportedException>(() =>
            _loggingRepository.AddBusinessUnitLogForCurrentActor(businessUnit, action, logChanges)
        );
    }

    [Test]
    public async Task GetLogsForProject_Test()
    {
        var exampleLog = new Log
        {
            Id = 1,
            AuthorId = null,
            AuthorName = "camo",
            AuthorTokenId = null,
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 301,
            Action = Action.ADDED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    OldValue = "",
                    NewValue = "Example Project",
                    Property = "ProjectName",
                },
            ],
        };

        var exampleProject = new Project
        {
            ProjectName = "Example Project",
            Slug = "example_project",
            ClientName = "Example Client",
            Logs = [exampleLog],
            CompanyId = 1,
        };
        exampleLog.Project = exampleProject;

        await _context.Projects.AddAsync(exampleProject);
        await _context.SaveChangesAsync();

        var logs = await _loggingRepository.GetLogsForProject(301);

        Assert.That(logs, Has.Count.EqualTo(1));
        Assert.That(logs[0], Is.EqualTo(exampleLog));

        var log = logs[0];
        Assert.Multiple(() =>
        {
            Assert.That(log.Id, Is.EqualTo(1));
            Assert.That(log.AuthorId, Is.Null);
            Assert.That(log.AuthorName, Is.EqualTo("camo"));
            Assert.That(log.ProjectId, Is.EqualTo(301));
            Assert.That(log.Action, Is.EqualTo(Action.ADDED_PROJECT));
            Assert.That(log.Changes, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task GetLogsForProject_Test_NoLogsForProjectReturnsEmptyList()
    {
        var exampleProject = new Project
        {
            ProjectName = "Example Project",
            Slug = "example_project",
            ClientName = "Example Client",
            Logs = null,
            CompanyId = 1,
        };
        await _context.Projects.AddAsync(exampleProject);
        await _context.SaveChangesAsync();

        var logs = await _loggingRepository.GetLogsForProject(301);

        Assert.That(logs, Has.Count.EqualTo(0));
    }

    [Test]
    public async Task GetAllLogs_Test()
    {
        var exampleLog1 = new Log
        {
            Id = 1,
            AuthorId = null,
            AuthorTokenId = null,
            AuthorName = "camo",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 301,
            Action = Action.ADDED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    OldValue = "",
                    NewValue = "Example Project",
                    Property = "ProjectName",
                },
            ],
        };

        var exampleProject1 = new Project
        {
            ProjectName = "Example Project",
            Slug = "example_project",
            ClientName = "Example Client",
            Logs = [exampleLog1],
            CompanyId = 1,
        };

        var exampleLog2 = new Log
        {
            Id = 2,
            AuthorId = null,
            AuthorTokenId = null,
            AuthorName = "someUserName",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 302,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    OldValue = "Example Project",
                    NewValue = "Another Project",
                    Property = "ProjectName",
                },
            ],
        };

        var exampleProject2 = new Project
        {
            ProjectName = "Another Project",
            Slug = "another_project",
            ClientName = "Example Client",
            Logs = [exampleLog2],
            CompanyId = 1,
        };
        await _context.Projects.AddAsync(exampleProject1);
        await _context.Projects.AddAsync(exampleProject2);
        await _context.SaveChangesAsync();

        var logs = await _loggingRepository.GetAllLogs();

        Assert.That(logs, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetLogsWithSearch_Test()
    {
        var exampleLog1 = new Log
        {
            Id = 1,
            AuthorId = null,
            AuthorTokenId = null,
            AuthorName = "camo",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 301,
            Action = Action.ADDED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    OldValue = "",
                    NewValue = "Example Project",
                    Property = "ProjectName",
                },
            ],
        };

        var exampleProject1 = new Project
        {
            ProjectName = "Example Project",
            Slug = "example_project",
            ClientName = "Example Client",
            Logs = [exampleLog1],
            CompanyId = 1,
        };

        var exampleLog2 = new Log
        {
            Id = 2,
            AuthorId = null,
            AuthorTokenId = null,
            AuthorName = "someUserName",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 302,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    OldValue = "Example Project",
                    NewValue = "Another Project",
                    Property = "ProjectName",
                },
            ],
        };

        var exampleProject2 = new Project
        {
            ProjectName = "Another Project",
            Slug = "another_project",
            ClientName = "Example Client",
            Logs = [exampleLog2],
            CompanyId = 1,
        };
        await _context.Projects.AddAsync(exampleProject1);
        await _context.Projects.AddAsync(exampleProject2);
        await _context.SaveChangesAsync();

        var logs = await _loggingRepository.GetLogsWithSearch("Another Project");

        Assert.That(logs, Has.Count.EqualTo(1));

        var log = logs[0];
        Assert.Multiple(() =>
        {
            Assert.That(log.Id, Is.EqualTo(2));
            Assert.That(log.AuthorId, Is.Null);
            Assert.That(log.AuthorName, Is.EqualTo("someUserName"));
            Assert.That(log.ProjectId, Is.EqualTo(302));
            Assert.That(log.Action, Is.EqualTo(Action.UPDATED_PROJECT));
            Assert.That(log.Changes, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task GetLogsWithSearch_SearchInStaticMessagePartMapsToActionProperty_Test()
    {
        var exampleLog1 = new Log
        {
            Id = 1,
            AuthorId = null,
            AuthorTokenId = null,
            AuthorName = "camo",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 301,
            Action = Action.ADDED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    OldValue = "",
                    NewValue = "Example Project",
                    Property = "ProjectName",
                },
            ],
        };

        var exampleProject1 = new Project
        {
            ProjectName = "Example Project",
            Slug = "example_project",
            ClientName = "Example Client",
            Logs = [exampleLog1],
            CompanyId = 1,
        };

        var exampleLog2 = new Log
        {
            Id = 2,
            AuthorId = null,
            AuthorTokenId = null,
            AuthorName = "someUserName",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = 302,
            Action = Action.UPDATED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    OldValue = "Example Project",
                    NewValue = "Another Project",
                    Property = "ProjectName",
                },
            ],
        };

        var exampleProject2 = new Project
        {
            ProjectName = "Another Project",
            Slug = "another_project",
            ClientName = "Example Client",
            Logs = [exampleLog2],
            CompanyId = 1,
        };
        await _context.Projects.AddAsync(exampleProject1);
        await _context.Projects.AddAsync(exampleProject2);
        await _context.SaveChangesAsync();

        var logs = await _loggingRepository.GetLogsWithSearch("updated");

        Assert.That(logs, Has.Count.EqualTo(1));

        var log = logs[0];
        Assert.Multiple(() =>
        {
            Assert.That(log.Id, Is.EqualTo(2));
            Assert.That(log.AuthorId, Is.Null);
            Assert.That(log.AuthorName, Is.EqualTo("someUserName"));
            Assert.That(log.ProjectId, Is.EqualTo(302));
            Assert.That(log.Action, Is.EqualTo(Action.UPDATED_PROJECT));
            Assert.That(log.Changes, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task GetLogsForAffectedUser_Test()
    {
        var exampleLog1 = new Log
        {
            Id = 1,
            AuthorId = null,
            AuthorName = "camo",
            AuthorTokenId = null,
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = null,
            Action = Action.REMOVED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    OldValue = "",
                    NewValue = "Example Project",
                    Property = "ProjectName",
                },
            ],
        };

        var exampleUser = new ApplicationUser
        {
            Id = "Newton",
            EmployeeId = "123",
            Email = "newton@royalastronomicalsociety.co.uk",
            IsActive = true,
            IsScimProvisioned = false,
        };

        var exampleLog2 = new Log
        {
            Id = 2,
            AuthorId = null,
            AuthorTokenId = null,
            AuthorName = "someUserName",
            TimeStamp = DateTimeOffset.UtcNow,
            Action = Action.UPDATED_USER,
            AffectedUserId = "Newton",
            AffectedUserEmail = "Newton@royalastronomicalsociety.co.uk",
            AffectedUser = exampleUser,
            Changes =
            [
                new LogChange
                {
                    OldValue = "yes",
                    NewValue = "no",
                    Property = "flying",
                },
            ],
        };

        await _context.Users.AddAsync(exampleUser);
        await _context.Logs.AddAsync(exampleLog1);
        await _context.Logs.AddAsync(exampleLog2);
        await _context.SaveChangesAsync();

        var logs = await _loggingRepository.GetLogsForUser("Newton");

        Assert.That(logs, Has.Count.EqualTo(1));

        var log = logs[0];
        Assert.Multiple(() =>
        {
            Assert.That(log.Id, Is.EqualTo(2));
            Assert.That(log.AuthorId, Is.Null);
            Assert.That(log.AuthorName, Is.EqualTo("someUserName"));
            Assert.That(log.Action, Is.EqualTo(Action.UPDATED_USER));
            Assert.That(log.AffectedUserEmail, Is.EqualTo("Newton@royalastronomicalsociety.co.uk"));
            Assert.That(log.AffectedUserId, Is.EqualTo("Newton"));
            Assert.That(log.AffectedUser, Is.EqualTo(exampleUser));
            Assert.That(log.AffectedUserId, Is.EqualTo("Newton"));
            Assert.That(log.Changes, Has.Count.EqualTo(1));
        });
    }

    [Test]
    public async Task GetLogsForGlobalPlugin_Test()
    {
        var exampleLog1 = new Log
        {
            Id = 1,
            AuthorId = null,
            AuthorTokenId = null,
            AuthorName = "camo",
            TimeStamp = DateTimeOffset.UtcNow,
            ProjectId = null,
            Action = Action.REMOVED_PROJECT,
            Changes =
            [
                new LogChange
                {
                    OldValue = "",
                    NewValue = "Example Project",
                    Property = "ProjectName",
                },
            ],
        };

        var examplePlugin = new Plugin { Id = 42, PluginName = "Gravity" };

        var exampleLog2 = new Log
        {
            Id = 2,
            AuthorId = null,
            AuthorName = "someUserName",
            AuthorTokenId = null,
            TimeStamp = DateTimeOffset.UtcNow,
            Action = Action.UPDATED_GLOBAL_PLUGIN,
            GlobalPluginId = 42,
            GlobalPluginName = "Gravity",
            GlobalPlugin = examplePlugin,
            Changes =
            [
                new LogChange
                {
                    OldValue = "no",
                    NewValue = "yes",
                    Property = "discovered",
                },
            ],
        };

        await _context.Plugins.AddAsync(examplePlugin);
        await _context.Logs.AddAsync(exampleLog1);
        await _context.Logs.AddAsync(exampleLog2);
        await _context.SaveChangesAsync();

        var logs = await _loggingRepository.GetLogsForGlobalPlugin(42);

        Assert.That(logs, Has.Count.EqualTo(1));

        var log = logs[0];
        Assert.Multiple(() =>
        {
            Assert.That(log.Id, Is.EqualTo(2));
            Assert.That(log.AuthorId, Is.Null);
            Assert.That(log.AuthorName, Is.EqualTo("someUserName"));
            Assert.That(log.Action, Is.EqualTo(Action.UPDATED_GLOBAL_PLUGIN));
            Assert.That(log.GlobalPluginId, Is.EqualTo(42));
            Assert.That(log.GlobalPluginName, Is.EqualTo("Gravity"));
            Assert.That(log.GlobalPlugin, Is.EqualTo(examplePlugin));
            Assert.That(log.Changes, Has.Count.EqualTo(1));
        });
    }
}
