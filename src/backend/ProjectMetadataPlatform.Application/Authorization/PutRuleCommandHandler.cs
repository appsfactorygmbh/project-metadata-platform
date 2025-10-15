using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Casbin;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Authorization;
using ProjectMetadataPlatform.Domain.Logs;

namespace ProjectMetadataPlatform.Application.Authorization;

/// <summary>
/// Handler for the <see cref="PutRuleCommand" />
/// </summary>
public class PutRuleCommandHandler : IRequestHandler<PutRuleCommand, bool>
{
    private readonly IEnforcer _enforcer;

    private readonly ILogRepository _logRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Creates a new Instance of  <see cref="PutRuleCommandHandler"/>"
    /// </summary>
    /// <param name="enforcer">Authorization Enforcer</param>
    /// <param name="logRepository">Log Repository</param>
    /// <param name="unitOfWork"></param>
    public PutRuleCommandHandler(
        IEnforcer enforcer,
        ILogRepository logRepository,
        IUnitOfWork unitOfWork
    )
    {
        _enforcer = enforcer;
        _logRepository = logRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Creates a new Policy Rule.
    /// </summary>
    /// <param name="request">Contains the Policy Rule to be created.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Bool signifying if the rule could be added.</returns>
    public async Task<bool> Handle(PutRuleCommand request, CancellationToken cancellationToken)
    {
        await _enforcer.LoadPolicyAsync();

        var sub_rule = ConvertToPolicyRuleString(request.PolicyRule.SubjectRule, "r.sub");
        var obj_rule = ConvertToPolicyRuleString(request.PolicyRule.ObjectRule, "r.obj");
        var env_rule = ConvertToPolicyRuleString(request.PolicyRule.EnvironmentRule, "r.env");

        var changes = new List<LogChange>
        {
            new()
            {
                OldValue = "",
                NewValue =
                    $"{sub_rule} && {obj_rule} && {env_rule} && {request.PolicyRule.Action} && {request.PolicyRule.Effect.ToString().ToLower()}",
                Property = nameof(request.PolicyRule),
            },
        };
        await _logRepository.AddAuthorizationLogForCurrentUser(
            Domain.Logs.Action.ADDED_RULE,
            changes
        );
        var result = await _enforcer.AddPolicyAsync(
            sub_rule,
            obj_rule,
            env_rule,
            request.PolicyRule.Action,
            request.PolicyRule.Effect.ToString().ToLower()
        );
        result = result && await _enforcer.SavePolicyAsync();
        await _unitOfWork.CompleteAsync();
        return result;
    }

    /// <summary>
    /// Converts a Policy Rule object into an string enforcable by Casbin.
    /// </summary>
    /// <param name="ruleElementGroups">Policy Rule object.</param>
    /// <param name="target">Either Subject, Object or Environment.</param>
    /// <returns>Policy Rule as a Sring</returns>
    public static string ConvertToPolicyRuleString(
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
                var value = ruleElement.Value?.ToString() ?? "";
                if (
                    (ruleElement.Value as JsonElement?).GetValueOrDefault().ValueKind
                    == JsonValueKind.String
                )
                {
                    value = "\"" + value + "\"";
                }
                if (
                    (ruleElement.Value as JsonElement?).GetValueOrDefault().ValueKind
                        == JsonValueKind.True
                    || (ruleElement.Value as JsonElement?).GetValueOrDefault().ValueKind
                        == JsonValueKind.False
                )
                {
                    value = value.ToLower();
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

        if (!ruleString.Any() || ruleString == "()")
            ruleString = "true";
        return ruleString;
    }
}
