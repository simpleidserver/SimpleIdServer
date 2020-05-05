// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Helpers
{
    public class SCIMRepresentationHelper : ISCIMRepresentationHelper
    {
        public readonly SCIMHostOptions _options;

        public SCIMRepresentationHelper(IOptions<SCIMHostOptions> options)
        {
            _options = options.Value;
        }

        public SCIMRepresentation ExtractSCIMRepresentationFromJSON(JObject json, ICollection<SCIMSchema> schemas)
        {
            var attrsSchema = schemas.SelectMany(s => s.Attributes);
            var missingRequiredAttributes = attrsSchema.Where(a => a.Required && !json.ContainsKey(a.Name));
            if (missingRequiredAttributes.Any())
            {
                throw new SCIMSchemaViolatedException(string.Format(Global.RequiredAttributesAreMissing, string.Join(",", missingRequiredAttributes.Select(a => a.Name))));
            }

            return BuildRepresentation(json, attrsSchema, schemas, _options.IgnoreUnsupportedCanonicalValues);
        }

        public static SCIMRepresentation BuildRepresentation(JObject json, IEnumerable<SCIMSchemaAttribute> attrsSchema, ICollection<SCIMSchema> schemas, bool ignoreUnsupportedCanonicalValues)
        {
            var result = new SCIMRepresentation();
            var externalId = json.SelectToken(SCIMConstants.StandardSCIMRepresentationAttributes.ExternalId);
            if (externalId != null)
            {
                result.ExternalId = externalId.ToString();
                json.Remove(SCIMConstants.StandardSCIMRepresentationAttributes.ExternalId);
            }

            result.Schemas = schemas;
            result.Attributes = BuildRepresentationAttributes(json, attrsSchema, ignoreUnsupportedCanonicalValues);
            return result;
        }

        public static ICollection<SCIMRepresentationAttribute> BuildRepresentationAttributes(JObject json, IEnumerable<SCIMSchemaAttribute> attrsSchema, bool ignoreUnsupportedCanonicalValues)
        {
            var attributes = new List<SCIMRepresentationAttribute>();
            foreach (var jsonProperty in json)
            {
                if (jsonProperty.Key == SCIMConstants.StandardSCIMRepresentationAttributes.Schemas || SCIMConstants.StandardSCIMCommonRepresentationAttributes.Contains(jsonProperty.Key))
                {
                    continue;
                }

                var attrSchema = attrsSchema.FirstOrDefault(a => a.Name == jsonProperty.Key);
                if (attrSchema == null)
                {
                    throw new SCIMSchemaViolatedException(string.Format(Global.AttributeIsNotRecognirzed, jsonProperty.Key));
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
                        throw new SCIMSchemaViolatedException(string.Format(Global.AttributeIsNotArray, jsonProperty.Key));
                    }


                    attributes.AddRange(BuildAttributes(jArr, attrSchema, ignoreUnsupportedCanonicalValues));
                }
                else
                {
                    var jArr = new JArray();
                    jArr.Add(jsonProperty.Value);
                    attributes.AddRange(BuildAttributes(jArr, attrSchema, ignoreUnsupportedCanonicalValues));
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

                            attributes.Add(attr);
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

                            attributes.Add(attr);
                        }

                        break;
                }
            }

            return attributes;
        }

        private static ICollection<SCIMRepresentationAttribute> BuildAttributes(JArray jArr, SCIMSchemaAttribute schemaAttribute, bool ignoreUnsupportedCanonicalValues)
        {
            var result = new List<SCIMRepresentationAttribute>();
            if (schemaAttribute.Type == SCIMSchemaAttributeTypes.COMPLEX)
            {
                foreach(var jsonProperty in jArr)
                {
                    var record = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute)
                    {
                        Values = BuildRepresentationAttributes(jsonProperty as JObject, schemaAttribute.SubAttributes, ignoreUnsupportedCanonicalValues)
                    };
                    result.Add(record);
                }
            }
            else
            {
                var record = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute);
                switch(schemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        record.ValuesBoolean = jArr.Select(j => bool.Parse(j.ToString())).ToList();
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        record.ValuesInteger = jArr.Select(j => int.Parse(j.ToString())).ToList();
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        record.ValuesDateTime = jArr.Select(j => DateTime.Parse(j.ToString())).ToList();
                        break;
                    case SCIMSchemaAttributeTypes.STRING:
                        record.ValuesString = jArr.Select(j => j.ToString()).ToList();
                        if (schemaAttribute.CanonicalValues != null && schemaAttribute.CanonicalValues.Any() && !ignoreUnsupportedCanonicalValues && !record.ValuesString.All(_ => schemaAttribute.CanonicalValues.Contains(_)))
                        {
                            throw new SCIMSchemaViolatedException(string.Format(Global.NotValidCanonicalValue, schemaAttribute.Name));
                        }

                        break;
                    case SCIMSchemaAttributeTypes.REFERENCE:
                        record.ValuesReference = jArr.Select(j => j.ToString()).ToList();
                        break;
                }

                result.Add(record);
            }

            return result;
        }
    }
}