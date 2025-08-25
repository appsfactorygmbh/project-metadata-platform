using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class GetUserQueryHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockUserRepo = new Mock<IUsersRepository>();
        _handler = new GetUserQueryHandler(_mockUserRepo.Object);
    }

    private GetUserQueryHandler _handler;
    private Mock<IUsersRepository> _mockUserRepo;

    [Test]
    public async Task HandleGetUserRequest_Test()
    {
        var userResponseContent = new IdentityUser { Id = "1", Email = "Hinz" };

        _mockUserRepo.Setup(m => m.GetUserByIdAsync("1")).ReturnsAsync(userResponseContent);
        var request = new GetUserQuery("1");
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IdentityUser>());
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo("1"));
            Assert.That(result.Email, Is.EqualTo("Hinz"));
        });
    }
}
