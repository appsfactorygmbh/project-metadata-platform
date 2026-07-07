using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using Cerbos.Api.V1.Engine;
using Cerbos.Sdk.Builder;
using Google.Protobuf.WellKnownTypes;
using ProjectMetadataPlatform.Domain.Authorization;
using Principal = Cerbos.Sdk.Builder.Principal;
using Resource = Cerbos.Sdk.Builder.Resource;

namespace ProjectMetadataPlatform.Infrastructure.Authorization;

/// <summary>
/// Helper Class for conversion between PmP and Cerbos objects.
/// </summary>
public static class AuthorizationConverter
{
    /// <summary>
    /// Options for Json Serialization.
    /// </summary>
    public static JsonSerializerOptions Options { get; set; } = new();

    /// <summary>
    /// Converts an Object to a Authorization Principle.
    /// </summary>
    /// <param name="obj"> Object to be converted.</param>
    /// <param name="kind">The Kind of Principle.</param>
    /// <returns>Cerbos Principle.</returns>
    public static Principal ToPrincipal(this Object obj, string kind)
    {
        return Principal
            .NewInstance(kind, "Default")
            .WithPolicyVersion(AuthorizationConstants.POLICY_VERSION)
            .WithAttributes(ConvertObjectToAttributeDict(obj));
    }

    /// <summary>
    /// Converts an Object to a Authorization Resource.
    /// </summary>
    /// <param name="obj"> Object to be converted.</param>
    /// <param name="kind">The Kind of Resource.</param>
    /// <param name="id">Id of the Resource.</param>
    /// <param name="updates">Dictionary of Update Requests.</param>
    /// <returns>Cerbos Resource</returns>
    public static Resource ToResource(
        this object obj,
        string kind,
        string id,
        Dictionary<string, object?>? updates = null
    )
    {
        var attributeDict = ConvertObjectToAttributeDict(obj);
        if (updates != null)
        {
            attributeDict.Add(
                "UpdateRequests",
                AttributeValue.MapValue(ConvertObjectToAttributeDict(updates))
            );
        }
        return Resource
            .NewInstance(kind, id)
            .WithPolicyVersion(AuthorizationConstants.POLICY_VERSION)
            .WithAttributes(attributeDict);
    }

    /// <summary>
    /// Converts a Object to an Dictionary of Attribute names and Cerbos Attribute Values.
    /// </summary>
    /// <param name="obj">Object to be converted</param>
    /// <returns>Dictionary Representing the Object.</returns>
    /// <exception cref="ArgumentException">Thrown if the Object Serialized as Json is not an complex object.</exception>
    private static Dictionary<string, AttributeValue> ConvertObjectToAttributeDict(Object obj)
    {
        if (obj == null)
        {
            return [];
        }
        var element = JsonSerializer.SerializeToElement(obj, Options);

        if (element.ValueKind != JsonValueKind.Object)
        {
            throw new ArgumentException("Non Json-Objects can't be converted");
        }
        return ConvertJsonElementToAttributeDict(element);
    }

    /// <summary>
    /// Converts a Json Element to an Dictionary of Attribute names and Cerbos Attribute Values.
    /// </summary>
    /// <param name="element">Element to be converted.</param>
    /// <returns>Dictionary Representing the Element</returns>
    private static Dictionary<string, AttributeValue> ConvertJsonElementToAttributeDict(
        JsonElement element
    )
    {
        var attributeDict = new Dictionary<string, AttributeValue> { };

        foreach (var property in element.EnumerateObject())
        {
            attributeDict.Add(property.Name, ConvertJsonElementToAttributeValue(property.Value));
        }

        return attributeDict;
    }

