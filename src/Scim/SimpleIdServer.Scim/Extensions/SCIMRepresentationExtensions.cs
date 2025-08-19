// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Parser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Scim.Domain
{
    public static class SCIMRepresentationExtensions
    {
        public static void Apply(this SCIMRepresentation representation, List<RepresentationSyncResult> syncLst, List<SCIMPatchResult> patchLst)
        {
            var patchRemovedAttrs = patchLst.Where(p => p.Attr != null && p.Operation == SCIMPatchOperations.REMOVE);
            var patchUpdatedAttr = patchLst.Where(p => p.Attr != null && p.Operation == SCIMPatchOperations.REPLACE);
            representation.FlatAttributes = representation.FlatAttributes.Where(a => !patchRemovedAttrs.Any(r => r.Attr.Id == a.Id) && !patchUpdatedAttr.Any(r => r.Attr.Id == a.Id)).ToList();
            var patchAddedAttr = patchLst.Where(p => p.Attr != null && p.Operation == SCIMPatchOperations.ADD && !representation.FlatAttributes.Any(a => a.SchemaAttributeId == p.Attr.SchemaAttributeId && a.ComputedValueIndex == p.Attr.ComputedValueIndex));
            foreach (var p in patchAddedAttr) representation.FlatAttributes.Add(p.Attr);
            foreach (var p in patchUpdatedAttr) representation.FlatAttributes.Add(p.Attr);
            foreach(var sync in syncLst)
            {
                var attributesAdded = sync.AddedRepresentationAttributes.Where(a => a != null && a.RepresentationId == representation.Id);
                var attributesRemoved = sync.RemovedRepresentationAttributes.Where(a => a != null && a.RepresentationId == representation.Id);
                var attributesUpdated = sync.UpdatedRepresentationAttributes.Where(a => a != null && a.RepresentationId == representation.Id);
                representation.FlatAttributes = representation.FlatAttributes.Where(a => !attributesRemoved.Any(r => r.Id == a.Id) && !attributesUpdated.Any(r => r.Id == a.Id)).ToList();
                foreach (var attributeAdded in attributesAdded)
                    representation.FlatAttributes.Add(attributeAdded);
                foreach (var attributeUpdated in attributesUpdated)
                    representation.FlatAttributes.Add(attributeUpdated);
            }
        }

        public static void FilterAttributes(this IEnumerable<SCIMRepresentation> representations, IEnumerable<SCIMAttributeExpression> includedAttributes, IEnumerable<SCIMAttributeExpression> excludedAttributes)
        {
            foreach (var representation in representations) representation.FilterAttributes(includedAttributes, excludedAttributes);
        }

        public static void FilterAttributes(this SCIMRepresentation representation, IEnumerable<SCIMAttributeExpression> includedAttributes, IEnumerable<SCIMAttributeExpression> excludedAttributes)
        {
            var queryableAttributes = representation.FlatAttributes.AsQueryable();
            representation.RefreshHierarchicalAttributesCache();
            if (includedAttributes != null && includedAttributes.Any())
            {
                var attrs = queryableAttributes.FilterAttributes(includedAttributes).ToList();
                var includedFullPathLst = (includedAttributes != null && includedAttributes.Any()) ? includedAttributes.Where(i => i is SCIMComplexAttributeExpression).Select(i => i.GetFullPath()) : new List<string>();
                representation.FlatAttributes = attrs.Where(a => a != null).SelectMany(_ =>
                {
                    var lst = new List<SCIMRepresentationAttribute> { _ };
                    lst.AddRange(_.Children.Where(c => includedFullPathLst.Any(f => c.FullPath.StartsWith(f))));
                    return lst;
                }).ToList();
            }
            else if (excludedAttributes != null && excludedAttributes.Any())
                representation.FlatAttributes = queryableAttributes.FilterAttributes(excludedAttributes, false).ToList();
        }

        public static IQueryable<SCIMRepresentationAttribute> FilterAttributes(this IQueryable<SCIMRepresentationAttribute> representationAttributes, IEnumerable<SCIMAttributeExpression> attributes, bool isIncluded = true, string propertyName = "CachedChildren")
        {
            var enumarableType = typeof(Queryable);
            var whereMethod = enumarableType.GetMethods()
                 .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                 .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentationAttribute));
            var representationParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), "rp");
            var startWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
            var fullPathProperty = Expression.Property(representationParameter, "FullPath");
            var namespaceProperty = Expression.Property(representationParameter, "Namespace");
            Expression expression = null;
            foreach (var attribute in attributes.Where(a => a != null))
            {
                Expression record = null;
                if (attribute.TryContainsGroupingExpression(out SCIMComplexAttributeExpression complexAttributeExpression))
                {
                    record = attribute.EvaluateAttributes(representationParameter, propertyName);
                }
                else
                {
                    if (isIncluded)
                    {
                        record = FilterIncludedAttributes(fullPathProperty, attribute, null, attribute.GetFullPath(false));
                    }
                    else
                    {
                        record = Expression.Call(fullPathProperty, startWith, Expression.Constant(attribute.GetFullPath(false)));
                    }
                }

                var ns = attribute.GetNamespace();
                if (!string.IsNullOrWhiteSpace(ns))
                {
                    record = Expression.And(record, Expression.Equal(namespaceProperty, Expression.Constant(ns)));
                }

                if (expression == null)
                {
                    expression = record;
                }
                else
                {
                    expression = Expression.Or(expression, record);
                }
            }

            if (expression == null)
            {
                return representationAttributes;
            }

            if (!isIncluded)
            {
                expression = Expression.Not(expression);
            }

            var equalLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(expression, representationParameter);
            var whereExpr = Expression.Call(whereMethod, Expression.Constant(representationAttributes), equalLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<SCIMRepresentationAttribute>), "f");
            var finalSelectRequestBody = Expression.Lambda(whereExpr, new ParameterExpression[] { finalSelectArg });
            var filteredAttrs = (IQueryable<SCIMRepresentationAttribute>)finalSelectRequestBody.Compile().DynamicInvoke(representationAttributes);
            return filteredAttrs;
        }

        private static Expression FilterIncludedAttributes(MemberExpression member, SCIMAttributeExpression attr, string parentPath, string fullPath)
        {
            var fp = string.IsNullOrWhiteSpace(parentPath) ? attr.Name : $"{parentPath}.{attr.Name}";
            fp = SCIMAttributeExpression.RemoveNamespace(fp);
            Expression equal = null;
            if (fullPath == fp)
            {
                var startWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                equal = Expression.Call(member, startWith, Expression.Constant(fullPath));
            }
            else
            {
                equal = Expression.Equal(member, Expression.Constant(fp));
            }

            Expression sub = null;
            if (attr.Child != null)
            {
                sub = FilterIncludedAttributes(member, attr.Child, fp, fullPath);
            }

            return sub == null ? equal : Expression.Or(equal, sub);
        }

        public static void ApplyEmptyArray(this SCIMRepresentation representation)
        {
            foreach(var schema in representation.Schemas)
            {
                var attrs = schema.HierarchicalAttributes.Select(a => a.Leaf).Where(a => a.MultiValued);
                foreach (var attr in attrs)
                {
                    if (!representation.FlatAttributes.Any(a => a.SchemaAttributeId == attr.Id))
                    {
                        representation.FlatAttributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), attr, schema.Id));
                    }
                }
            }
        }

        public static JsonObject ToResponse(this SCIMRepresentation representation, string location, bool isGetRequest = false, bool includeStandardAttributes = true, bool addEmptyArray = false, bool mergeExtensionAttributes = false)
        {
            var jObj = new JsonObject();
            if (!string.IsNullOrEmpty(representation.Id))
            {
                jObj.Add(StandardSCIMRepresentationAttributes.Id, representation.Id);
            }

            if (includeStandardAttributes)
            {
                representation.AddStandardAttributes(location, new List<string> { }, ignore: true);
            }

            if(addEmptyArray)
            {
                representation.ApplyEmptyArray();
            }

            var attributes = representation.HierarchicalAttributes
                .Where(p => string.IsNullOrWhiteSpace(p.ParentAttributeId))
                .Select(a =>
            {
                var schema = representation.GetSchema(a);
                var order = 1;
                if (schema != null && schema.IsRootSchema)
                {
                    order = 0;
                }

                return new EnrichParameter(schema, order, a);
            }).ToList();
            EnrichResponse(attributes, jObj, mergeExtensionAttributes, isGetRequest);
            return jObj;
        }

        private static bool TryGetExternalId(PatchOperationParameter patchOperation, out string externalId)
        {
            externalId = null;
            if (patchOperation.Value == null)
            {
                return false;
            }

            var jObj = patchOperation.Value as JsonObject;
            if (patchOperation.Path == StandardSCIMRepresentationAttributes.ExternalId && 
                (patchOperation.Value.GetType() == typeof(string) || patchOperation.Value.GetType() == typeof(JsonValue)))
            {
                externalId = patchOperation.Value.ToString();
                return true;
            }

            if (jObj != null && jObj.ContainsKey(StandardSCIMRepresentationAttributes.ExternalId))
            {
                externalId = jObj[StandardSCIMRepresentationAttributes.ExternalId].ToString();
                return true;
            }

            return false;
        }

        private static List<SCIMPatchResult> Merge(List<SCIMRepresentationAttribute> attributes, ICollection<SCIMRepresentationAttribute> newAttributes, string fullPath)
        {
            var result = new List<SCIMPatchResult>();
            foreach (var attr in attributes)
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

        public static void EnrichResponse(IEnumerable<EnrichParameter> attributes, JsonObject jObj, bool mergeExtensionAttributes = false, bool isGetRequest = false)
        {
            foreach (var kvp in attributes.OrderBy(at => at.Order).GroupBy(a => a.AttributeNode.SchemaAttribute.Id))
            {
                var record = jObj;
                var records = kvp.ToList();
                var firstRecord = records.First();
                if (!firstRecord.AttributeNode.IsReadable(isGetRequest))
                {
                    continue;
                }

                var attributeName = firstRecord.AttributeNode.SchemaAttribute.Name;
                if (firstRecord.Schema != null && !firstRecord.Schema.IsRootSchema && 
                    ((mergeExtensionAttributes && jObj.ContainsKey(attributeName)) || (!mergeExtensionAttributes && string.IsNullOrWhiteSpace(firstRecord.AttributeNode.ParentAttributeId)))
                )
                {
                    if (jObj.ContainsKey(firstRecord.Schema.Id))
                    {
                        record = jObj[firstRecord.Schema.Id] as JsonObject;
                    }
                    else
                    {
                        record = new JsonObject();
                        jObj.Add(firstRecord.Schema.Id, record);
                    }
                }

                switch (firstRecord.AttributeNode.SchemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.STRING:
                        var valuesStr = records.Select(r => r.AttributeNode.ValueString).Where(r => r != null);
                        if (valuesStr.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JsonNode)new JsonArray(valuesStr.Select(s => JsonValue.Create(s)).ToArray()) : valuesStr.First());
                        break;
                    case SCIMSchemaAttributeTypes.REFERENCE:
                        var valuesRef = records.Select(r => r.AttributeNode.ValueReference).Where(r => r != null);
                        if (valuesRef.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JsonNode)new JsonArray(valuesRef.Select(s => JsonValue.Create(s)).ToArray()) : valuesRef.First());
                        break;
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        var valuesBoolean = records.Select(r => r.AttributeNode.ValueBoolean).Where(r => r != null);
                        if (valuesBoolean.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JsonNode)new JsonArray(valuesBoolean.Select(s => JsonValue.Create(s)).ToArray()) : valuesBoolean.First());
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        var valuesInteger = records.Select(r => r.AttributeNode.ValueInteger).Where(r => r != null);
                        if (valuesInteger.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JsonNode)new JsonArray(valuesInteger.Select(s => JsonValue.Create(s)).ToArray()) : valuesInteger.First());
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        var valuesDateTime = records.Select(r => r.AttributeNode.ValueDateTime).Where(r => r != null);
                        if (valuesDateTime.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JsonNode)new JsonArray(valuesDateTime.Select(s => JsonValue.Create(s)).ToArray()) : valuesDateTime.First());
                        break;
                    case SCIMSchemaAttributeTypes.COMPLEX:
                        if (firstRecord.AttributeNode.SchemaAttribute.MultiValued == false)
                        {
                            var jObjVal = new JsonObject();
                            var children = firstRecord.AttributeNode.CachedChildren;
                            EnrichResponse(children.Select(v => new EnrichParameter(firstRecord.Schema, 0, v)), jObjVal, mergeExtensionAttributes, isGetRequest);
                            if (jObjVal.AsEnumerable().Any())
                            {
                                record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, jObjVal);
                            }
                        }
                        else
                        {
                            var jArr = new JsonArray();
                            foreach (var attr in records)
                            {
                                var jObjVal = new JsonObject();
                                var children = attr.AttributeNode.CachedChildren;
                                EnrichResponse(children.Select(v => new EnrichParameter(firstRecord.Schema, 0, v)), jObjVal, mergeExtensionAttributes, isGetRequest);
                                if (jObjVal.AsEnumerable().Any())
                                {
                                    jArr.Add(jObjVal);
                                }
                            }

                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, jArr);
                        }
                        break;
                    case SCIMSchemaAttributeTypes.DECIMAL:
                        var valuesDecimal = records.Select(r => r.AttributeNode.ValueDecimal).Where(r => r != null);
                        if (valuesDecimal.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JsonNode)new JsonArray(valuesDecimal.Select(s => JsonValue.Create(s)).ToArray()) : valuesDecimal.First());
                        break;
                    case SCIMSchemaAttributeTypes.BINARY:
                        var valuesBinary = records.Select(r => r.AttributeNode.ValueBinary).Where(r => r != null);
                        if (valuesBinary.Any())
                            record.Add(firstRecord.AttributeNode.SchemaAttribute.Name, firstRecord.AttributeNode.SchemaAttribute.MultiValued ? (JsonNode)new JsonArray(valuesBinary.Select(s => JsonValue.Create(s)).ToArray()) : valuesBinary.First());
                        break;
                }
            }
        }

        public class EnrichParameter
        {
            public EnrichParameter(SCIMSchema schema, int order, SCIMRepresentationAttribute attributeNode)
            {
                Schema = schema;
                Order = order;
                AttributeNode = attributeNode;
            }

            public SCIMSchema Schema { get; set; }
            public int Order { get; set; }
            public SCIMRepresentationAttribute AttributeNode { get; set; }
        }
    }
}
