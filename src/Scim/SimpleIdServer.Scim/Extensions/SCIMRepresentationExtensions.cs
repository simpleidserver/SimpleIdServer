// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Domain
{
    public static class SCIMRepresentationExtensions
    {
        public static void ApplyEmptyArray(this SCIMRepresentation representation)
        {
            var attrs = representation.Schemas.SelectMany(s => s.HierarchicalAttributes.Select(a => a.Leaf).Where(a => a.MultiValued));
            foreach (var attr in attrs)
            {
                if (!representation.Attributes.Any(a => a.SchemaAttribute.Name == attr.Name))
                {
                    representation.Attributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), attr));
                }
            }
        }

        public static List<SCIMPatchResult> ApplyPatches(this SCIMRepresentation representation,  ICollection<PatchOperationParameter> patches, bool ignoreUnsupportedCanonicalValues)
        {
            var result = new List<SCIMPatchResult>();
            foreach (var patch in patches)
            {
                var scimFilter = SCIMFilterParser.Parse(patch.Path, representation.Schemas);
                var schemaAttributes = representation.Schemas.SelectMany(_ => _.Attributes);
                List<SCIMRepresentationAttribute> attributes = null;
                string fullPath = null;
                if (scimFilter != null)
                {
                    var scimAttributeExpression = scimFilter as SCIMAttributeExpression;
                    if (scimAttributeExpression == null)
                    {
                        throw new SCIMAttributeException(Global.InvalidAttributeExpression);
                    }

                    fullPath = scimAttributeExpression.GetFullPath();
                    schemaAttributes = representation.Schemas
                        .Select(s => s.GetAttribute(fullPath))
                        .Where(s => s != null);

                    attributes = scimAttributeExpression.EvaluateAttributes(representation.HierarchicalAttributes.AsQueryable(), true).ToList();
                }
                else
                {
                    attributes = representation.HierarchicalAttributes.Select(h => h.Leaf).ToList();
                }

                var removeCallback = new Action<ICollection<SCIMRepresentationAttribute>>((attrs) =>
                {
                    foreach (var a in attrs)
                    {
                        var removedAttrs = representation.RemoveAttributeById(a);
                        foreach(var removedAttr in removedAttrs)
                        {
                            result.Add(new SCIMPatchResult { Attr = removedAttr, Operation = SCIMPatchOperations.REMOVE, Path = removedAttr.FullPath });
                        }
                    }
                });
                switch (patch.Operation)
                {
                    case SCIMPatchOperations.ADD:
                        try
                        {
                            if (schemaAttributes == null || !schemaAttributes.Any())
                            {
                                throw new SCIMNoTargetException(string.Format(Global.AttributeIsNotRecognirzed, patch.Path));
                            }

                            var newAttributes = ExtractRepresentationAttributesFromJSON(representation.Schemas, schemaAttributes.ToList(), patch.Value, ignoreUnsupportedCanonicalValues);
                            removeCallback(attributes.Where(a => !a.SchemaAttribute.MultiValued && a.FullPath == fullPath).ToList());
                            var isAttributeExits = !string.IsNullOrWhiteSpace(fullPath) && attributes.Any(a => a.FullPath == fullPath);
                            foreach (var newAttribute in newAttributes.OrderBy(a => a.GetLevel()))
                            {
                                var path = newAttribute.FullPath;
                                var schemaAttr = newAttribute.SchemaAttribute;
                                IEnumerable<SCIMRepresentationAttribute> parentAttributes = null;
                                if (fullPath == path)
                                {
                                    var attr = attributes.FirstOrDefault(a => a.FullPath == fullPath);
                                    if (attr != null)
                                    {
                                        var parent = representation.GetParentAttribute(attr);
                                        if (parent != null)
                                        {
                                            parentAttributes = new[] { parent };
                                        }
                                    }
                                    else
                                    {
                                        parentAttributes = representation.GetParentAttributesByPath(fullPath).ToList();
                                    }
                                }

                                if (schemaAttr.MultiValued && schemaAttr.Type != SCIMSchemaAttributeTypes.COMPLEX)
                                {
                                    var filteredAttribute = attributes.FirstOrDefault(_ => _.FullPath == path);
                                    if(filteredAttribute != null)
                                    {
                                        newAttribute.AttributeId = filteredAttribute.AttributeId;
                                    }

                                    representation.AddAttribute(newAttribute);
                                }
                                else if (parentAttributes != null && parentAttributes.Any())
                                {
                                    foreach(var parentAttribute in parentAttributes)
                                    {
                                        representation.AddAttribute(parentAttribute, newAttribute);
                                    }
                                }
                                else
                                {
                                    representation.Attributes.Add(newAttribute);
                                }

                                attributes.Add(newAttribute);
                                result.Add(new SCIMPatchResult { Attr = newAttribute, Operation = SCIMPatchOperations.ADD, Path = fullPath });
                            }
                        }
                        catch (SCIMSchemaViolatedException)
                        {
                            continue;
                        }
                        break;
                    case SCIMPatchOperations.REMOVE:
                        {
                            if (scimFilter == null)
                            {
                                throw new SCIMNoTargetException(string.Format(Global.InvalidPath, patch.Path));
                            }

                            removeCallback(attributes);
                        }
                        break;
                    case SCIMPatchOperations.REPLACE:
                        {
                            if (schemaAttributes == null || !schemaAttributes.Any())
                            {
                                throw new SCIMNoTargetException(string.Format(Global.AttributeIsNotRecognirzed, patch.Path));
                            }

                            var complexAttr = scimFilter as SCIMComplexAttributeExpression;
                            if (complexAttr != null && !attributes.Any() && complexAttr.GroupingFilter != null)
                            {
                                throw new SCIMNoTargetException(Global.PatchMissingAttribute);
                            }

                            try
                            {
                                List<SCIMRepresentationAttribute> parents = new List<SCIMRepresentationAttribute>();
                                if (complexAttr != null)
                                {
                                    var attr = attributes.First(a => a.FullPath == fullPath);
                                    var parent = string.IsNullOrWhiteSpace(attr.ParentAttributeId) ? attr : representation.GetParentAttribute(attributes.First(a => a.FullPath == fullPath));
                                    if (parent != null)
                                    {
                                        parents = new List<SCIMRepresentationAttribute> { parent };
                                    }
                                }
                                else
                                {
                                    parents = representation.GetParentAttributesByPath(fullPath).ToList();
                                }

                                if (scimFilter != null && parents.Any())
                                {
                                    foreach(var parent in parents)
                                    {
                                        var flatHiearchy = representation.GetFlatHierarchicalChildren(parent).ToList();
                                        var scimAttributeExpression = scimFilter as SCIMAttributeExpression;
                                        var newAttributes = ExtractRepresentationAttributesFromJSON(representation.Schemas, schemaAttributes.ToList(), patch.Value, ignoreUnsupportedCanonicalValues);
                                        foreach (var newAttribute in newAttributes.OrderBy(l => l.GetLevel()))
                                        {
                                            if (!flatHiearchy.Any(a => a.FullPath == newAttribute.FullPath))
                                            {
                                                var parentPath = SCIMRepresentation.GetParentPath(newAttribute.FullPath);
                                                var p = flatHiearchy.FirstOrDefault(a => a.FullPath == parentPath);
                                                if (p != null)
                                                {
                                                    representation.AddAttribute(p, newAttribute);
                                                }
                                                else
                                                {
                                                    representation.AddAttribute(newAttribute);
                                                }

                                                result.Add(new SCIMPatchResult { Attr = newAttribute, Operation = SCIMPatchOperations.ADD, Path = fullPath });
                                            }
                                        }

                                        result.AddRange(Merge(flatHiearchy, newAttributes, fullPath));
                                    }
                                }
                                else
                                {
                                    var newAttributes = ExtractRepresentationAttributesFromJSON(representation.Schemas, schemaAttributes.ToList(), patch.Value, ignoreUnsupportedCanonicalValues);
                                    var flatHiearchy = representation.Attributes.ToList();
                                    result.AddRange(Merge(flatHiearchy, newAttributes, fullPath));
                                }
                            }
                            catch (SCIMSchemaViolatedException)
                            {
                                continue;
                            }
                        }
                        break;
                }
            }

            var displayNameAttr = representation.Attributes.FirstOrDefault(a => a.FullPath == "displayName");
            if (displayNameAttr != null)
            {
                representation.SetDisplayName(displayNameAttr.ValueString);
            }

            return result;
        }

        public static JObject ToResponse(this SCIMRepresentation representation, string location, bool isGetRequest = false, bool includeStandardAttributes = true)
        {
            var jObj = new JObject
            {
                { SCIMConstants.StandardSCIMRepresentationAttributes.Id, representation.Id }
            };

            if (includeStandardAttributes)
            {
                representation.AddStandardAttributes(location);
            }

            var attributes = representation.HierarchicalAttributes.Select(a => a.Leaf).Select(a =>
            {
                var schema = representation.GetSchema(a);
                int order = 1;
                if(schema != null && schema.IsRootSchema)
                {
                    order = 0;
                }

                return new EnrichParameter(schema, order, a);
            });
            EnrichResponse(representation, attributes, jObj, isGetRequest);
            return jObj;
        }

        public static JObject ToResponseWithIncludedAttributes(this SCIMRepresentation representation, ICollection<SCIMExpression> includedAttributes, string location = null)
        {
            var clone = (SCIMRepresentation)representation.Clone();
            var filteredAttributes = new List<SCIMRepresentationAttribute>();
            clone.AddStandardAttributes(location);
            var attributes = clone.HierarchicalAttributes.AsQueryable();
            foreach (var includedAttribute in includedAttributes)
            {

                var attrs = (includedAttribute as SCIMAttributeExpression).EvaluateAttributes(attributes, false);
                filteredAttributes.AddRange(attrs);
            }

            clone.Attributes = filteredAttributes;
            return clone.ToResponse(location, true, false);
        }

        public static JObject ToResponseWithExcludedAttributes(this SCIMRepresentation representation, ICollection<SCIMExpression> excludedAttributes, string location = null)
        {
            var clone = (SCIMRepresentation)representation.Clone();
            clone.AddStandardAttributes(location);
            var attributes = clone.HierarchicalAttributes.AsQueryable();
            foreach (var excludedAttribute in excludedAttributes)
            {
                var attrs = (excludedAttribute as SCIMAttributeExpression).EvaluateAttributes(attributes, true);
                foreach(var attr in attrs)
                {
                    clone.RemoveAttributeById(attr);
                }
            }

            return clone.ToResponse(location, true, false);
        }

        private static List<SCIMPatchResult> Merge(List<SCIMRepresentationAttribute> attributes, ICollection<SCIMRepresentationAttribute> newAttributes, string fullPath)
        {
            var result = new List<SCIMPatchResult>();
            foreach(var attr in attributes)
            {
                var newAttr = newAttributes.FirstOrDefault(na => na.FullPath == attr.FullPath);
                if (newAttr == null)
                {
                    continue;
                }

                attr.ValueString = newAttr.ValueString;
                attr.ValueBoolean = newAttr.ValueBoolean;
                attr.ValueDateTime = newAttr.ValueDateTime;
                attr.ValueDecimal = newAttr.ValueDecimal;
                attr.ValueInteger = newAttr.ValueInteger;
                attr.ValueReference = newAttr.ValueReference;
                attr.ValueBinary = newAttr.ValueBinary;
                result.Add(new SCIMPatchResult { Attr = attr, Operation = SCIMPatchOperations.ADD, Path = fullPath });
            }

            return result;
        }

        private static ICollection<SCIMRepresentationAttribute> ExtractRepresentationAttributesFromJSON(
            ICollection<SCIMSchema> schemas,
            ICollection<SCIMSchemaAttribute> schemaAttributes,
            object obj,
            bool ignoreUnsupportedCanonicalValues)
        {
            var result = new List<SCIMRepresentationAttribute>();
            var jArr = obj as JArray;
            var jObj = obj as JObject;
            if (jObj != null && schemaAttributes != null && 
                schemaAttributes.Any() && 
                schemaAttributes.Count() == 1 && 
                schemaAttributes.First().Type == SCIMSchemaAttributeTypes.COMPLEX)
            {
                jArr = new JArray();
                jArr.Add(jObj);
            }

            var mainSchema = schemas.First(s => s.IsRootSchema);
            var extensionSchemas = schemas.Where(s => !s.IsRootSchema).ToList();
            if (jArr != null)
            {
                var schemaAttr = schemaAttributes.First();
                var schema = schemas.FirstOrDefault(s => s.HasAttribute(schemaAttr));
                result.AddRange(SCIMRepresentationHelper.BuildAttributes(jArr, schemaAttr, schema, ignoreUnsupportedCanonicalValues));
            }
            else if (jObj != null)
            {
                var resolutionResult = SCIMRepresentationHelper.Resolve(jObj, mainSchema, extensionSchemas);
                result.AddRange(SCIMRepresentationHelper.BuildRepresentationAttributes(resolutionResult, resolutionResult.AllSchemaAttributes, ignoreUnsupportedCanonicalValues, true));
            }
            else if (schemaAttributes.Any() && schemaAttributes.Count() == 1)
            {
                var schemaAttribute = schemaAttributes.First();
                switch (schemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schemaAttribute, valueBoolean: bool.Parse(obj.ToString())));
                        break;
                    case SCIMSchemaAttributeTypes.STRING:
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schemaAttribute, valueString: obj.ToString()));
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schemaAttribute, valueInteger: int.Parse(obj.ToString())));
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), schemaAttribute, valueDateTime: DateTime.Parse(obj.ToString()) ));
                        break;
                }
            }

            return result;
        }

        public static void EnrichResponse(SCIMRepresentation representation, IEnumerable<EnrichParameter> attributes, JObject jObj, bool isGetRequest = false)
        {
            foreach (var kvp in attributes.OrderBy(at => at.Order).GroupBy(a => a.Attr.SchemaAttribute.Id))
            {
                var record = jObj;
                var records = kvp.ToList();
                var firstRecord = records.First();
                if (!firstRecord.Attr.IsReadable(isGetRequest))
                {
                    continue;
                }

                var attributeName = firstRecord.Attr.SchemaAttribute.Name;
                // Si schéma d'extension alors il faut ajouter à l'object.
                if (firstRecord.Schema != null && !firstRecord.Schema.IsRootSchema && jObj.ContainsKey(attributeName))
                {
                    if (jObj.ContainsKey(firstRecord.Schema.Id))
                    {
                        record = jObj[firstRecord.Schema.Id] as JObject;
                    }
                    else
                    {
                        record = new JObject();
                        jObj.Add(firstRecord.Schema.Id, record);
                    }
                }

                switch (firstRecord.Attr.SchemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.STRING:
                        var valuesStr = records.Select(r => r.Attr.ValueString).Where(r => !string.IsNullOrWhiteSpace(r));
                        if (valuesStr.Any())
                            record.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesStr) : valuesStr.First());
                        break;
                    case SCIMSchemaAttributeTypes.REFERENCE:
                        var valuesRef = records.Select(r => r.Attr.ValueReference).Where(r => !string.IsNullOrWhiteSpace(r));
                        if (valuesRef.Any())
                            record.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesRef) : valuesRef.First());
                        break;
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        var valuesBoolean = records.Select(r => r.Attr.ValueBoolean).Where(r => r != null);
                        if (valuesBoolean.Any())
                            record.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesBoolean) : valuesBoolean.First());
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        var valuesInteger = records.Select(r => r.Attr.ValueInteger).Where(r => r != null);
                        if (valuesInteger.Any())
                            record.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesInteger) : valuesInteger.First());
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        var valuesDateTime = records.Select(r => r.Attr.ValueDateTime).Where(r => r != null);
                        if (valuesDateTime.Any())
                            record.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesDateTime) : valuesDateTime.First());
                        break;
                    case SCIMSchemaAttributeTypes.COMPLEX:
                        if (firstRecord.Attr.SchemaAttribute.MultiValued == false)
                        {
                            var jObjVal = new JObject();
                            var values = representation.GetChildren(firstRecord.Attr);
                            EnrichResponse(representation, values.Select(v => new EnrichParameter(firstRecord.Schema, 0, v)), jObjVal, isGetRequest);
                            if (jObjVal.Children().Count() > 0)
                            {
                                record.Add(firstRecord.Attr.SchemaAttribute.Name, jObjVal);
                            }
                        }
                        else
                        {
                            var jArr = new JArray();
                            foreach (var attr in records)
                            {
                                var jObjVal = new JObject();
                                var values = representation.GetChildren(attr.Attr);
                                EnrichResponse(representation, values.Select(v => new EnrichParameter(firstRecord.Schema, 0, v)), jObjVal, isGetRequest);
                                if (jObjVal.Children().Count() > 0)
                                {
                                    jArr.Add(jObjVal);
                                }
                            }

                            record.Add(firstRecord.Attr.SchemaAttribute.Name, jArr);
                        }
                        break;
                    case SCIMSchemaAttributeTypes.DECIMAL:
                        var valuesDecimal = records.Select(r => r.Attr.ValueDecimal).Where(r => r != null);
                        if (valuesDecimal.Any())
                            record.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesDecimal) : valuesDecimal.First());
                        break;
                    case SCIMSchemaAttributeTypes.BINARY:
                        var valuesBinary = records.Select(r => r.Attr.ValueBinary).Where(r => r != null);
                        if (valuesBinary.Any())
                            record.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesBinary) : valuesBinary.First());
                        break;
                }
            }
        }

        public class EnrichParameter
        {
            public EnrichParameter(SCIMSchema schema, int order, SCIMRepresentationAttribute attr)
            {
                Schema = schema;
                Order = order;
                Attr = attr;
            }

            public SCIMSchema Schema { get; set; }
            public int Order { get; set; }
            public SCIMRepresentationAttribute Attr { get; set; }
        }
    }
}
