using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Errors.UserException;
using ProjectMetadataPlatform.Domain.Logs;
using ProjectMetadataPlatform.Domain.Teams;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class PatchUserCommandHandlerTest
{
    private PatchUserCommandHandler _handler;
    private Mock<IUsersRepository> _mockUsersRepo;
    private Mock<IPasswordHasher<ApplicationUser>> _mockPasswordHasher;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ILogRepository> _mockLogRepo;
    private Mock<ITeamRepository> _mockTeamRepo;

    [SetUp]
    public void Setup()
    {
        _mockUsersRepo = new Mock<IUsersRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher<ApplicationUser>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogRepo = new Mock<ILogRepository>();
        _mockTeamRepo = new Mock<ITeamRepository>();
        _handler = new PatchUserCommandHandler(
            _mockUsersRepo.Object,
            _mockPasswordHasher.Object,
            _mockTeamRepo.Object,
            _mockUnitOfWork.Object,
            _mockLogRepo.Object
        );
    }

    [Test]
    public async Task PatchUser_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "42",
            Email = "candela@hip-hop.dancehall",
            IsActive = true,
            IsScimProvisioned = false,
            Teams = new HashSet<Team>
            {
                new Team { TeamName = "Team1", BusinessUnit = "BU" },
            },
        };
        _mockUsersRepo.Setup(m => m.CheckUserExists(It.IsAny<string>())).ReturnsAsync(false);
        _mockUsersRepo.Setup(repo => repo.GetUserByEmailAsync("123")).ReturnsAsync(user);
        _mockUsersRepo
            .Setup(repo => repo.StoreUser(It.IsAny<ApplicationUser>()))
            .ReturnsAsync((ApplicationUser p) => p);
        _mockUnitOfWork.Setup(m => m.CompleteAsync()).Returns(Task.CompletedTask);
        _mockLogRepo
            .Setup(m =>
                m.AddUserLogForCurrentActor(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                )
            )
            .Returns(Task.CompletedTask);
        _mockTeamRepo
            .Setup(m => m.GetTeamByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new Team { TeamName = "Team1", BusinessUnit = "BU" });

        var operations = new List<PatchUserCommand.OperationRecord>
        {
            new()
            {
                Path = "externalId",
                Operation = PatchOperations.Replace,
                Value = JsonDocument.Parse("\"hello world\"").RootElement,
            },
            new()
            {
                Path = "id",
                Operation = PatchOperations.Replace,
                Value = JsonDocument.Parse("\"122\"").RootElement,
            },
            new()
            {
                Path = "userName",
                Operation = PatchOperations.Replace,
                Value = JsonDocument.Parse("\"angela@hip-hop.dancehall\"").RootElement,
            },
            new()
            {
                Path = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:organization",
                Operation = PatchOperations.Add,
                Value = JsonDocument.Parse("\"AiFactory\"").RootElement,
            },
            new()
            {
                Path = "urn:ietf:params:scim:schemas:extension:pmp:User:departments",
                Operation = PatchOperations.Add,
                Value = JsonDocument.Parse("[\"Design\",\"QA\"]").RootElement,
            },
            new()
            {
                Path = "urn:ietf:params:scim:schemas:extension:pmp:User:teamSupport",
                Operation = PatchOperations.Add,
                Value = JsonDocument.Parse("[{\"value\": \"Team1\"}]").RootElement,
            },
            new()
            {
                Path = "urn:ietf:params:scim:schemas:extension:pmp:User:jobTitles",
                Operation = PatchOperations.Add,
                Value = JsonDocument.Parse("[]").RootElement,
            },
            new()
            {
                Path = "urn:ietf:params:scim:schemas:extension:pmp:User:team",
                Operation = PatchOperations.Remove,
            },
            new()
            {
                Path = "urn:ietf:params:scim:schemas:extension:pmp:User:businessUnits",
                Operation = PatchOperations.Add,
                Value = JsonDocument.Parse("[\"Health\"]").RootElement,
            },
            new()
            {
                Path = "active",
                Operation = PatchOperations.Replace,
                Value = JsonDocument.Parse("false").RootElement,
            },
        };
        var result = await _handler.Handle(
            new PatchUserCommand { Id = "123", Operations = operations },
            It.IsAny<CancellationToken>()
        );

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Email, Is.EqualTo("angela@hip-hop.dancehall"));
            Assert.That(result.EmployeeId, Is.EqualTo("122"));
            Assert.That(result.IsActive, Is.EqualTo(false));
            Assert.That(result.Teams, Is.EqualTo(new List<Team> { }));
            Assert.That(result.TeamSupport?.FirstOrDefault()?.TeamName, Is.EqualTo("Team1"));
            Assert.That(result.BusinessUnits, Is.EqualTo(new List<string> { "Health" }));
            Assert.That(result.JobTitles, Is.EqualTo(new List<string> { }));
            Assert.That(result.Departments, Is.EqualTo(new List<string> { "Design", "QA" }));
            Assert.That(result.Company, Is.EqualTo("AiFactory"));
        });
    }

    [Test]
    public async Task PatchUser_ChangeNothing_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "123",
            Id = "42",
            Email = "cold@play.co.uk",
            IsActive = true,
            IsScimProvisioned = false,
        };

        _mockUsersRepo.Setup(m => m.CheckUserExists(It.IsAny<string>())).ReturnsAsync(true);
        _mockUsersRepo.Setup(repo => repo.GetUserByIdAsync("123")).ReturnsAsync(user);
        _mockUsersRepo
            .Setup(repo => repo.StoreUser(It.IsAny<ApplicationUser>()))
            .ReturnsAsync((ApplicationUser p) => p);
        var result = await _handler.Handle(
            new PatchUserCommand { Id = "123", Operations = [] },
            It.IsAny<CancellationToken>()
        );

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.Id, Is.EqualTo(user.Id));
            Assert.That(result.EmployeeId, Is.EqualTo(user.EmployeeId));
        });
    }

    [Test]
    public void PatchUser_NotFound_Test()
    {
        _mockUsersRepo.Setup(m => m.CheckUserExists(It.IsAny<string>())).ReturnsAsync(true);
        _mockUsersRepo
            .Setup(repo => repo.GetUserByIdAsync("42"))
            .ThrowsAsync(new UserNotFoundException("42"));

        Assert.ThrowsAsync<UserNotFoundException>(() =>
            _handler.Handle(new PatchUserCommand { Id = "42" }, It.IsAny<CancellationToken>())
        );
    }

    [Test]
    public async Task PatchUser_LogTest()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "42",
            Id = "42",
            Email = "oldButGold@htwk.com",
            IsActive = true,
            IsScimProvisioned = false,
        };
        var newEmail = "newAndShiny@htwk.com";

        _mockUsersRepo.Setup(m => m.CheckUserExists(It.IsAny<string>())).ReturnsAsync(true);
        _mockUsersRepo.Setup(repo => repo.GetUserByIdAsync("42")).ReturnsAsync(user);
        _mockUsersRepo
            .Setup(repo => repo.StoreUser(It.IsAny<ApplicationUser>()))
            .ReturnsAsync((ApplicationUser p) => p);

        await _handler.Handle(
            new PatchUserCommand
            {
                Id = "42",
                Operations =
                [
                    new PatchUserCommand.OperationRecord
                    {
                        Path = "userName",
                        Operation = PatchOperations.Replace,
                        Value = JsonDocument.Parse($"\"{newEmail}\"").RootElement,
                    },
                ],
            },
            It.IsAny<CancellationToken>()
        );

        _mockLogRepo.Verify(
            m =>
                m.AddUserLogForCurrentActor(
                    It.Is<ApplicationUser>(u => u.Email == newEmail),
                    Action.UPDATED_USER,
                    It.Is<List<LogChange>>(changes =>
                        changes.Any(change =>
                            change.Property == "Email"
                            && change.OldValue == "oldButGold@htwk.com"
                            && change.NewValue == newEmail
                        )
                    )
                ),
            Times.Once
        );
    }

    [Test]
    public async Task PatchUser_PasswordChangeLogTest()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "42",
            Id = "42",
            Email = "hanSolo",
            PasswordHash = "oldPassword",
            IsActive = true,
            IsScimProvisioned = false,
        };
        var newPassword = "newPassword";
        var newPasswordHash = "newPasswordHash";
        _mockUsersRepo.Setup(m => m.CheckUserExists(It.IsAny<string>())).ReturnsAsync(true);
        _mockUsersRepo.Setup(repo => repo.CheckPasswordFormat("newPassword")).ReturnsAsync(true);
        _mockUsersRepo.Setup(repo => repo.GetUserByIdAsync("42")).ReturnsAsync(user);
        _mockUsersRepo
            .Setup(repo => repo.StoreUser(It.IsAny<ApplicationUser>()))
            .ReturnsAsync((ApplicationUser p) => p);
        _mockPasswordHasher
            .Setup(ph => ph.HashPassword(user, newPassword))
            .Returns(newPasswordHash);

        await _handler.Handle(
            new PatchUserCommand
            {
                Id = "42",
                Operations =
                [
                    new PatchUserCommand.OperationRecord
                    {
                        Path = "password",
                        Operation = PatchOperations.Replace,
                        Value = JsonDocument.Parse($"\"{newPassword}\"").RootElement,
                    },
                ],
            },
            It.IsAny<CancellationToken>()
        );

        _mockLogRepo.Verify(
            m =>
                m.AddUserLogForCurrentActor(
                    It.Is<ApplicationUser>(u => u.PasswordHash == newPasswordHash),
                    Action.UPDATED_USER,
                    It.Is<List<LogChange>>(changes =>
                        changes.Any(change =>
                            change.Property == "PasswordHash"
                            && change.OldValue == "old password was changed"
                            && change.NewValue == "new password *****"
                        )
                    )
                ),
            Times.Once
        );
    }
}
