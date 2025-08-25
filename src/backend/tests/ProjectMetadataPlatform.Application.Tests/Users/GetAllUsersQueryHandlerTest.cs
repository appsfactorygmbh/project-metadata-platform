using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;

namespace ProjectMetadataPlatform.Application.Tests.Users;

[TestFixture]
public class GetAllUsersQueryHandlerTest
{
    [SetUp]
    public void Setup()
    {
        _mockUserRepo = new Mock<IUsersRepository>();
        _handler = new GetAllUsersQueryHandler(_mockUserRepo.Object);
    }

    private GetAllUsersQueryHandler _handler;
    private Mock<IUsersRepository> _mockUserRepo;

    [Test]
    public async Task HandleGetAllUsersRequest_EmptyResponse_Test()
    {
        _mockUserRepo.Setup(m => m.GetAllUsersAsync()).ReturnsAsync([]);
        var request = new GetAllUsersQuery();
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        IdentityUser[] resultArray = result as IdentityUser[] ?? result.ToArray();
        Assert.That(resultArray, Is.Not.Null);
        Assert.That(resultArray, Is.InstanceOf<IEnumerable<IdentityUser>>());

        Assert.That(resultArray, Has.Length.EqualTo(0));
    }

    [Test]
    public async Task HandleGetAllUsersRequest_Test()
    {
        var usersResponseContent = new List<IdentityUser>
        {
            new() { Id = "1", Email = "Hinz" },
        };

        _mockUserRepo.Setup(m => m.GetAllUsersAsync()).ReturnsAsync(usersResponseContent);
        var request = new GetAllUsersQuery();
        var result = (await _handler.Handle(request, It.IsAny<CancellationToken>())).ToList();

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<IEnumerable<IdentityUser>>());

        var resultArray = result.ToArray();
        Assert.That(resultArray, Has.Length.EqualTo(1));
        Assert.Multiple(() =>
        {
            Assert.That(resultArray[0].Id, Is.EqualTo("1"));
            Assert.That(resultArray[0].Email, Is.EqualTo("Hinz"));
        });
    }
}
