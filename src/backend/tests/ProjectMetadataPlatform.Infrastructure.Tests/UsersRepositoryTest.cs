using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Errors.UserException;
using ProjectMetadataPlatform.Domain.Users;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Users;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class UsersRepositoryTest : TestsWithDatabase
{
    [SetUp]
    public void Setup()
    {
        _mockUserManager = new Mock<UserManager<ApplicationUser>>(
            new Mock<IUserStore<ApplicationUser>>().Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
        );
        _context = DbContext();
        _repository = new UsersRepository(_context, _mockUserManager.Object);

        ClearData(_context);
    }

    private ProjectMetadataPlatformDbContext _context;
    private UsersRepository _repository;
    private Mock<UserManager<ApplicationUser>> _mockUserManager;

    [TearDown]
    public void TearDown()
    {
        using var context = DbContext();

        _ = context.Database.EnsureDeleted();

        _context?.Dispose();
    }

    [Test]
    public async Task CheckLogin_Successful_Test()
    {
        const string email = "test";
        const string password = "test";

        _ = _mockUserManager
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new ApplicationUser
                {
                    EmployeeId = "Id",
                    Email = email,
                    IsActive = true,
                    IsScimProvisioned = false,
                }
            );
        _ = _mockUserManager
            .Setup(m => m.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var result = await _repository.CheckLogin(email, password);
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckLogin_Unsuccessful_Test()
    {
        const string email = "test";
        const string password = "test";

        _ = _mockUserManager
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(
                new ApplicationUser
                {
                    EmployeeId = "Id",
                    Email = email,
                    IsActive = true,
                    IsScimProvisioned = false,
                }
            );
        _ = _mockUserManager
            .Setup(m => m.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _repository.CheckLogin(email, password);
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CreateUserAsync_Test()
    {
        var user = new ApplicationUser
        {
            Id = "1",
            EmployeeId = "Id",
            Email = "Example Email",
            IsActive = true,
            IsScimProvisioned = false,
        };
        const string password = "test";
        _ = _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        var id = await _repository.CreateUserAsync(user, password);
        Assert.That(id, Is.EqualTo("1"));
    }

    [Test]
    public void CreateUserAsync_InvalidPassword_Test()
    {
        _ = _context.Users.Add(
            new ApplicationUser
            {
                EmployeeId = "Id",
                Email = "Example Email",
                Id = "1",
                IsActive = true,
                IsScimProvisioned = false,
            }
        );
        var user = new ApplicationUser
        {
            EmployeeId = "Id",
            Email = "Example Email",
            IsActive = true,
            IsScimProvisioned = false,
        };
        const string password = "test";
        _ = _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed());

        _ = Assert.ThrowsAsync<UserCouldNotBeCreatedException>(() =>
            _repository.CreateUserAsync(user, password)
        );
    }

    [Test]
    public void CreateUserAsync_DuplicateEmail_Test()
    {
        _ = _context.Users.Add(
            new ApplicationUser
            {
                EmployeeId = "Id1",
                Email = "Example Email",
                Id = "1",
                IsActive = true,
                IsScimProvisioned = false,
            }
        );
        var user = new ApplicationUser
        {
            EmployeeId = "Id2",
            Email = "Example Email",
            IsActive = true,
            IsScimProvisioned = false,
        };
        const string password = "test";
        _ = _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName" }));

        var exception = Assert.ThrowsAsync<UserAlreadyExistsException>(() =>
            _repository.CreateUserAsync(user, password)
        );
        Assert.That(exception.Message, Is.EqualTo("User creation Failed : DuplicateEmail"));
    }

    [Test]
    public async Task GetUsersAsync_EmptyResponse_Test()
    {
        var result = await _repository.GetUsersAsync("");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetUsersAsyncNoFilter_Test()
    {
        var usersResponseContent = new List<ApplicationUser>
        {
            new()
            {
                EmployeeId = "Id",
                Id = "1",
                Email = "Hinz",
                IsActive = true,
                IsScimProvisioned = false,
            },
        };

        _context.Users.AddRange(usersResponseContent);
        _ = await _context.SaveChangesAsync();

        var result = (await _repository.GetUsersAsync("")).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.ElementAt(0).Id, Is.EqualTo("1"));
            Assert.That(result.ElementAt(0).Email, Is.EqualTo("Hinz"));
        });
    }

    [Test]
    public async Task GetUsersAsyncEmployeeNumberFilter_Test()
    {
        var usersResponseContent = new List<ApplicationUser>
        {
            new()
            {
                EmployeeId = "Id",
                Id = "1",
                Email = "Hinz",
                IsActive = true,
                IsScimProvisioned = false,
            },
            new()
            {
                EmployeeId = "Id2",
                Id = "2",
                Email = "Hanz",
                IsActive = true,
                IsScimProvisioned = false,
            },
        };

        _context.Users.AddRange(usersResponseContent);
        _ = await _context.SaveChangesAsync();

        var result = (await _repository.GetUsersAsync("externalId eq \"Id\"")).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo("1"));
            Assert.That(result[0].Email, Is.EqualTo("Hinz"));
        });
    }

    [Test]
    public async Task GetUsersAsyncEmailFilter_Test()
    {
        var usersResponseContent = new List<ApplicationUser>
        {
            new()
            {
                EmployeeId = "Id",
                Id = "1",
                Email = "Hinz",
                IsActive = true,
                IsScimProvisioned = false,
            },
            new()
            {
                EmployeeId = "Id2",
                Id = "2",
                Email = "Hanz",
                IsActive = true,
                IsScimProvisioned = false,
            },
        };

        _context.Users.AddRange(usersResponseContent);
        _ = await _context.SaveChangesAsync();

        var result = (await _repository.GetUsersAsync("userName eq \"Hanz\"")).ToList();

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo("2"));
            Assert.That(result[0].Email, Is.EqualTo("Hanz"));
        });
    }

    [Test]
    public async Task GetUserByIdAsync_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "Id",
            Id = "1",
            Email = "Hinz",
            IsActive = true,
            IsScimProvisioned = false,
        };
        _context.Users.AddRange(user);
        _ = await _context.SaveChangesAsync();

        var result = await _repository.GetUserByIdAsync("Id");

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<ApplicationUser>());
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo("1"));
            Assert.That(result.Email, Is.EqualTo("Hinz"));
        });
    }

    [Test]
    public void GetUserByIdAsync_NonexistentUser_Test()
    {
        _ = _mockUserManager
            .Setup(m => m.FindByIdAsync("1"))
            .ThrowsAsync(new UserNotFoundException("1"));
        _ = Assert.ThrowsAsync<UserNotFoundException>(() => _repository.GetUserByIdAsync("1"));
    }

    [Test]
    public async Task GetUserByEmailAsync_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "Id",
            Email = "bigboss@bankofevil.com",
            Id = "1",
            IsActive = true,
            IsScimProvisioned = false,
        };
        _ = _context.Users.Add(user);
        _ = await _context.SaveChangesAsync();
        var result = await _repository.GetUserByEmailAsync("bigboss@bankofevil.com");
        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public void GetUserByEmailAsync_NotFound_Test()
    {
        _ = _mockUserManager
            .Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
            .ThrowsAsync(new UserNotFoundException("1"));

        _ = Assert.ThrowsAsync<UserNotFoundException>(() => _repository.GetUserByEmailAsync("1"));
    }

    [Test]
    public async Task StoreUser_CreatesUser_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "Id",
            Id = "",
            Email = "notblackmidi@geordiegreep.com",
            IsActive = true,
            IsScimProvisioned = false,
        };
        _ = _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);
        var result = await _repository.StoreUser(user);

        _mockUserManager.Verify(x => x.CreateAsync(user), Times.Once);

        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public async Task StoreUser_UpdatesUser_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "Id",
            Id = "13",
            Email = "emily.armstrong@linkinpark.leipzig.de",
            IsActive = true,
            IsScimProvisioned = false,
        };
        _ = _mockUserManager
            .Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);
        var result = await _repository.StoreUser(user);

        _mockUserManager.Verify(x => x.UpdateAsync(user), Times.Once);

        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public void StoreUserAsync_Create_DuplicateEmail_Test()
    {
        _ = _context.Users.Add(
            new ApplicationUser
            {
                EmployeeId = "Id",
                Email = "Example Email",
                Id = "1",
                IsActive = true,
                IsScimProvisioned = false,
            }
        );
        var user = new ApplicationUser
        {
            EmployeeId = "Id",
            Email = "Example Email",
            Id = "",
            IsActive = true,
            IsScimProvisioned = false,
        };
        _ = _mockUserManager
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName" }));

        _ = Assert.ThrowsAsync<UserAlreadyExistsException>(() => _repository.StoreUser(user));
    }

    [Test]
    public void StoreUserAsync_Update_DuplicateEmail_Test()
    {
        _ = _context.Users.Add(
            new ApplicationUser
            {
                EmployeeId = "Id",
                Email = "Example Email",
                Id = "1",
                IsActive = true,
                IsScimProvisioned = false,
            }
        );
        var user = new ApplicationUser
        {
            EmployeeId = "Id",
            Email = "Example Email",
            Id = "5",
            IsActive = true,
            IsScimProvisioned = false,
        };
        _ = _mockUserManager
            .Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName" }));

        _ = Assert.ThrowsAsync<UserAlreadyExistsException>(() => _repository.StoreUser(user));
    }

    [Test]
    public async Task DeleteUserAsync_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "Id",
            Id = "1",
            IsActive = true,
            IsScimProvisioned = false,
        };
        _ = _mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);
        _ = _mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        var result = await _repository.DeleteUserAsync(user);

        _mockUserManager.Verify(x => x.DeleteAsync(user), Times.Once);

        Assert.That(result, Is.EqualTo(user));
    }

    [Test]
    public void DeleteUser_Failed_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "Id",
            Id = "1",
            IsActive = true,
            IsScimProvisioned = false,
        };
        _ = _mockUserManager.Setup(m => m.FindByIdAsync("1")).ReturnsAsync(user);
        _ = _mockUserManager.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Failed());

        _ = Assert.ThrowsAsync<UserCouldNotBeDeletedException>(() =>
            _repository.DeleteUserAsync(user)
        );
    }

    [Test]
    public async Task CheckPasswordFormat_Correct_Test()
    {
        const string password = "test11A!!!";
        var result = await _repository.CheckPasswordFormat(password);
        Assert.That(result, Is.True);
    }

    [Test]
    public void CheckPasswordFormat_Incorrect_Test()
    {
        const string password = "test";
        _ = Assert.ThrowsAsync<UserInvalidPasswordFormatException>(() =>
            _repository.CheckPasswordFormat(password)
        );
    }

    [Test]
    public async Task CheckUserExits_False_Test()
    {
        const string employeeId = "test11A!!!";
        var result = await _repository.CheckUserExists(employeeId);
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckUserExits_True_Test()
    {
        var user = new ApplicationUser
        {
            EmployeeId = "test11A!!!",
            Id = "1",
            IsActive = true,
            IsScimProvisioned = false,
        };
        _ = _context.Users.Add(user);
        _ = await _context.SaveChangesAsync();
        const string employeeId = "test11A!!!";
        var result = await _repository.CheckUserExists(employeeId);
        Assert.That(result, Is.True);
    }
}
