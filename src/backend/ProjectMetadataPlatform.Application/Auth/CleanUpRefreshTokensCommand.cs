using ProjectMetadataPlatform.Application.Interfaces;

namespace ProjectMetadataPlatform.Application.Auth;

/// <summary>
/// Command For Removing Expired Refresh Tokens
/// </summary>
public record CleanUpRefreshTokensCommand() : IRequest;
