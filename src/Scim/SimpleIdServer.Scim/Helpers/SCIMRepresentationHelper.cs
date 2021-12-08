// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
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

        public SCIMRepresentation ExtractSCIMRepresentationFromJSON(JObject json, string externalId, SCIMSchema mainSchema, ICollection<SCIMSchema> extensionSchemas)
        {
            CheckRequiredAttributes(mainSchema, extensionSchemas, json);
            return BuildRepresentation(json, externalId, mainSchema, extensionSchemas, _options.IgnoreUnsupportedCanonicalValues);
        }

        public static SCIMRepresentation BuildRepresentation(JObject json, string externalId, SCIMSchema mainSchema, ICollection<SCIMSchema> extensionSchemas, bool ignoreUnsupportedCanonicalValues)
        {
            var schemas = new List<SCIMSchema>
            {
                mainSchema
            };
            schemas.AddRange(extensionSchemas);
            var result = new SCIMRepresentation
            {
                ExternalId = externalId,
                Schemas = schemas
            };
            result.Schemas = schemas;
            var resolutionResult = Resolve(json, mainSchema, extensionSchemas);
            result.FlatAttributes = BuildRepresentationAttributes(resolutionResult, resolutionResult.AllSchemaAttributes, ignoreUnsupportedCanonicalValues);
            var attr = result.FlatAttributes.FirstOrDefault(a => a.SchemaAttribute.Name == "displayName");
            if (attr != null)
            {
                result.DisplayName = attr.ValueString;
            }

            return result;
        }

        public static ICollection<SCIMRepresentationAttribute> BuildRepresentationAttributes(ResolutionResult resolutionResult, ICollection<SCIMSchemaAttribute> allSchemaAttributes, bool ignoreUnsupportedCanonicalValues, bool ignoreDefaultAttrs = false)
        {
            var attributes = new List<SCIMRepresentationAttribute>();
            foreach(var record in resolutionResult.Rows)
            {
                if (record.SchemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.READONLY)
                {
                    continue;
                }

                // Add attribute
                if (record.SchemaAttribute.MultiValued)
                {
                    var jArr = record.Content as JArray;
                    if (jArr == null)
                    {
                        throw new SCIMSchemaViolatedException(string.Format(Global.AttributeIsNotArray, record.SchemaAttribute.Name));
                    }


                    attributes.AddRange(BuildAttributes(jArr, record.SchemaAttribute, record.Schema, ignoreUnsupportedCanonicalValues));
                }
                else
                {
                    var jArr = new JArray();
                    jArr.Add(record.Content);
                    attributes.AddRange(BuildAttributes(jArr, record.SchemaAttribute, record.Schema, ignoreUnsupportedCanonicalValues));
                }
            }

            if (ignoreDefaultAttrs)
            {
                return attributes;
            }

            var defaultAttributes = allSchemaAttributes.Where(a => !attributes.Any(at => at.SchemaAttribute.Name == a.Name) && a.Mutability == SCIMSchemaAttributeMutabilities.READWRITE);
            foreach (var defaultAttr in defaultAttributes)
            {
                var attributeId = Guid.NewGuid().ToString();
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
                                attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attributeId, defaultAttr, valueString: str));
                            }
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
                                attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attributeId, defaultAttr, valueInteger: i));
                            }
                        }

                        break;
                }
            }

            return attributes;
        }

        public static ICollection<SCIMRepresentationAttribute> BuildAttributes(JArray jArr, SCIMSchemaAttribute schemaAttribute, SCIMSchema schema, bool ignoreUnsupportedCanonicalValues)
        {
            var result = new List<SCIMRepresentationAttribute>();
            if (schemaAttribute.Mutability == SCIMSchemaAttributeMutabilities.READONLY)
            {
                return result;
            }

            var attributeId = Guid.NewGuid().ToString();
            if (schemaAttribute.Type == SCIMSchemaAttributeTypes.COMPLEX)
            {
                if (!jArr.Any())
                {
                    result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attributeId, schemaAttribute));
                }
                else
                {
                    foreach (var jsonProperty in jArr)
                    {
                        var rec = jsonProperty as JObject;
                        if (rec == null)
                        {
                            throw new SCIMSchemaViolatedException(string.Format(Global.NotValidJSON, jsonProperty.ToString()));
                        }

                        var subAttributes = schema.GetChildren(schemaAttribute).ToList();
                        CheckRequiredAttributes(schema, subAttributes, rec);
                        var resolutionResult = Resolve(rec, schema, subAttributes);
                        var parent = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attributeId, schemaAttribute);
                        var children = BuildRepresentationAttributes(resolutionResult, subAttributes, ignoreUnsupportedCanonicalValues);
                        foreach(var child in children)
                        {
                            if (SCIMRepresentation.GetParentPath(child.FullPath) == parent.FullPath)
                            {
                                child.ParentAttributeId = parent.Id;
                            }

                            result.Add(child);
                        }

                        result.Add(parent);
                    }
                }
            }
            else
            {
                switch(schemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        var valuesBooleanResult = Extract<bool>(jArr);
                        if (valuesBooleanResult.InvalidValues.Any())
                        {
                            throw new SCIMSchemaViolatedException(string.Format(Global.NotValidBoolean, string.Join(",", valuesBooleanResult.InvalidValues)));
                        }

                        foreach(var b in valuesBooleanResult.Values)
                        {
                            var record = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attributeId, schemaAttribute, valueBoolean: b);
                            result.Add(record);
                        }
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        var valuesIntegerResult = Extract<int>(jArr);
                        if (valuesIntegerResult.InvalidValues.Any())
                        {
                            throw new SCIMSchemaViolatedException(string.Format(Global.NotValidInteger, string.Join(",", valuesIntegerResult.InvalidValues)));
                        }

                        foreach (var i in valuesIntegerResult.Values)
                        {
                            var record = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attributeId, schemaAttribute, valueInteger: i);
                            result.Add(record);
                        }
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        var valuesDateTimeResult = Extract<DateTime>(jArr);
                        if (valuesDateTimeResult.InvalidValues.Any())
                        {
                            throw new SCIMSchemaViolatedException(string.Format(Global.NotValidDateTime, string.Join(",", valuesDateTimeResult.InvalidValues)));
                        }

                        foreach (var d in valuesDateTimeResult.Values)
                        {
                            var record = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attributeId, schemaAttribute, valueDateTime: d);
                            result.Add(record);
                        }
                        break;
                    case SCIMSchemaAttributeTypes.STRING:
                        var strs = jArr.Select(j => j.ToString()).ToList();
                        if (schemaAttribute.CanonicalValues != null
                            && schemaAttribute.CanonicalValues.Any()
                            && !ignoreUnsupportedCanonicalValues
                            && !strs.All(_ => schemaAttribute.CaseExact ?
                                schemaAttribute.CanonicalValues.Contains(_)
                                : schemaAttribute.CanonicalValues.Contains(_, StringComparer.OrdinalIgnoreCase))
                        )
                        {
                            throw new SCIMSchemaViolatedException(string.Format(Global.NotValidCanonicalValue, schemaAttribute.Name));
                        }

                        foreach (var s in strs)
                        {
                            var record = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attributeId, schemaAttribute, valueString: s);
                            result.Add(record);
                        }
                        break;
                    case SCIMSchemaAttributeTypes.REFERENCE:
                        var refs = jArr.Select(j => j.ToString()).ToList();
                        foreach (var reference in refs)
                        {
                            var record = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attributeId, schemaAttribute, valueReference: reference);
                            result.Add(record);
                        }
                        break;
                    case SCIMSchemaAttributeTypes.DECIMAL:
                        var valuesDecimalResult = Extract<decimal>(jArr);
                        if (valuesDecimalResult.InvalidValues.Any())
                        {
                            throw new SCIMSchemaViolatedException(string.Format(Global.NotValidDecimal, string.Join(",", valuesDecimalResult.InvalidValues)));
                        }

                        foreach (var d in valuesDecimalResult.Values)
                        {
                            var record = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attributeId, schemaAttribute, valueDecimal: d);
                            result.Add(record);
                        }
                        break;
                    case SCIMSchemaAttributeTypes.BINARY:
                        var invalidValues = new List<string>();
                        var valuesBinary = new List<string>();
                        foreach(var rec in jArr)
                        {
                            try
                            {
                                Convert.FromBase64String(rec.ToString());
                                valuesBinary.Add(rec.ToString());
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

                        foreach (var b in valuesBinary)
                        {
                            var record = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), attributeId, schemaAttribute, valueBinary: b);
                            result.Add(record);
                        }
                        break;
                }
            }

            return result;
        }

        #region Resolve attributes

        public static ResolutionResult Resolve(JObject json, SCIMSchema mainSchema, ICollection<SCIMSchema> extensionSchemas)
        {
            var rows = new List<ResolutionRowResult>();
            var schemas = new List<SCIMSchema>
            {
                mainSchema
            };
            schemas.AddRange(extensionSchemas);
            foreach (var kvp in json)
            {
                if (kvp.Key == SCIMConstants.StandardSCIMRepresentationAttributes.Schemas || SCIMConstants.StandardSCIMCommonRepresentationAttributes.Contains(kvp.Key))
                {
                    continue;
                }

                if (extensionSchemas.Any(s => kvp.Key.StartsWith(s.Id, StringComparison.InvariantCultureIgnoreCase)))
                {
                    rows.AddRange(ResolveFullQualifiedName(kvp, extensionSchemas));
                    continue;
                }

                rows.Add(Resolve(kvp, schemas));
            }

            return new ResolutionResult(schemas, rows);
        }

        public static ResolutionResult Resolve(JObject json, SCIMSchema schema, ICollection<SCIMSchemaAttribute> schemaAttributes)
        {
            var rows = new List<ResolutionRowResult>();
            foreach(var kvp in json)
            {
                rows.Add(Resolve(kvp, schema, schemaAttributes));
            }

            return new ResolutionResult(rows);
        }

        private static ResolutionRowResult Resolve(KeyValuePair<string, JToken> kvp, ICollection<SCIMSchema> allSchemas)
        {
            var schema = allSchemas.FirstOrDefault(s => s.Attributes.Any(at => at.FullPath == kvp.Key));
            if (schema == null)
            {
                throw new SCIMSchemaViolatedException(string.Format(Global.AttributeIsNotRecognirzed, kvp.Key));
            }

            return new ResolutionRowResult(schema, schema.Attributes.First(at => at.FullPath == kvp.Key), kvp.Value);
        }

        private static ICollection<ResolutionRowResult> ResolveFullQualifiedName(KeyValuePair<string, JToken> kvp, ICollection<SCIMSchema> extensionSchemas)
        {
            var jObj = kvp.Value as JObject;
            if (jObj == null)
            {
                throw new SCIMSchemaViolatedException(string.Format(Global.PropertyCannotContainsArray, kvp.Key));
            }

            var result = new List<ResolutionRowResult>();
            var schema = extensionSchemas.First(e => kvp.Key == e.Id);
            foreach (var skvp in jObj)
            {
                var attrSchema = schema.Attributes.FirstOrDefault(a => a.Name == skvp.Key);
                if (attrSchema == null)
                {
                    throw new SCIMSchemaViolatedException(string.Format(Global.AttributeIsNotRecognirzed, skvp.Key));
                }

                result.Add(new ResolutionRowResult(schema, attrSchema, skvp.Value));
            }

            return result;
        }

        private static ResolutionRowResult Resolve(KeyValuePair<string, JToken> kvp, SCIMSchema schema, ICollection<SCIMSchemaAttribute> schemaAttributes)
        {
            var attrSchema = schemaAttributes.FirstOrDefault(a => a.Name == kvp.Key);
            if (attrSchema == null)
            {
                throw new SCIMSchemaViolatedException(string.Format(Global.AttributeIsNotRecognirzed, kvp.Key));
            }

            return new ResolutionRowResult(schema, attrSchema, kvp.Value);
        }

        #endregion

        #region Check required attributes

        private static void CheckRequiredAttributes(SCIMSchema mainSchema, ICollection<SCIMSchema> extensionSchemas, JObject json)
        {
            CheckRequiredAttributes(mainSchema, json);
            foreach(var extensionSchema in extensionSchemas)
            {
                CheckRequiredAttributes(extensionSchema, json);
            }
        }

        private static void CheckRequiredAttributes(SCIMSchema schema, JObject json)
        {
            var attributes = schema.HierarchicalAttributes.Select(h => h.Leaf);
            CheckRequiredAttributes(schema, attributes, json);
        }

        private static void CheckRequiredAttributes(SCIMSchema schema, IEnumerable<SCIMSchemaAttribute> schemaAttributes, JObject json)
        {
            var missingRequiredAttributes = schemaAttributes.Where(a => a.Required && !json.HasNotEmptyElement(a.Name, schema.Id));
            if (missingRequiredAttributes.Any())
            {
                throw new SCIMSchemaViolatedException(string.Format(Global.RequiredAttributesAreMissing, string.Join(",", missingRequiredAttributes.Select(a => $"{schema.Id}:{a.Name}"))));
            }
        }

        #endregion

        public class ResolutionRowResult
        {
            public ResolutionRowResult(SCIMSchemaAttribute schemaAttribute, JToken content)
            {
                SchemaAttribute = schemaAttribute;
                Content = content;
            }

            public ResolutionRowResult(SCIMSchema schema, SCIMSchemaAttribute schemaAttribute, JToken content) : this(schemaAttribute, content)
            {
                Schema = schema;
            }

            public SCIMSchema Schema { get; set; }
            public SCIMSchemaAttribute SchemaAttribute { get; set; }
            public JToken Content { get; set; }
        }

        public class ResolutionResult
        {
            public ResolutionResult(ICollection<ResolutionRowResult> rows)
            {
                Rows = rows;
            }

            public ResolutionResult(ICollection<SCIMSchema> schemas, ICollection<ResolutionRowResult> rows) : this(rows)
            {
                Schemas = schemas;
                Rows = rows;
            }

            public ICollection<SCIMSchema> Schemas { get; set; }
            public ICollection<ResolutionRowResult> Rows { get; set; }
            public ICollection<SCIMSchemaAttribute> AllSchemaAttributes
            {
                get
                {
                    return Schemas.SelectMany(s => s.HierarchicalAttributes).Select(h => h.Leaf).ToList();
                }
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
