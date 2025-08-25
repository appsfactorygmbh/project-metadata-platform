using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;

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
        var user = new IdentityUser { Id = "13", Email = "squidlauncher@bankofevil.com" };

        _mockUserRepo
            .Setup(m => m.GetUserByEmailAsync("squidlauncher@bankofevil.com"))
            .ReturnsAsync(user);

        var request = new GetUserByEmailQuery("squidlauncher@bankofevil.com");

        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IdentityUser>());
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo("13"));
            Assert.That(result.Email, Is.EqualTo("squidlauncher@bankofevil.com"));
        });
    }

    [Test]
    public async Task HandleGetUserByEmail_NotFound_Test()
    {
        _mockUserRepo.Setup(m => m.GetUserByEmailAsync("Vector")).ReturnsAsync((IdentityUser)null!);

        var request = new GetUserByEmailQuery("Vector");

        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Null);
    }
}
