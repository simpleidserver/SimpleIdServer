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
using System.Reflection;

namespace SimpleIdServer.Scim.Helpers
{
    public class SCIMRepresentationHelper : ISCIMRepresentationHelper
    {
        public readonly SCIMHostOptions _options;

        public SCIMRepresentationHelper(IOptions<SCIMHostOptions> options)
        {
            _options = options.Value;
        }

        public SCIMRepresentation ExtractSCIMRepresentationFromJSON(JObject json, string externalId, ICollection<SCIMSchema> schemas)
        {
            var attrsSchema = schemas.SelectMany(s => s.Attributes);
            CheckRequiredAttributes(attrsSchema, json);
            return BuildRepresentation(json, attrsSchema, externalId, schemas, _options.IgnoreUnsupportedCanonicalValues);
        }

        public static SCIMRepresentation BuildRepresentation(JObject json, IEnumerable<SCIMSchemaAttribute> attrsSchema, string externalId, ICollection<SCIMSchema> schemas, bool ignoreUnsupportedCanonicalValues)
        {
            var result = new SCIMRepresentation();
            result.ExternalId = externalId;
            result.Schemas = schemas;
            result.Attributes = BuildRepresentationAttributes(json, attrsSchema, ignoreUnsupportedCanonicalValues);
            return result;
        }

        public static ICollection<SCIMRepresentationAttribute> BuildRepresentationAttributes(JObject json, IEnumerable<SCIMSchemaAttribute> attrsSchema, bool ignoreUnsupportedCanonicalValues, bool ignoreDefaultAttrs = false)
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

            if (ignoreDefaultAttrs)
            {
                return attributes;
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

        public static ICollection<SCIMRepresentationAttribute> BuildAttributes(JArray jArr, SCIMSchemaAttribute schemaAttribute, bool ignoreUnsupportedCanonicalValues)
        {
            var result = new List<SCIMRepresentationAttribute>();
            if (schemaAttribute.Type == SCIMSchemaAttributeTypes.COMPLEX)
            {
                foreach(JObject jsonProperty in jArr)
                {
                    CheckRequiredAttributes(schemaAttribute.SubAttributes, jsonProperty);
                    var record = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute)
                    {
                        Values = BuildRepresentationAttributes(jsonProperty as JObject, schemaAttribute.SubAttributes, ignoreUnsupportedCanonicalValues)
                    };
                    
                    foreach (var subAttribute in record.Values)
                    {
                        subAttribute.Parent = record;
                    }
                    
                    result.Add(record);
                }
            }
            else
            {
                var record = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute);
                switch(schemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        var valuesBooleanResult = Extract<bool>(jArr);
                        if (valuesBooleanResult.InvalidValues.Any())
                        {
                            throw new SCIMSchemaViolatedException(string.Format(Global.NotValidBoolean, string.Join(",", valuesBooleanResult.InvalidValues)));
                        }

                        record.ValuesBoolean = valuesBooleanResult.Values;
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        var valuesIntegerResult = Extract<int>(jArr);
                        if (valuesIntegerResult.InvalidValues.Any())
                        {
                            throw new SCIMSchemaViolatedException(string.Format(Global.NotValidInteger, string.Join(",", valuesIntegerResult.InvalidValues)));
                        }

                        record.ValuesInteger = valuesIntegerResult.Values;
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        var valuesDateTimeResult = Extract<DateTime>(jArr);
                        if (valuesDateTimeResult.InvalidValues.Any())
                        {
                            throw new SCIMSchemaViolatedException(string.Format(Global.NotValidDateTime, string.Join(",", valuesDateTimeResult.InvalidValues)));
                        }

                        record.ValuesDateTime = valuesDateTimeResult.Values;
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
                    case SCIMSchemaAttributeTypes.DECIMAL:
                        var valuesDecimalResult = Extract<decimal>(jArr);
                        if (valuesDecimalResult.InvalidValues.Any())
                        {
                            throw new SCIMSchemaViolatedException(string.Format(Global.NotValidDecimal, string.Join(",", valuesDecimalResult.InvalidValues)));
                        }

                        record.ValuesDecimal = valuesDecimalResult.Values;
                        break;
                    case SCIMSchemaAttributeTypes.BINARY:
                        var invalidValues = new List<string>();
                        var valuesBinary = new List<byte[]>();
                        foreach(var rec in jArr)
                        {
                            try
                            {
                                valuesBinary.Add(Convert.FromBase64String(rec.ToString()));
                            }
                            catch(FormatException)
                            {
                                invalidValues.Add(rec.ToString());
                            }
                        }

                        if (invalidValues.Any())
                        {
                            throw new SCIMSchemaViolatedException(string.Format(Global.NotValidBase64, string.Join(",", invalidValues)));
                        }

                        record.ValuesBinary = valuesBinary;
                        break;
                }

                result.Add(record);
            }

            return result;
        }

        private static void CheckRequiredAttributes(IEnumerable<SCIMSchemaAttribute> attributes, JObject json)
        {
            var missingRequiredAttributes = attributes.Where(a => a.Required && !json.ContainsKey(a.Name));
            if (missingRequiredAttributes.Any())
            {
                throw new SCIMSchemaViolatedException(string.Format(Global.RequiredAttributesAreMissing, string.Join(",", missingRequiredAttributes.Select(a => a.Name))));
            }
        }

        private class ExtractionResult<T>
        {
            public ExtractionResult()
            {
                InvalidValues = new List<string>();
                Values = new List<T>();
            }

            public ICollection<string> InvalidValues { get; set; }
            public ICollection<T> Values { get; set; }
        }

        private static ExtractionResult<T> Extract<T>(JArray jArr) where T : struct
        {
            var result = new ExtractionResult<T>();
            var type = typeof(T);
            Type[] argTypes = { typeof(string), type.MakeByRefType() };
            var method = typeof(T).GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public, Type.DefaultBinder, argTypes, null);
            foreach(var record in jArr)
            {
                var parameters = new object[] { record.ToString(), null };
                var success = (bool)method.Invoke(null, parameters);
                if (!success)
                {
                    result.InvalidValues.Add(record.ToString());
                } 
                else
                {
                    var retVal = (T)parameters[1];
                    result.Values.Add(retVal);
                }
            }

            return result;
        }
    }
}
