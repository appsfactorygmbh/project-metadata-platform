namespace ProjectMetadataPlatform.Api.Teams.Models;

/// <summary>
/// The representation of a team in responses.
/// </summary>
public class GetTeamResponse
{
    /// <summary>
    /// Gets or sets the id of the team dto.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the team dto. This property is required and unique.
    /// </summary>
    public required string TeamName { get; set; }

    /// <summary>
    /// Gets or sets the business unit associated with the team. This property is required.
    /// </summary>
    public required string BusinessUnit { get; set; }

    /// <summary>
    /// Gets or sets the PTL associated with the team. This property is optional.
    /// </summary>
    public string? PTL { get; set; }
}
