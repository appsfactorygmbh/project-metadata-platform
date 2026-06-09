using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
using ProjectMetadataPlatform.Infrastructure.Auth;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Tests;

[TestFixture]
public class ApiTokenRepositoryTest : TestsWithDatabase
{
    private ProjectMetadataPlatformDbContext _context;
    private Mock<IPasswordHasher<ApiToken>> _passwordHasherMock;
    private ApiTokenRepository _apiTokenRepository;

    [SetUp]
    public void Setup()
    {
        _context = DbContext();
        _passwordHasherMock = new Mock<IPasswordHasher<ApiToken>>();
        _apiTokenRepository = new ApiTokenRepository(_context, _passwordHasherMock.Object);
        ClearData(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    [Test]
    public async Task GetApiTokens_EmptyResponseTest()
    {
        var result = await _apiTokenRepository.GetApiTokens();
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetApiTokens_EnumerableResponseTest()
    {
        List<ApiToken> tokens =
        [
            new ApiToken { Name = "Token1", Token = "TokenHash1" },
            new ApiToken { Name = "Token2", Token = "TokenHash2" },
        ];
        await _context.ApiTokens.AddRangeAsync(tokens);
        await _context.SaveChangesAsync();
        var result = await _apiTokenRepository.GetApiTokens();
        var resultList = result.ToList();
        Assert.That(result, Is.Not.Empty);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.Multiple(() =>
        {
            Assert.That(resultList[0].Name, Is.EqualTo("Token1"));
            Assert.That(resultList[0].Token, Is.EqualTo("TokenHash1"));
            Assert.That(resultList[1].Name, Is.EqualTo("Token2"));
            Assert.That(resultList[1].Token, Is.EqualTo("TokenHash2"));
        });
    }

    [Test]
    public async Task GetApiTokenById_NotFoundTest()
    {
        Assert.ThrowsAsync<ApiTokenNotFoundException>(() => _apiTokenRepository.GetApiTokenById(1));
    }

    [Test]
    public async Task GetApiTokenById_ReturnsTokenTest()
    {
        var token = new ApiToken
        {
            Id = 1,
            Name = "Token1",
            Token = "TokenHash1",
        };

        await _context.ApiTokens.AddAsync(token);
        await _context.SaveChangesAsync();

        var result = await _apiTokenRepository.GetApiTokenById(1);

        Assert.That(result, Is.EqualTo(token));
    }

    [Test]
    public async Task GetApiTokenByName_NotFoundTest()
    {
        Assert.ThrowsAsync<ApiTokenNotFoundException>(() =>
            _apiTokenRepository.GetApiTokenByName("Token")
        );
    }

    [Test]
    public async Task GetApiTokenByName_ReturnsTokenTest()
    {
        var token = new ApiToken
        {
            Id = 1,
            Name = "Token1",
            Token = "TokenHash1",
        };

        await _context.ApiTokens.AddAsync(token);
        await _context.SaveChangesAsync();

        var result = await _apiTokenRepository.GetApiTokenByName("Token1");

        Assert.That(result, Is.EqualTo(token));
    }

    [Test]
    public async Task DeleteApiTokenTest()
    {
        var token = new ApiToken
        {
            Id = 1,
            Name = "Token1",
            Token = "TokenHash1",
        };

        await _context.ApiTokens.AddAsync(token);
        await _context.SaveChangesAsync();

        var result = await _apiTokenRepository.GetApiTokenById(1);

        Assert.That(result, Is.EqualTo(token));

        await _apiTokenRepository.DeleteApiToken(result);
        await _context.SaveChangesAsync();
        Assert.ThrowsAsync<ApiTokenNotFoundException>(() => _apiTokenRepository.GetApiTokenById(1));
    }

    [Test]
    public async Task CheckScimTokenExists_False_Test()
    {
        var result = await _apiTokenRepository.CheckScimTokenExists();

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task CheckScimTokenExists_True_Test()
    {
        var token = new ApiToken
        {
            Id = 1,
            Name = "Token1",
            Token = "TokenHash1",
            Scopes = new List<TokenScopes> { TokenScopes.SCIM },
        };

        await _context.ApiTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        var result = await _apiTokenRepository.CheckScimTokenExists();

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task StoreApiTokenTest()
    {
        _passwordHasherMock
            .Setup(m => m.HashPassword(It.IsAny<ApiToken>(), It.IsAny<string>()))
            .Returns("HashedToken");

        var token = new ApiToken
        {
            Id = 1,
            Name = "Token",
            Token = "UnhashedTokenValue",
        };

        await _apiTokenRepository.StoreApiToken(token);
        await _context.SaveChangesAsync();

        var result = await _apiTokenRepository.GetApiTokenById(1);

        Assert.That(result.Name, Is.EqualTo("Token"));
        Assert.That(result.Token, Is.EqualTo("HashedToken"));
    }

    [Test]
    public async Task UpdateApiTokenTest()
    {
        _passwordHasherMock
            .Setup(m => m.HashPassword(It.IsAny<ApiToken>(), It.IsAny<string>()))
            .Returns("NewHashedToken");

        var token = new ApiToken
        {
            Id = 1,
            Name = "Token1",
            Token = "TokenHash1",
            Scopes = new List<TokenScopes> { TokenScopes.SCIM },
        };
        var newtoken = new ApiToken
        {
            Id = 1,
            Name = "NewToken",
            Token = "UnhashedTokenValue",
            Scopes = new List<TokenScopes> { TokenScopes.SCIM },
        };

        await _context.ApiTokens.AddAsync(token);
        await _context.SaveChangesAsync();
        _context.Entry(token).State = EntityState.Detached;
        await _apiTokenRepository.UpdateApiToken(newtoken);
        await _context.SaveChangesAsync();

        var result = await _apiTokenRepository.GetApiTokenById(1);

        Assert.That(result.Name, Is.EqualTo("NewToken"));
        Assert.That(result.Token, Is.EqualTo("NewHashedToken"));
        Assert.That(result.Scopes, Is.EqualTo(new List<TokenScopes> { TokenScopes.SCIM }));
    }

    [Test]
    public async Task GetVerifiedToken_EmptyResponseTest()
    {
        _passwordHasherMock
            .Setup(m =>
                m.VerifyHashedPassword(It.IsAny<ApiToken>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(PasswordVerificationResult.Failed);
        List<ApiToken> tokens =
        [
            new ApiToken { Name = "Token1", Token = "TokenHash1" },
            new ApiToken { Name = "Token2", Token = "TokenHash2" },
        ];
        await _context.ApiTokens.AddRangeAsync(tokens);
        await _context.SaveChangesAsync();
        var result = await _apiTokenRepository.GetVerifiedToken("token");
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetVerifiedToken_EnumerableResponseTest()
    {
        _passwordHasherMock
            .Setup(m =>
                m.VerifyHashedPassword(It.IsAny<ApiToken>(), It.IsAny<string>(), It.IsAny<string>())
            )
            .Returns(PasswordVerificationResult.Success);
        List<ApiToken> tokens =
        [
            new ApiToken { Name = "Token1", Token = "TokenHash1" },
            new ApiToken { Name = "Token2", Token = "TokenHash2" },
        ];
        await _context.ApiTokens.AddRangeAsync(tokens);
        await _context.SaveChangesAsync();
        var result = await _apiTokenRepository.GetVerifiedToken("test");
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Name, Is.EqualTo("Token1"));
            Assert.That(result?.Token, Is.EqualTo("TokenHash1"));
        });
    }
}
