using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectMetadataPlatform.Application;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Auth;
using ProjectMetadataPlatform.Infrastructure.DataAccess;
using ProjectMetadataPlatform.Infrastructure.Projects;

namespace ProjectMetadataPlatform.Infrastructure.Auth;

/// <summary>
/// Handles User Management using the UserManager provided by AspNetCore Identity.
/// </summary>
public class AuthRepository : RepositoryBase<RefreshToken>, IAuthRepository
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ProjectMetadataPlatformDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectsRepository" /> class.
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="userManager"></param>
    public AuthRepository(
        ProjectMetadataPlatformDbContext dbContext,
        UserManager<IdentityUser> userManager
    )
        : base(dbContext)
    {
        _userManager = userManager;
        _context = dbContext;
    }

    /// <summary>
    /// Checks if the given login credentials are correct.
    /// </summary>
    /// <param name="email">Email of the user</param>
    /// <param name="password">Password of the user</param>
    /// <returns>True, if the credentials are correct</returns>
    public async Task<bool> CheckLogin(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null && await _userManager.CheckPasswordAsync(user, password);
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
        _ = await _context.SaveChangesAsync();
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
        var token = GetIf(rt => user != null && rt.UserId == user.Id).FirstOrDefaultAsync().Result;
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

        _ = await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks for the existence of a refresh Token for a specific user.
    /// </summary>
    /// <param name="email">Email of a user</param>
    /// <returns>True if a token exists; False if no token exists</returns>
    public async Task<bool> CheckRefreshTokenExists(string email)
    {
        return await GetIf(rt => rt.User != null && rt.User.Email == email).AnyAsync();
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
}
