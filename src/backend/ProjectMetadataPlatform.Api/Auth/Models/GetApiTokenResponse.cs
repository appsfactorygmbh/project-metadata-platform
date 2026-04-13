using System;
using ProjectMetadataPlatform.Domain.Auth;

namespace ProjectMetadataPlatform.Api.Auth.Models;

/// <summary>
/// Response for getting a Token without details.
/// </summary>
/// <param name="Id">Internal identifier of the token.</param>
/// <param name="Name">Name of the token.</param>
public record GetApiTokenResponse(int Id, string Name);
