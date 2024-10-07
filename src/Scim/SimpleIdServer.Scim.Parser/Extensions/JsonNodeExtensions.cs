// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Parser.Operators;
using System;
using System.Data;
using System.Linq;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Scim.Parser.Extensions;

public static class JsonNodeExtensions
{
    public static JsonNode Filter(this JsonNode jsonNode, SCIMExpression scimExpression, JsonObject result = null)
    {
        var attrExpression = scimExpression as SCIMAttributeExpression;
        var logicalExpression = scimExpression as SCIMLogicalExpression;
        var comparisonExpression = scimExpression as SCIMComparisonExpression;
        if (attrExpression != null) return jsonNode.Filter(attrExpression, result);
        if (comparisonExpression != null) return jsonNode.Filter(comparisonExpression);
        return null;
    }

    public static JsonNode Filter(this JsonNode jsonNode, SCIMAttributeExpression parameterExpression, JsonObject result = null)
    {
        var complex = parameterExpression as SCIMComplexAttributeExpression;
        var isJsonObject = jsonNode is JsonObject;
        if (!isJsonObject) return null;
        Func<JsonObject, JsonNode> getChild = (parameter) =>
        {
            if (!parameter.ContainsKey(parameterExpression.Name)) return null;
            var childJsonObject = parameter[parameterExpression.Name];
            if (complex != null)
            {
                childJsonObject = childJsonObject.Filter(complex.GroupingFilter);
            }

            if (parameterExpression.Child != null)
            {
                var sub = new JsonObject();
                var children = childJsonObject.Filter(parameterExpression.Child, sub);
                if (children == null) return null;
                if (result != null)
                {
                    if(!result.ContainsKey(parameterExpression.Name))
                    {
                        result.Add(parameterExpression.Name, sub);
                    }
                    else
                    {
                        var obj = result[parameterExpression.Name].AsObject();
                        foreach (var rec in sub)
                            obj.Add(rec.Key, rec.Value.ToString());
                    }
                }
                return children;
            }

            if (childJsonObject != null && result != null)
                result.Add(parameterExpression.Name, JsonNode.Parse(childJsonObject.ToJsonString()));
            return childJsonObject;
        };
        var jsonObject = jsonNode.AsObject();
        return getChild(jsonObject);
    }

    public static JsonNode Filter(this JsonNode jsonNode, SCIMComparisonExpression comparisonExpression)
    {
        var isJsonObject = jsonNode is JsonObject;
        var isJsonArray = jsonNode is JsonArray;
        Func<JsonNode, JsonNode> filter = (record) =>
        {
            var isCorrect = false;
            var valueObject = record;
            if (comparisonExpression.LeftExpression != null)
                valueObject = valueObject.Filter(comparisonExpression.LeftExpression);
            switch (comparisonExpression.ComparisonOperator)
            {
                case SCIMComparisonOperators.NE:
                    isCorrect = !valueObject.ToString().Equals(comparisonExpression.Value, StringComparison.InvariantCultureIgnoreCase);
                    break;
                case SCIMComparisonOperators.GT:
                    {
                        if (int.TryParse(valueObject.ToString(), out int i))
                            isCorrect = i > int.Parse(comparisonExpression.Value);
                        if (DateTime.TryParse(valueObject.ToString(), out DateTime dt))
                            isCorrect = dt > DateTime.Parse(comparisonExpression.Value);
                        if (decimal.TryParse(valueObject.ToString(), out decimal dc))
                            isCorrect = dc > decimal.Parse(comparisonExpression.Value);
                    }
                    break;
                case SCIMComparisonOperators.GE:
                    {
                        if (int.TryParse(valueObject.ToString(), out int i))
                            isCorrect = i >= int.Parse(comparisonExpression.Value);
                        if (DateTime.TryParse(valueObject.ToString(), out DateTime dt))
                            isCorrect = dt >= DateTime.Parse(comparisonExpression.Value);
                        if (decimal.TryParse(valueObject.ToString(), out decimal dc))
                            isCorrect = dc >= decimal.Parse(comparisonExpression.Value);
                    }
                    break;
                case SCIMComparisonOperators.LE:
                    {
                        if (int.TryParse(valueObject.ToString(), out int i))
                            isCorrect = i <= int.Parse(comparisonExpression.Value);
                        if (DateTime.TryParse(valueObject.ToString(), out DateTime dt))
                            isCorrect = dt <= DateTime.Parse(comparisonExpression.Value);
                        if (decimal.TryParse(valueObject.ToString(), out decimal dc))
                            isCorrect = dc <= decimal.Parse(comparisonExpression.Value);
                    }
                    break;
                case SCIMComparisonOperators.LT:
                    {
                        if (int.TryParse(valueObject.ToString(), out int i))
                            isCorrect = i < int.Parse(comparisonExpression.Value);
                        if (DateTime.TryParse(valueObject.ToString(), out DateTime dt))
                            isCorrect = dt < DateTime.Parse(comparisonExpression.Value);
                        if (decimal.TryParse(valueObject.ToString(), out decimal dc))
                            isCorrect = dc < decimal.Parse(comparisonExpression.Value);
                    }
                    break;
                case SCIMComparisonOperators.EQ:
                    isCorrect = valueObject.ToString().Equals(comparisonExpression.Value, StringComparison.InvariantCultureIgnoreCase);
                    break;
                case SCIMComparisonOperators.SW:
                    isCorrect = valueObject.ToString().StartsWith(comparisonExpression.Value, StringComparison.InvariantCultureIgnoreCase);
                    break;
                case SCIMComparisonOperators.EW:
                    isCorrect = valueObject.ToString().EndsWith(comparisonExpression.Value, StringComparison.InvariantCultureIgnoreCase);
                    break;
                case SCIMComparisonOperators.CO:
                    var arr = valueObject.AsArray();
                    if (arr != null)
                    {
                        isCorrect = arr.Select(a => a.ToString()).Any(c => c.Equals(comparisonExpression.Value, StringComparison.InvariantCultureIgnoreCase));
                    }
                    else
                    {
                        isCorrect = valueObject.ToString().Contains(comparisonExpression.Value, StringComparison.InvariantCultureIgnoreCase);
                    }
                    break;
            }

            if (!isCorrect) return null;
            return record;
        };

        if (isJsonObject)
        {
            return filter(jsonNode);
        }

        if (isJsonArray)
        {
            var result = new JsonArray();
            foreach (var record in jsonNode.AsArray())
            {
                var val = record.Filter(comparisonExpression);
                if (val != null) result.Add(JsonNode.Parse(val.ToJsonString()));
            }

            return result;
        }

        return null;
    }
}
