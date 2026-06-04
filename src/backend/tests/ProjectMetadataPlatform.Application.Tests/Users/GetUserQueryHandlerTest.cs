using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Application.Users;
using ProjectMetadataPlatform.Domain.Users;

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
        var userResponseContent = new ApplicationUser
        {
            EmployeeId = "1000",
            Id = "1",
            Email = "Hinz",
            IsActive = true,
            IsScimProvisioned = false,
        };

        _mockUserRepo.Setup(m => m.GetUserByIdAsync("1000")).ReturnsAsync(userResponseContent);
        var request = new GetUserQuery("1000");
        var result = await _handler.Handle(request, It.IsAny<CancellationToken>());

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ApplicationUser>());
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo("1"));
            Assert.That(result.Email, Is.EqualTo("Hinz"));
            Assert.That(result.EmployeeId, Is.EqualTo("1000"));
            Assert.That(result.IsActive, Is.EqualTo(true));
            Assert.That(result.IsScimProvisioned, Is.EqualTo(false));
        });
    }
}
