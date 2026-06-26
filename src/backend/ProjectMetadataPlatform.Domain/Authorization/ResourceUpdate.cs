namespace ProjectMetadataPlatform.Domain.Authorization;

public class ResourceUpdate
{
    public required string PropertyName { get; set; }

    public object? NewValue { get; set; }
}
