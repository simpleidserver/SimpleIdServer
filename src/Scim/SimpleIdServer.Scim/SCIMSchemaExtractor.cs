// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Builder;
using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleIdServer.Scim
{
    public class SCIMSchemaExtractor
    {
        public static SCIMSchema Extract(string filePath, string resourceType)
        {
            var content = File.ReadAllText(filePath);
            var jObj = JsonConvert.DeserializeObject<JObject>(content);
            var builder = SCIMSchemaBuilder.Create(jObj[SCIMConstants.StandardSCIMRepresentationAttributes.Id].ToString(), 
                jObj[SCIMConstants.StandardSCIMRepresentationAttributes.Name].ToString(), 
                resourceType, 
                jObj[SCIMConstants.StandardSCIMRepresentationAttributes.Description].ToString());
            var attributes = jObj[SCIMConstants.StandardSCIMRepresentationAttributes.Attributes] as JArray;
            foreach(JObject attribute in attributes)
            {
                JArray subAttributes = null;
                if (attribute.ContainsKey(SCIMConstants.StandardSCIMRepresentationAttributes.SubAttributes))
                {
                    subAttributes = attribute[SCIMConstants.StandardSCIMRepresentationAttributes.SubAttributes] as JArray;
                }

                builder.AddAttribute(
                    ExtractString(attribute, SCIMConstants.StandardSCIMRepresentationAttributes.Name),
                    ExtractEnum<SCIMSchemaAttributeTypes>(attribute, SCIMConstants.StandardSCIMRepresentationAttributes.Type),
                    multiValued: ExtractBoolean(attribute, SCIMConstants.StandardSCIMRepresentationAttributes.MultiValued),
                    required: ExtractBoolean(attribute, SCIMConstants.StandardSCIMRepresentationAttributes.Required),
                    caseExact: ExtractBoolean(attribute, SCIMConstants.StandardSCIMRepresentationAttributes.CaseExact),
                    mutability: ExtractEnum<SCIMSchemaAttributeMutabilities>(attribute, SCIMConstants.StandardSCIMRepresentationAttributes.Mutability),
                    returned: ExtractEnum<SCIMSchemaAttributeReturned>(attribute, SCIMConstants.StandardSCIMRepresentationAttributes.Returned),
                    uniqueness: ExtractEnum<SCIMSchemaAttributeUniqueness>(attribute, SCIMConstants.StandardSCIMRepresentationAttributes.Uniqueness),
                    description: ExtractString(attribute, SCIMConstants.StandardSCIMRepresentationAttributes.Description),
                    canonicalValues: ExtractList(attribute, SCIMConstants.StandardSCIMRepresentationAttributes.CanonicalValues),
                    callback: (c) =>
                    {
                        if (subAttributes == null)
                        {
                            return;
                        }

                        foreach(JObject subAttr in subAttributes)
                        {
                            c.AddAttribute(
                                ExtractString(subAttr, SCIMConstants.StandardSCIMRepresentationAttributes.Name),
                                ExtractEnum<SCIMSchemaAttributeTypes>(subAttr, SCIMConstants.StandardSCIMRepresentationAttributes.Type),
                                multiValued: ExtractBoolean(subAttr, SCIMConstants.StandardSCIMRepresentationAttributes.MultiValued),
                                required: ExtractBoolean(subAttr, SCIMConstants.StandardSCIMRepresentationAttributes.Required),
                                caseExact: ExtractBoolean(subAttr, SCIMConstants.StandardSCIMRepresentationAttributes.CaseExact),
                                mutability: ExtractEnum<SCIMSchemaAttributeMutabilities>(subAttr, SCIMConstants.StandardSCIMRepresentationAttributes.Mutability),
                                returned: ExtractEnum<SCIMSchemaAttributeReturned>(subAttr, SCIMConstants.StandardSCIMRepresentationAttributes.Returned),
                                uniqueness: ExtractEnum<SCIMSchemaAttributeUniqueness>(subAttr, SCIMConstants.StandardSCIMRepresentationAttributes.Uniqueness),
                                description: ExtractString(subAttr, SCIMConstants.StandardSCIMRepresentationAttributes.Description),
                                canonicalValues: ExtractList(subAttr, SCIMConstants.StandardSCIMRepresentationAttributes.CanonicalValues)
                            );
                        }
                    }
                );
            }

            return builder.Build();
        }

        private static List<string> ExtractList(JObject jObj, string name)
        {
            if (!jObj.ContainsKey(name))
            {
                return null;
            }

            var result = jObj[name].Values<string>();
            return result.ToList();
        }

        private static T ExtractEnum<T>(JObject jObj, string name) where T : struct
        {
            var value = ExtractString(jObj, name);
            if (string.IsNullOrWhiteSpace(value))
            {
                return default(T);
            }

            T result;
            if (!Enum.TryParse(value.ToLowerInvariant(), out result))
            {
                return default(T);
            }

            return result;
        }

        private static bool ExtractBoolean(JObject jObj, string name)
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

        private static string ExtractString(JObject jObj, string name)
        {
            if (!jObj.ContainsKey(name))
            {
                return null;
            }


            return jObj[name].ToString();
        }
    }
}
