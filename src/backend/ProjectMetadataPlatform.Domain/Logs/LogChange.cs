namespace ProjectMetadataPlatform.Domain.Logs;

/// <summary>
/// Represents a single Property that was changed.
/// </summary>
public class LogChange
{
    /// <summary>
    /// The id of the Log change
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the associated log.
    /// </summary>
    public Log? Log { get; set; }

    /// <summary>
    /// Gets or sets the ID of the log.
    /// </summary>
    public int LogId { get; set; }

    /// <summary>
    /// Gets or sets the old value of the property.
    /// </summary>
    public string OldValue { get; set; } = "";

    /// <summary>
    /// Gets or sets the new value of the property.
    /// </summary>
    public string NewValue { get; set; } = "";

    /// <summary>
    /// Gets or sets the name of the property that was changed.
    /// </summary>
    public required string Property { get; set; }
}
