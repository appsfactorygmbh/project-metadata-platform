using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Users;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class GetUserByEmailQueryHandlerTest
{
    private GetUserByEmailQueryHandler _handler;
    private Mock<IUsersRepository> _mockUserRepo;

    [SetUp]
    public void Setup()
    {
        _mockUserRepo = new Mock<IUsersRepository>();
        _handler = new GetUserByEmailQueryHandler(_mockUserRepo.Object);
    }

    [Test]
    public async Task HandleGetUserByEmail_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "abc",
            Id = "13",
            Email = "squidlauncher@bankofevil.com",
            IsActive = true,
            IsScimProvisioned = false,
        };

        _mockUserRepo
            .Setup(m => m.GetUserByEmailAsync("squidlauncher@bankofevil.com"))
            .ReturnsAsync(user);

        var request = new GetUserByEmailQuery("squidlauncher@bankofevil.com");

        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ApplicationUser>());
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo("13"));
            Assert.That(result.Email, Is.EqualTo("squidlauncher@bankofevil.com"));
            Assert.That(result.EmployeeId, Is.EqualTo("abc"));
            Assert.That(result.IsActive, Is.EqualTo(true));
            Assert.That(result.IsScimProvisioned, Is.EqualTo(false));
        });
    }

    [Test]
    public async Task HandleGetUserByEmail_NotFound_Test()
    {
        _mockUserRepo
            .Setup(m => m.GetUserByEmailAsync("Vector"))
            .ReturnsAsync((ApplicationUser)null!);

        var request = new GetUserByEmailQuery("Vector");

        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Null);
    }
}
