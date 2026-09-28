using ProjectMetadataPlatform.Application.Helper.Models;

namespace ProjectMetadataPlatform.Application.Users;

public record UserCursor(string Email, string EmployeeId) : Cursor;
