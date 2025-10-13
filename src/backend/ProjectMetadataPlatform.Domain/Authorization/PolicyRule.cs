using System.Collections.Generic;

namespace ProjectMetadataPlatform.Domain.Authorization;

public record PolicyRule
{
    public IEnumerable<RuleElementGroup> SubjectRule { get; set; }

    public IEnumerable<RuleElementGroup> ObjectRule { get; set; }

    public IEnumerable<RuleElementGroup> EnvironmentRule { get; set; }

    public string Action { get; set; }

    public Effect Effect { get; set; }

    public record RuleElementGroup
    {
        public Logic Logic { get; set; }

        public IEnumerable<RuleElement> RuleElements { get; set; }

        public record RuleElement
        {
            public string Attribute { get; set; }

            public Operation Operation { get; set; }

            public object Value { get; set; }
        }
    }
}
