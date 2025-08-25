using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Errors.UserException;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class PatchUserCommandHandlerTest
{
    private PatchUserCommandHandler _handler;
    private Mock<IUsersRepository> _mockUsersRepo;
    private Mock<IPasswordHasher<IdentityUser>> _mockPasswordHasher;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ILogRepository> _mockLogRepo;

    [SetUp]
    public void Setup()
    {
        _mockUsersRepo = new Mock<IUsersRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher<IdentityUser>>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogRepo = new Mock<ILogRepository>();
        _handler = new PatchUserCommandHandler(
            _mockUsersRepo.Object,
            _mockPasswordHasher.Object,
            _mockUnitOfWork.Object,
            _mockLogRepo.Object
        );
    }

    [Test]
    public async Task PatchUser_Test()
    {
        var user = new IdentityUser { Id = "42", Email = "candela@hip-hop.dancehall" };
        var newUser = new IdentityUser { Id = "42", Email = "angela@hip-hop.dancehall" };

        _mockUsersRepo.Setup(repo => repo.GetUserByIdAsync("42")).ReturnsAsync(user);
        _mockUsersRepo
            .Setup(repo => repo.StoreUser(It.IsAny<IdentityUser>()))
            .ReturnsAsync((IdentityUser p) => p);
        _mockUnitOfWork.Setup(m => m.CompleteAsync()).Returns(Task.CompletedTask);
        _mockLogRepo
            .Setup(m =>
                m.AddUserLogForCurrentUser(
                    It.IsAny<IdentityUser>(),
                    It.IsAny<Action>(),
                    It.IsAny<List<LogChange>>()
                )
            )
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            new PatchUserCommand("42", "angela@hip-hop.dancehall"),
            It.IsAny<CancellationToken>()
        );

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Email, Is.EqualTo(newUser.Email));
            Assert.That(result.Id, Is.EqualTo(newUser.Id));
        });
    }

    [Test]
    public async Task PatchUser_ChangeNothing_Test()
    {
        var user = new IdentityUser { Id = "42", Email = "cold@play.co.uk" };

        _mockUsersRepo.Setup(repo => repo.GetUserByIdAsync("42")).ReturnsAsync(user);
        _mockUsersRepo
            .Setup(repo => repo.StoreUser(It.IsAny<IdentityUser>()))
            .ReturnsAsync((IdentityUser p) => p);
        var result = await _handler.Handle(
            new PatchUserCommand("42"),
            It.IsAny<CancellationToken>()
        );

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.Id, Is.EqualTo(user.Id));
        });
    }

    [Test]
    public void PatchUser_NotFound_Test()
    {
        _mockUsersRepo
            .Setup(repo => repo.GetUserByIdAsync("42"))
            .ThrowsAsync(new UserNotFoundException("42"));

        Assert.ThrowsAsync<UserNotFoundException>(() =>
            _handler.Handle(new PatchUserCommand("42"), It.IsAny<CancellationToken>())
        );
    }

    [Test]
    public async Task PatchUser_LogTest()
    {
        var user = new IdentityUser { Id = "42", Email = "oldButGold@htwk.com" };
        var newEmail = "newAndShiny@htwk.com";

        _mockUsersRepo.Setup(repo => repo.GetUserByIdAsync("42")).ReturnsAsync(user);
        _mockUsersRepo
            .Setup(repo => repo.StoreUser(It.IsAny<IdentityUser>()))
            .ReturnsAsync((IdentityUser p) => p);

        await _handler.Handle(new PatchUserCommand("42", newEmail), It.IsAny<CancellationToken>());

        _mockLogRepo.Verify(
            m =>
                m.AddUserLogForCurrentUser(
                    It.Is<IdentityUser>(u => u.Email == newEmail),
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
        var user = new IdentityUser
        {
            Id = "42",
            Email = "hanSolo",
            PasswordHash = "oldPassword",
        };
        var newPassword = "newPassword";
        var newPasswordHash = "newPasswordHash";

        _mockUsersRepo.Setup(repo => repo.CheckPasswordFormat("newPassword")).ReturnsAsync(true);
        _mockUsersRepo.Setup(repo => repo.GetUserByIdAsync("42")).ReturnsAsync(user);
        _mockUsersRepo
            .Setup(repo => repo.StoreUser(It.IsAny<IdentityUser>()))
            .ReturnsAsync((IdentityUser p) => p);
        _mockPasswordHasher
            .Setup(ph => ph.HashPassword(user, newPassword))
            .Returns(newPasswordHash);

        await _handler.Handle(
            new PatchUserCommand("42", Password: newPassword),
            It.IsAny<CancellationToken>()
        );

        _mockLogRepo.Verify(
            m =>
                m.AddUserLogForCurrentUser(
                    It.Is<IdentityUser>(u => u.PasswordHash == newPasswordHash),
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

    [Test]
    public async Task PatchUser_EmailNotChanged_Test()
    {
        var user = new IdentityUser { Id = "42", Email = "oldButGold@htwk.com" };
        var newEmail = "oldButGold@htwk.com";

        _mockUsersRepo.Setup(repo => repo.GetUserByIdAsync("42")).ReturnsAsync(user);
        _mockUsersRepo
            .Setup(repo => repo.StoreUser(It.IsAny<IdentityUser>()))
            .ReturnsAsync((IdentityUser p) => p);

        await _handler.Handle(new PatchUserCommand("42", newEmail), It.IsAny<CancellationToken>());

        _mockLogRepo.Verify(
            m =>
                m.AddUserLogForCurrentUser(
                    It.IsAny<IdentityUser>(),
                    Action.UPDATED_USER,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
    }

    [Test]
    public async Task PatchUser_PasswordNotChanged_Test()
    {
        var user = new IdentityUser
        {
            Id = "42",
            Email = "never",
            PasswordHash = "password",
        };
        var newPassword = "password";

        _mockUsersRepo.Setup(repo => repo.GetUserByIdAsync("42")).ReturnsAsync(user);
        _mockUsersRepo
            .Setup(repo => repo.StoreUser(It.IsAny<IdentityUser>()))
            .ReturnsAsync((IdentityUser p) => p);
        _mockPasswordHasher
            .Setup(ph => ph.HashPassword(user, newPassword))
            .Returns(user.PasswordHash);

        await _handler.Handle(
            new PatchUserCommand("42", Password: newPassword),
            It.IsAny<CancellationToken>()
        );

        _mockLogRepo.Verify(
            m =>
                m.AddUserLogForCurrentUser(
                    It.IsAny<IdentityUser>(),
                    Action.UPDATED_USER,
                    It.IsAny<List<LogChange>>()
                ),
            Times.Never
        );
    }
}
