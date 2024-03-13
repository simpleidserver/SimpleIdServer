// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Domains;

public class PresentationDefinitionInputDescriptorConstraint
{
    /// <summary>
    /// . The value of this property MUST be an array of one or more JSONPath string expressions (as defined in the JSONPath Syntax Definition section) that select a target value from the input.
    /// </summary>
    public List<string> Path { get; set; } = new List<string>();
    /// <summary>
    /// if present its value MUST be a JSON Schema descriptor used to filter against the values returned from evaluation of the JSONPath string expressions in the path array.
    /// </summary>
    public string Filter { get; set; }

    public static JsonObject Serialize(List<PresentationDefinitionInputDescriptorConstraint> constraints)
    {
        var result = new JsonObject();
        var fields = new JsonArray();
        result.Add("fields", fields);
        foreach(var constraint in constraints)
        {
            var record = new JsonObject();
            if (constraint.Path != null && constraint.Path.Any())
            {
                var jArr = new JsonArray();
                foreach (var p in constraint.Path)
                    jArr.Add(p);
                record.Add("path", jArr);
            }

            if (!string.IsNullOrWhiteSpace(constraint.Filter))
                record.Add("filter", JsonObject.Parse(constraint.Filter));

            fields.Add(record);
        }

        return result;
    }
}