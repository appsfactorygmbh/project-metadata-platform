using System.Text.Json.Serialization;

namespace ProjectMetadataPlatform.Api.Errors;

/// <summary>
/// Response model representing an error response.
/// </summary>
/// <param name="Message">The error message.</param>
public record ErrorResponse([property: JsonPropertyName("message")] string Message);
