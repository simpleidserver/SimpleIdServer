// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Scim.Serialization;

public class SCIMSerializer
{
    public JsonObject Serialize(object instance)
    {
        var type = instance.GetType();
        var scimSchemaAttributes = type.GetCustomAttributes(typeof(SCIMSchemaAttribute), false);
        if (!scimSchemaAttributes.Any())
        {
            throw new InvalidOperationException(Global.MissingSCIMSchemaAttribute);
        }

        var scimSchemaAttribute = (SCIMSchemaAttribute)scimSchemaAttributes.First();
        var jObj = new JsonObject
        {
            { StandardSCIMRepresentationAttributes.Schemas, new JsonArray(scimSchemaAttribute.Schemas.Select(s => JsonValue.Create(s)).ToArray()) }
        };
        Serialize(instance, jObj);
        return jObj;
    }

    private static void Serialize(object instance, JsonObject jObj)
    {
        var type = instance.GetType();
        foreach (var property in type.GetProperties())
        {
            var properties = property.GetCustomAttributes(typeof(SCIMSchemaPropertyAttribute), false);
            if (!properties.Any())
            {
                continue;
            }

            var obj = property.GetValue(instance);
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
            {
                continue;
            }

            var propAttr = (SCIMSchemaPropertyAttribute)properties.First();
            if (property.PropertyType.IsGenericType && typeof(ICollection<>).IsAssignableFrom(property.PropertyType.GetGenericTypeDefinition()) || property.PropertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)))
            {
                var arr = (IEnumerable<object>)obj;
                var genericArgument = property.PropertyType.GetGenericArguments().First();
                if (genericArgument.IsGenericType || genericArgument == typeof(string))
                {
                    jObj.Add(propAttr.Name, new JsonArray(arr.Select(a => JsonNode.Parse(JsonSerializer.Serialize(a))).ToArray()));
                }
                else
                {
                    var jArr = new JsonArray();
                    foreach(var record in arr)
                    {
                        var newObj = new JsonObject();
                        Serialize(record, newObj);
                        jArr.Add(newObj);
                    }

                    jObj.Add(propAttr.Name, jArr);
                }
            }
            else
            {
                jObj.Add(propAttr.Name, property.GetValue(instance).ToString());
            }
        }
    }
}
