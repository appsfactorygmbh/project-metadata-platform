using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Domain.Users;
using ProjectMetadataPlatform.Infrastructure.DataAccess;

namespace ProjectMetadataPlatform.Infrastructure.Auth;

/// <summary>
/// Handles Refresh Token Management.
/// </summary>
public class RefreshTokenRepository : RepositoryBase<RefreshToken>, IRefreshTokenRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenRepository" /> class.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="userManager"></param>
    public RefreshTokenRepository(
        ProjectMetadataPlatformDbContext dbContext,
        UserManager<ApplicationUser> userManager
    )
        : base(dbContext)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Saves a refresh Token to the database.
    /// </summary>
    /// <param name="email">associated Email</param>
    /// <param name="refreshToken">Value of the Token</param>
    /// <returns></returns>
    public async Task StoreRefreshToken(string email, string refreshToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var expirationTime = int.Parse(
            EnvironmentUtils.GetEnvVarOrLoadFromFile("REFRESH_TOKEN_EXPIRATION_HOURS"),
            CultureInfo.InvariantCulture
        );
        var token = new RefreshToken
        {
            Token = refreshToken,
            User = user,
            UserId = user?.Id,
            ExpirationDate = DateTime.UtcNow.AddHours(expirationTime),
        };
        Create(token);
    }

    /// <summary>
    /// updates an existing refresh Token.
    /// </summary>
    /// <param name="email">associates Email</param>
    /// <param name="refreshToken">Values of the Token</param>
    /// <returns></returns>
    public async Task UpdateRefreshToken(string email, string refreshToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var token = await GetIf(rt => user != null && rt.UserId == user.Id).FirstOrDefaultAsync();
        var expirationTime = int.Parse(
            EnvironmentUtils.GetEnvVarOrLoadFromFile("REFRESH_TOKEN_EXPIRATION_HOURS"),
            CultureInfo.InvariantCulture
        );
        if (token != null)
        {
            token.Token = refreshToken;
            token.ExpirationDate = DateTime.UtcNow.AddHours(expirationTime);
            Update(token);
        }
    }

    /// <summary>
    /// Checks for the existence of a refresh Token for a specific user.
    /// </summary>
    /// <param name="email">Email of a user</param>
    /// <returns>True if a token exists; False if no token exists</returns>
    public async Task<bool> CheckRefreshTokenExists(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return await GetIf(rt => rt.User != null && user != null && rt.User.Id == user.Id)
            .AnyAsync();
    }

    /// <summary>
    /// Checks if a refresh Token is valid.
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns>true if the token is valid; false if the token isn't valid</returns>
    public async Task<bool> CheckRefreshTokenRequest(string refreshToken)
    {
        var token = await GetIf(rt => rt.Token == refreshToken)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        return token != null && token.ExpirationDate > DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the email related to a refresh Token.
    /// </summary>
    /// <param name="refreshToken">a refresh Token</param>
    /// <returns>a username</returns>
    public async Task<string?> GetEmailByRefreshToken(string refreshToken)
    {
        var token = await GetIf(rt => rt.Token == refreshToken)
            .AsNoTracking()
            .FirstOrDefaultAsync();
        var user = await _userManager.Users.FirstOrDefaultAsync(a =>
            token != null && a.Id == token.UserId
        );

        return user?.Email;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<RefreshToken>> GetExpiredTokens()
    {
        return await GetIf(rt => rt.ExpirationDate <= DateTimeOffset.UtcNow).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteRefreshTokens(IEnumerable<RefreshToken> refreshTokens)
    {
        DeleteRange([.. refreshTokens]);
    }
}
