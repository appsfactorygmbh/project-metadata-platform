using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Errors.AuthExceptions;
using ProjectMetadataPlatform.Domain.Users;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Projects;

namespace ProjectMetadataPlatform.Infrastructure.Auth;

/// <summary>
/// Handles Management of Api Tokens.
/// </summary>
public class ApiTokenRepository : RepositoryBase<ApiToken>, IApiTokenRepository
{
    private readonly IPasswordHasher<ApiToken> _passwordHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiTokenRepository" /> class.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="passwordHasher"></param>
    public ApiTokenRepository(
        ProjectMetadataPlatformDbContext dbContext,
        IPasswordHasher<ApiToken> passwordHasher
    )
        : base(dbContext)
    {
        _passwordHasher = passwordHasher;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<ApiToken>> GetApiTokens()
    {
        return GetEverything();
    }

    /// <inheritdoc/>
    public async Task<ApiToken> GetApiTokenById(int id)
    {
        return await GetIf(t => t.Id == id).FirstOrDefaultAsync()
            ?? throw new ApiTokenNotFoundException(id);
    }

    /// <inheritdoc/>
    public async Task<ApiToken> GetApiTokenByName(string name)
    {
        return await GetIf(t => t.Name == name).FirstOrDefaultAsync()
            ?? throw new ApiTokenNotFoundException(name);
    }

    /// <inheritdoc/>
    public async Task DeleteApiToken(ApiToken token)
    {
        Delete(token);
    }

    /// <inheritdoc/>
    public async Task<bool> CheckScimTokenExists()
    {
        return await GetIf(t => t.Scopes != null && t.Scopes.Contains(TokenScopes.SCIM)).AnyAsync();
    }

    /// <inheritdoc/>
    public async Task StoreApiToken(ApiToken token)
    {
        token.Token = _passwordHasher.HashPassword(token, token.Token);
        Create(token);
    }

    /// <inheritdoc/>
    public async Task UpdateApiToken(ApiToken token)
    {
        token.Token = _passwordHasher.HashPassword(token, token.Token);
        Update(token);
    }

    /// <inheritdoc/>
    public async Task<ApiToken?> GetVerifiedToken(string token)
    {
        return (await GetEverything().ToListAsync()).FirstOrDefault(t =>
            _passwordHasher.VerifyHashedPassword(t, t.Token, token)
            == PasswordVerificationResult.Success
        );
    }
}
