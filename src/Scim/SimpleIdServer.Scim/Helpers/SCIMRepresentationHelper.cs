// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Helpers
{
    public class SCIMRepresentationHelper : ISCIMRepresentationHelper
    {
        public SCIMRepresentation ExtractSCIMRepresentationFromJSON(JObject json, ICollection<SCIMSchema> schemas)
        {
            var attrsSchema = schemas.SelectMany(s => s.Attributes);
            var missingRequiredAttributes = attrsSchema.Where(a => a.Required && !json.ContainsKey(a.Name));
            if (missingRequiredAttributes.Any())
            {
                throw new SCIMSchemaViolatedException("missingRequiredAttribute", $"required attributes {string.Join(",", missingRequiredAttributes.Select(a => a.Name))} are missing");
            }

            return new SCIMRepresentation(schemas, BuildRepresentationAttributes(json, attrsSchema));
        }

        public static ICollection<SCIMRepresentationAttribute> BuildRepresentationAttributes(JObject json, IEnumerable<SCIMSchemaAttribute> attrsSchema)
        {
            var result = new List<SCIMRepresentationAttribute>();
            foreach (var jsonProperty in json)
            {
                if (jsonProperty.Key == SCIMConstants.StandardSCIMRepresentationAttributes.Schemas)
                {
                    continue;
                }

                var attrSchema = attrsSchema.FirstOrDefault(a => a.Name == jsonProperty.Key);
                if (attrSchema == null)
                {
                    throw new SCIMSchemaViolatedException("unrecognizedAttribute", $"attribute {jsonProperty.Key} is not recognized by the SCIM schema");
                }

                if (attrSchema.Mutability == SCIMSchemaAttributeMutabilities.READONLY)
                {
                    continue;
                }

                if (attrSchema.MultiValued)
                {
                    var jArr = jsonProperty.Value as JArray;
                    if (jArr == null)
                    {
                        throw new SCIMSchemaViolatedException("badFormatAttribute", $"attribute {jsonProperty.Key} is not an array");
                    }

                    foreach (var subJson in jArr)
                    {
                        result.Add(BuildAttribute(subJson, attrSchema));
                    }
                }
                else
                {
                    result.Add(BuildAttribute(jsonProperty.Value, attrSchema));
                }
            }

            var defaultAttributes = attrsSchema.Where(a => !json.ContainsKey(a.Name) && a.Mutability == SCIMSchemaAttributeMutabilities.READWRITE);
            foreach (var defaultAttr in defaultAttributes)
            {
                var attr = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), defaultAttr);
                switch (defaultAttr.Type)
                {
                    case SCIMSchemaAttributeTypes.STRING:
                        if (defaultAttr.DefaultValueString.Any())
                        {
                            var defaultValueStr = defaultAttr.DefaultValueString;
                            if (!defaultAttr.MultiValued)
                            {
                                defaultValueStr = new List<string> { defaultValueStr.First() };
                            }

                            foreach (var str in defaultValueStr)
                            {
                                attr.Add(str);
                            }

                            result.Add(attr);
                        }

                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        if (defaultAttr.DefaultValueInt.Any())
                        {
                            var defaultValueInt = defaultAttr.DefaultValueInt;
                            if (!defaultAttr.MultiValued)
                            {
                                defaultValueInt = new List<int> { defaultValueInt.First() };
                            }

                            foreach (var i in defaultValueInt)
                            {
                                attr.Add(i);
                            }

                            result.Add(attr);
                        }

                        break;
                }
            }

            return result;
        }

        private static SCIMRepresentationAttribute BuildAttribute(JToken jsonProperty, SCIMSchemaAttribute schemaAttribute)
        {
            var result = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(),schemaAttribute);
            switch (schemaAttribute.Type)
            {
                case SCIMSchemaAttributeTypes.BOOLEAN:
                    result.Add(bool.Parse(jsonProperty.ToString()));
                    break;
                case SCIMSchemaAttributeTypes.INTEGER:
                    result.Add(int.Parse(jsonProperty.ToString()));
                    break;
                case SCIMSchemaAttributeTypes.DATETIME:
                    result.Add(DateTime.Parse(jsonProperty.ToString()));
                    break;
                case SCIMSchemaAttributeTypes.STRING:
                    result.Add(jsonProperty.ToString());
                    break;
                case SCIMSchemaAttributeTypes.COMPLEX:
                    result.Values = BuildRepresentationAttributes(jsonProperty as JObject, schemaAttribute.SubAttributes);
                    break;
                case SCIMSchemaAttributeTypes.REFERENCE:
                    // REFERENCE.
                    break;
            }

            return result;
        }
    }
}