using System.Collections.Generic;

namespace ProjectMetadataPlatform.Domain.Authorization;

/// <summary>
/// Represents an Access request filter.
/// </summary>
public record FilterTree
{
    /// <summary>
    /// Represents the Value of the current Filter Node as a string. It can be a Variable, a Value or an Operator.
    /// </summary>
    public required string NodeValue { get; set; }

    /// <summary>
    /// List of all Child Nodes of the Filter. Null if there are no Children
    /// </summary>
    public IEnumerable<FilterTree>? ChildNodes { get; set; }
}
