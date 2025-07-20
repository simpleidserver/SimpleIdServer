// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Domains.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Scim;

public class SCIMSchemaExtractor
{
    public static SCIMSchema Extract(string filePath, string resourceType, bool isRootSchema = false)
    {
        var content = File.ReadAllText(filePath);
        var jObj = JsonSerializer.Deserialize<JsonObject>(content, new JsonSerializerOptions
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
        });
        var builder = SCIMSchemaBuilder.Create(jObj[StandardSCIMRepresentationAttributes.Id].ToString(), 
            jObj[StandardSCIMRepresentationAttributes.Name].ToString(), 
            resourceType, 
            jObj[StandardSCIMRepresentationAttributes.Description].ToString(), 
            isRootSchema);
        var attributes = jObj[StandardSCIMRepresentationAttributes.Attributes] as JsonArray;
        foreach(JsonObject attribute in attributes)
        {
            JsonArray subAttributes = null;
            if (attribute.ContainsKey(StandardSCIMRepresentationAttributes.SubAttributes))
            {
                subAttributes = attribute[StandardSCIMRepresentationAttributes.SubAttributes] as JsonArray;
            }

            builder.AddAttribute(
                ExtractString(attribute, StandardSCIMRepresentationAttributes.Name),
                ExtractEnum<SCIMSchemaAttributeTypes>(attribute, StandardSCIMRepresentationAttributes.Type),
                multiValued: ExtractBoolean(attribute, StandardSCIMRepresentationAttributes.MultiValued),
                required: ExtractBoolean(attribute, StandardSCIMRepresentationAttributes.Required),
                caseExact: ExtractBoolean(attribute, StandardSCIMRepresentationAttributes.CaseExact),
                mutability: ExtractEnum<SCIMSchemaAttributeMutabilities>(attribute, StandardSCIMRepresentationAttributes.Mutability),
                returned: ExtractEnum<SCIMSchemaAttributeReturned>(attribute, StandardSCIMRepresentationAttributes.Returned),
                uniqueness: ExtractEnum<SCIMSchemaAttributeUniqueness>(attribute, StandardSCIMRepresentationAttributes.Uniqueness),
                description: ExtractString(attribute, StandardSCIMRepresentationAttributes.Description),
                canonicalValues: ExtractList(attribute, StandardSCIMRepresentationAttributes.CanonicalValues),
                callback: (c) =>
                {
                    if (subAttributes == null)
                    {
                        return;
                    }

                    foreach(JsonObject subAttr in subAttributes)
                    {
                        c.AddAttribute(
                            builder.Build(),
                            ExtractString(subAttr, StandardSCIMRepresentationAttributes.Name),
                            ExtractEnum<SCIMSchemaAttributeTypes>(subAttr, StandardSCIMRepresentationAttributes.Type),
                            multiValued: ExtractBoolean(subAttr, StandardSCIMRepresentationAttributes.MultiValued),
                            required: ExtractBoolean(subAttr, StandardSCIMRepresentationAttributes.Required),
                            caseExact: ExtractBoolean(subAttr, StandardSCIMRepresentationAttributes.CaseExact),
                            mutability: ExtractEnum<SCIMSchemaAttributeMutabilities>(subAttr, StandardSCIMRepresentationAttributes.Mutability),
                            returned: ExtractEnum<SCIMSchemaAttributeReturned>(subAttr, StandardSCIMRepresentationAttributes.Returned),
                            uniqueness: ExtractEnum<SCIMSchemaAttributeUniqueness>(subAttr, StandardSCIMRepresentationAttributes.Uniqueness),
                            description: ExtractString(subAttr, StandardSCIMRepresentationAttributes.Description),
                            canonicalValues: ExtractList(subAttr, StandardSCIMRepresentationAttributes.CanonicalValues)
                        );
                    }
                }
            );
        }

        return builder.Build();
    }

    private static List<string> ExtractList(JsonObject jObj, string name)
    {
        if (!jObj.ContainsKey(name))
        {
            return null;
        }

        var result = jObj[name].AsArray().OfType<JsonValue>().Select(v => v.GetValue<string>());
        return result.ToList();
    }

    private static T ExtractEnum<T>(JsonObject jObj, string name) where T : struct
    {
        var value = ExtractString(jObj, name);
        if (string.IsNullOrWhiteSpace(value))
        {
            return default(T);
        }

        T result;
        if (!Enum.TryParse(value.ToLowerInvariant(), true, out result))
        {
            return default(T);
        }

        return result;
    }

    private static bool ExtractBoolean(JsonObject jObj, string name)
    {
        var value = ExtractString(jObj, name);
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var result = false;
        if (!bool.TryParse(value, out result))
        {
            return false;
        }

        return result;
    }

    private static string ExtractString(JsonObject jObj, string name)
    {
        if (!jObj.ContainsKey(name))
        {
            return null;
        }


        return jObj[name].ToString();
    }
}
