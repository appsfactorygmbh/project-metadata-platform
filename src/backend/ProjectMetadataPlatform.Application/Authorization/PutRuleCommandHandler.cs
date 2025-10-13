using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Casbin;
using MediatR;
using ProjectMetadataPlatform.Domain.Authorization;

namespace ProjectMetadataPlatform.Application.Authorization;

public class PutRuleCommandHandler : IRequestHandler<PutRuleCommand, bool>
{
    private readonly IEnforcer _enforcer;

    public PutRuleCommandHandler(IEnforcer enforcer)
    {
        _enforcer = enforcer;
    }

    public async Task<bool> Handle(PutRuleCommand request, CancellationToken cancellationToken)
    {
        await _enforcer.LoadPolicyAsync();

        var sub_rule = ConvertToPolicyRuleString(request.PolicyRule.SubjectRule, "r.sub");
        var obj_rule = ConvertToPolicyRuleString(request.PolicyRule.ObjectRule, "r.obj");
        var env_rule = ConvertToPolicyRuleString(request.PolicyRule.EnvironmentRule, "r.env");

        await _enforcer.AddPolicyAsync(
            sub_rule,
            obj_rule,
            env_rule,
            request.PolicyRule.Action,
            request.PolicyRule.Effect.ToString().ToLower()
        );
        return await _enforcer.SavePolicyAsync();
    }

    public string ConvertToPolicyRuleString(
        IEnumerable<PolicyRule.RuleElementGroup> ruleElementGroups,
        string target
    )
    {
        var ruleString = "";

        foreach (var rule in ruleElementGroups)
        {
            IEnumerable<string> rulePart = [];
            foreach (var ruleElement in rule.RuleElements)
            {
                var value = ruleElement.Value.ToString();
                var a = (ruleElement.Value as JsonElement?).GetValueOrDefault().ValueKind;
                var b = JsonValueKind.String;
                if (
                    (ruleElement.Value as JsonElement?).GetValueOrDefault().ValueKind
                    == JsonValueKind.String
                )
                {
                    value = "\"" + value + "\"";
                }
                string ruleOperation = ruleElement.Operation switch
                {
                    Operation.EQUAL => $"{target}.{ruleElement.Attribute} == {value}",
                    Operation.UNEQUAL => $"{target}.{ruleElement.Attribute} != {value}",
                    Operation.CONTAINS => $"{target}.{ruleElement.Attribute}.Contains({value})",
                    Operation.NOT_IN => $"!{target}.{ruleElement.Attribute}.Contains({value})",
                    Operation.STARTS_WITH =>
                        $"{target}.{ruleElement.Attribute}.StartsWith({value})",
                    Operation.ENDS_WITH => $"{target}.{ruleElement.Attribute}.EndsWith({value})",
                    Operation.EMPTY => $"!{target}.{ruleElement.Attribute}.Any()",
                    _ => "",
                };
                rulePart = rulePart.Append(ruleOperation);
            }
            ruleString +=
                "(" + String.Join(rule.Logic == Logic.AND ? " && " : " || ", rulePart) + ")";
        }

        if (!ruleString.Any())
            ruleString = "true";
        return ruleString;
    }
}