    /// <summary>
    /// Converts a Json Element to its Cerbos Attribute Value.
    /// </summary>
    /// <param name="element">Element to be converted.</param>
    /// <returns>Attribute Value representing the Json Element</returns>
    private static AttributeValue ConvertJsonElementToAttributeValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => AttributeValue.MapValue(
                ConvertJsonElementToAttributeDict(element)
            ),
            JsonValueKind.Array => AttributeValue.ListValue(
                ConvertJsonElementToAttributeValueArray(element)
            ),
            JsonValueKind.String => AttributeValue.StringValue(element.GetString()),
            JsonValueKind.Number => AttributeValue.DoubleValue(element.GetDouble()),
            JsonValueKind.True => AttributeValue.BoolValue(true),
            JsonValueKind.False => AttributeValue.BoolValue(false),
            JsonValueKind.Undefined or JsonValueKind.Null or _ => AttributeValue.NullValue(),
        };
    }

    /// <summary>
    /// Converts a Json Element to a Array of Cerbos Attribute Values.
    /// </summary>
    /// <param name="element">Element to be converted.</param>
    /// <returns>Array of Attribute Values representing the Json Element</returns>
    private static AttributeValue[] ConvertJsonElementToAttributeValueArray(JsonElement element)
    {
        List<AttributeValue> list = [];
        foreach (var value in element.EnumerateArray())
        {
            list.Add(ConvertJsonElementToAttributeValue(value));
        }
        return [.. list];
    }

    /// <summary>
    /// Converts a Cerbos Plan Filter to an EFCore IQueryable
    /// </summary>
    /// <typeparam name="T">Type of Resource.</typeparam>
    /// <param name="query">Base Query.</param>
    /// <param name="filter">Cerbos AST to be converted.</param>
    /// <param name="attributeMap">Optional Mapping of Attribute Names to names used in policies.</param>
    /// <returns>IQueryable representing the AST.</returns>
    public static IQueryable<T> ConvertAstToQueryable<T>(
        IQueryable<T> query,
        PlanResourcesFilter filter,
        Dictionary<string, string>? attributeMap = null
    )
    {
        if (filter == null || filter.Kind == PlanResourcesFilter.Types.Kind.AlwaysDenied)
        {
            return query.Where(x => false);
        }

        if (filter.Kind == PlanResourcesFilter.Types.Kind.AlwaysAllowed)
        {
            return query;
        }

        if (
            filter.Condition?.NodeCase
            != PlanResourcesFilter.Types.Expression.Types.Operand.NodeOneofCase.Expression
        )
        {
            return query;
        }

        var param = Expression.Parameter(typeof(T), "e");
        var body = Traverse(filter.Condition.Expression, param, attributeMap ?? []);

        var lambda = Expression.Lambda<Func<T, bool>>(body, param);
        return query.Where(lambda);
    }

    /// <summary>
    /// Traverses a Cerbos Filter Expression and converts it to a Linq Expression.
    /// </summary>
    /// <param name="expression">Expression of a Cerbos AST</param>
    /// <param name="param">Parameter Expression</param>
    /// <param name="attributeMap">Optional Mapping of Attribute Names to names used in policies.</param>
    /// <returns>LinQ Expression representing the Filter Expression.</returns>
    /// <exception cref="NotSupportedException">Thrown if Cerbos Operation is not supported by this method.</exception>
    private static Expression Traverse(
        PlanResourcesFilter.Types.Expression expression,
        ParameterExpression param,
        Dictionary<string, string> attributeMap
    )
    {
        var op = expression.Operator;
        var operands = expression.Operands;

        if (op is "and" or "or")
        {
            var childExprs = operands.Select(o => ResolveOperand(o, param, attributeMap)).ToList();
            if (childExprs.Count == 0)
            {
                return Expression.Constant(op == "and");
            }

            return childExprs.Aggregate(
                (left, right) =>
                    op == "and" ? Expression.AndAlso(left, right) : Expression.OrElse(left, right)
            );
        }

        if (op == "not")
        {
            return Expression.Not(ResolveOperand(operands[0], param, attributeMap));
        }

        if (operands.Count >= 2 && op != "list" && op != "lambda")
        {
            var left = ResolveOperand(operands[0], param, attributeMap);
            var right = ResolveOperand(operands[1], param, attributeMap);

            AlignTypes(ref left, ref right);

            switch (op)
            {
                case "eq":
                    return Expression.Equal(left, right);
                case "ne":
                    return Expression.NotEqual(left, right);
                case "lt":
                    return Expression.LessThan(left, right);
                case "le":
                    return Expression.LessThanOrEqual(left, right);
                case "gt":
                    return Expression.GreaterThan(left, right);
                case "ge":
                    return Expression.GreaterThanOrEqual(left, right);
                case "add":
                    if (left.Type == typeof(string) || right.Type == typeof(string))
                    {
                        var concatMethod = typeof(string).GetMethod(
                            "Concat",
                            [typeof(object), typeof(object)]
                        );
                        return Expression.Call(
                            concatMethod!,
                            Expression.Convert(left, typeof(object)),
                            Expression.Convert(right, typeof(object))
                        );
                    }
                    return Expression.Add(left, right);
                case "sub":
                    return Expression.Subtract(left, right);
                case "mult":
                    return Expression.Multiply(left, right);
                case "div":
                    return Expression.Divide(left, right);
                case "mod":
                    return Expression.Modulo(left, right);
                case "in":
                    return BuildInOperator(left, right);
                case "index":
                    return Expression.ArrayIndex(left, right);
                default:
                    break;
            }
        }

        if (op == "list")
        {
            var elements = operands.Select(o => ResolveOperand(o, param, attributeMap)).ToList();
            var elementType = elements.FirstOrDefault()?.Type ?? typeof(object);
            var castedElements = elements.Select(e => Expression.Convert(e, elementType));
            return Expression.NewArrayInit(elementType, castedElements);
        }

        if (op == "lambda")
        {
            var lambdaVar = (
                (ConstantExpression)ResolveOperand(operands[0], param, attributeMap)
            ).Value!.ToString();

            var subParam = Expression.Parameter(typeof(object), lambdaVar);
            var lambdaBody = Traverse(operands[1].Expression, subParam, attributeMap);
            return Expression.Lambda(lambdaBody, subParam);
        }

        throw new NotSupportedException($"The Cerbos operator '{op}' is currently not supported.");
    }

    /// <summary>
    /// Resolves a Filter Operand to an LinQ Expression.
    /// </summary>
    /// <param name="operand">Operand to be converted.</param>
    /// <param name="param">Parameter Expression</param>
    /// <param name="attributeMap"> Optional Mapping of Attribute Names to names used in policies. </param>
    /// <returns>LinQ Expression representing the Operand.</returns>
    private static Expression ResolveOperand(
        PlanResourcesFilter.Types.Expression.Types.Operand operand,
        ParameterExpression param,
        Dictionary<string, string> attributeMap
    )
    {
        switch (operand.NodeCase)
        {
            case PlanResourcesFilter.Types.Expression.Types.Operand.NodeOneofCase.Value:
                return ResolveValue(operand.Value);

            case PlanResourcesFilter.Types.Expression.Types.Operand.NodeOneofCase.Variable:
                var path = operand.Variable;

                var mappedProp = attributeMap.ContainsKey(path)
                    ? attributeMap[path]
                    : path.Replace("request.resource.attr.", "")
                        .Replace("request.principal.attr.", "");
                return GetMemberExpression(param, mappedProp);

            case PlanResourcesFilter.Types.Expression.Types.Operand.NodeOneofCase.Expression:
                return Traverse(operand.Expression, param, attributeMap);
            case PlanResourcesFilter.Types.Expression.Types.Operand.NodeOneofCase.None:
            default:
                return Expression.Constant(false);
        }
    }

    /// <summary>
    /// Resolves a Protobuf wellknow type to an LinQ Expression.
    /// </summary>
    /// <param name="value">Value to be Converted.</param>
    /// <returns>LinQ Expression Representing the Value</returns>
    private static ConstantExpression ResolveValue(Value value)
    {
        switch (value.KindCase)
        {
            case Value.KindOneofCase.StringValue:
                return Expression.Constant(value.StringValue);
            case Value.KindOneofCase.NumberValue:
                return Expression.Constant(value.NumberValue);
            case Value.KindOneofCase.BoolValue:
                return Expression.Constant(value.BoolValue);
            case Value.KindOneofCase.NullValue:
                return Expression.Constant(null);
            case Value.KindOneofCase.ListValue:
                var constants = value.ListValue.Values.Select(v => ResolveValue(v).Value).ToArray();
                return Expression.Constant(constants);
            case Value.KindOneofCase.None:
            case Value.KindOneofCase.StructValue:
            default:
                return Expression.Constant(null);
        }
    }

    /// <summary>
    /// Converts an Cerbos in-Expression to a LinQ Expression.
    /// </summary>
    /// <param name="item">Item that in-Expression is checked for.</param>
    /// <param name="list">List that in-Expression is checked for.</param>
    /// <returns>LinQ Expression representing in-Constraint.</returns>
    private static MethodCallExpression BuildInOperator(Expression item, Expression list)
    {
        if (list.NodeType == ExpressionType.Constant)
        {
            var constExpr = (ConstantExpression)list;
            if (constExpr.Value is object[] objArray)
            {
                var elementType = item.Type;
                var underlyingType = Nullable.GetUnderlyingType(elementType) ?? elementType;
                var typedArray = Array.CreateInstance(elementType, objArray.Length);

                for (var i = 0; i < objArray.Length; i++)
                {
                    var convertedVal =
                        underlyingType.IsEnum && objArray[i] is string strVal
                            ? System.Enum.Parse(underlyingType, strVal, ignoreCase: true)
                            : Convert.ChangeType(objArray[i], underlyingType);

                    typedArray.SetValue(convertedVal, i);
                }
                list = Expression.Constant(typedArray);
            }
        }

        var containsMethod = typeof(Enumerable)
            .GetMethods()
            .Single(m => m.Name == "Contains" && m.GetParameters().Length == 2)
            .MakeGenericMethod(item.Type);

        return Expression.Call(null, containsMethod, list, item);
    }

    /// <summary>
    /// Creates Member Expression from a Parameter and a Property Path
    /// </summary>
    /// <param name="param">LinQ Parameter</param>
    /// <param name="propertyPath">Path to Property.</param>
    /// <returns>Member Expression.</returns>
    private static Expression GetMemberExpression(Expression param, string propertyPath)
    {
        return propertyPath.Split('.').Aggregate(param, Expression.PropertyOrField);
    }

    /// <summary>
    /// Aligns the types of two LinQ Constant Expressions
    /// </summary>
    /// <param name="left">A LinQ Constant.</param>
    /// <param name="right">Another LinQ Constant.</param>
    private static void AlignTypes(ref Expression left, ref Expression right)
    {
        if (left.Type == right.Type)
        {
            return;
        }

        var leftUnderlying = Nullable.GetUnderlyingType(left.Type) ?? left.Type;
        var rightUnderlying = Nullable.GetUnderlyingType(right.Type) ?? right.Type;

        if (left.NodeType == ExpressionType.Constant && right.NodeType != ExpressionType.Constant)
        {
            var val = ((ConstantExpression)left).Value;
            if (val != null)
            {
                var converted =
                    rightUnderlying.IsEnum && val is string strVal
                        ? System.Enum.Parse(rightUnderlying, strVal, ignoreCase: true)
                        : Convert.ChangeType(val, rightUnderlying);
                left = Expression.Constant(converted, right.Type);
            }
        }
        else if (
            right.NodeType == ExpressionType.Constant
            && left.NodeType != ExpressionType.Constant
        )
        {
            var val = ((ConstantExpression)right).Value;
            if (val != null)
            {
                var converted =
                    leftUnderlying.IsEnum && val is string strVal
                        ? System.Enum.Parse(leftUnderlying, strVal, ignoreCase: true)
                        : Convert.ChangeType(val, leftUnderlying);

                right = Expression.Constant(converted, left.Type);
            }
        }
    }
}
