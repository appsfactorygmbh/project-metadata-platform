using System.Collections.Generic;

namespace ProjectMetadataPlatform.Domain.Authorization;

/// <summary>
/// Record representing a rule of a authorization policy.
/// </summary>
public record PolicyRule
{
    /// <summary>
    /// List of Conditions for the subject.
    /// </summary>
    public IEnumerable<RuleElementGroup> SubjectRule { get; set; } = [];

    /// <summary>
    /// List of Conditions for the object.
    /// </summary>
    public IEnumerable<RuleElementGroup> ObjectRule { get; set; } = [];

    /// <summary>
    /// List of Conditions for the environment.
    /// </summary>
    public IEnumerable<RuleElementGroup> EnvironmentRule { get; set; } = [];

    /// <summary>
    /// Action the rule is referencing.
    /// </summary>
    public required string Action { get; set; }

    /// <summary>
    /// Effect of the rule.
    /// </summary>
    public required Effect Effect { get; set; }

    /// <summary>
    /// Record representing part of a policy rule.
    /// </summary>
    public record RuleElementGroup
    {
        /// <summary>
        /// Logic Operator to combine rule elements.
        /// </summary>
        public required Logic Logic { get; set; }

        /// <summary>
        /// Smallest Element of a policy rule.
        /// </summary>
        public IEnumerable<RuleElement> RuleElements { get; set; } = [];

        /// <summary>
        /// Record representing the smallest element of a policy rule.
        /// </summary>
        public record RuleElement
        {
            /// <summary>
            /// Attribute to check in a Condition.
            /// </summary>
            public required string Attribute { get; set; }

            /// <summary>
            /// Operation for the Condition.
            /// </summary>
            public required Operation Operation { get; set; }

            /// <summary>
            /// Value to check against.
            /// </summary>
            public object? Value { get; set; }
        }
    }
}
