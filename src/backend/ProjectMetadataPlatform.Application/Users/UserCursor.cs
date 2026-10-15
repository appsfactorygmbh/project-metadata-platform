using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Application.Users;

/// <summary>
/// Cursor Record pointing towards a User Resource.
/// </summary>
/// <param name="Email">Email of the User.</param>
/// <param name="EmployeeId">Employee Id of the User.</param>
public record UserCursor(string Email, string EmployeeId) : Cursor;
