using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Errors.UserException;
using ProjectMetadataPlatform.Domain.Logs;
using UserAction = ProjectMetadataPlatform.Domain.Logs.Action;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class DeleteUserCommandHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockUsersRepo = new Mock<IUsersRepository>();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Email, "camo")], "TestAuth");
        var contextUser = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = contextUser };
        httpContextAccessorMock
            .Setup(contextAccessor => contextAccessor.HttpContext)
            .Returns(httpContext);
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogRepository = new Mock<ILogRepository>();
        _mockLogRepository
            .Setup(repository => repository.GetLogsWithSearch(It.IsAny<string>()))
            .ReturnsAsync([]);
        _handler = new DeleteUserCommandHandler(
            _mockUsersRepo.Object,
            httpContextAccessorMock.Object,
            _mockLogRepository.Object,
            _mockUnitOfWork.Object
        );
        httpContextAccessorMock
            .Setup(contextAccessor => contextAccessor.HttpContext.User)
            .Returns(contextUser);
    }

    private DeleteUserCommandHandler _handler;
    private Mock<IUsersRepository> _mockUsersRepo;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ILogRepository> _mockLogRepository;

    [Test]
    public async Task DeleteUser_Test()
    {
        var user = new IdentityUser { Id = "1", Email = "user@example.com" };
        _mockUsersRepo.Setup(m => m.GetUserByIdAsync("1")).ReturnsAsync(user);
        _mockUsersRepo.Setup(m => m.DeleteUserAsync(user)).ReturnsAsync(user);
        var result = await _handler.Handle(new DeleteUserCommand("1"), CancellationToken.None);
        _mockLogRepository.Verify(
            m =>
                m.AddUserLogForCurrentUser(
                    It.Is<IdentityUser>(u => u.Id == "1"),
                    UserAction.REMOVED_USER,
                    It.Is<List<LogChange>>(changes =>
                        changes.Count == 1
                        && changes[0].OldValue == "user@example.com"
                        && changes[0].NewValue == ""
                        && changes[0].Property == nameof(IdentityUser.Email)
                    )
                ),
            Times.Once
        );
        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public void DeleteUser_InvalidUser_Test()
    {
        _mockUsersRepo
            .Setup(m => m.GetUserByIdAsync("1"))
            .ThrowsAsync(new UserNotFoundException("1"));

        Assert.ThrowsAsync<UserNotFoundException>(() =>
            _handler.Handle(new DeleteUserCommand("1"), CancellationToken.None)
        );
    }

    [Test]
    public void DeleteUser_SelfDeletionAttempt_Test()
    {
        var user = new IdentityUser { Email = "camo", Id = "1" };

        _mockUsersRepo.Setup(m => m.GetUserByIdAsync("1")).ReturnsAsync(user);
        _mockUsersRepo.Setup(m => m.GetUserByEmailAsync("camo")).ReturnsAsync(user);

        Assert.ThrowsAsync<UserCantDeleteThemselfException>(() =>
            _handler.Handle(new DeleteUserCommand("1"), CancellationToken.None)
        );
    }
}
