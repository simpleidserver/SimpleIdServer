// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SimpleIdServer.Scim.Domain
{
    public static class SCIMRepresentationExtensions
    {
        private static List<string> COMMNON_PROPERTY_NAMES = new List<string>
        {
            { SCIMConstants.StandardSCIMRepresentationAttributes.ExternalId },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.ResourceType}" },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.Created}" },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.LastModified}" },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.Version}" }
        };

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

        public static void ApplyPatches(this SCIMRepresentation representation,  ICollection<PatchOperationParameter> patches, bool ignoreUnsupportedCanonicalValues)
        {
            var queryableRepresentationAttributes = representation.Attributes.AsQueryable();
            foreach (var patch in patches)
            {
                var scimFilter = SCIMFilterParser.Parse(patch.Path, representation.Schemas);
                var attributeExpression = scimFilter as SCIMAttributeExpression;
                var schemaAttributes = representation.Schemas.SelectMany(_ => _.Attributes);
                List<SCIMRepresentationAttribute> attributes = null;
                string fullPath = null;
                if (attributeExpression != null)
                {
                    fullPath = attributeExpression.GetFullPath();
                    schemaAttributes = representation.Schemas
                        .Select(s => s.GetAttribute(fullPath))
                        .Where(s => s != null);
                    attributes = GetRepresentationAttributeFromPath(representation, queryableRepresentationAttributes, scimFilter).ToList();
                }
                else
                {
                    attributes = queryableRepresentationAttributes.ToList();
                }

                var removeCallback = new Func<ICollection<SCIMRepresentationAttribute>, List<SCIMRepresentationAttribute>>((attrs) =>
                {
                    var result = new List<SCIMRepresentationAttribute>();
                    foreach (var a in attrs)
                    {
                        representation.RemoveAttribute(a);
                    }

                    return result;
                });



                switch (patch.Operation)
                {
                    case SCIMPatchOperations.ADD:
                        try
                        {
                            // If the target location already contains the value then do nothing ...
                            if (schemaAttributes == null || !schemaAttributes.Any())
                            {
                                throw new SCIMNoTargetException(string.Format(Global.AttributeIsNotRecognirzed, patch.Path));
                            }

                            var newAttributes = ExtractRepresentationAttributesFromJSON(representation.Schemas, schemaAttributes.ToList(), patch.Value, ignoreUnsupportedCanonicalValues);
                            var parentAttrs = removeCallback(attributes.Where(a => !a.SchemaAttribute.MultiValued && newAttributes.Any(_ => _.SchemaAttribute.Name == a.SchemaAttribute.Name)).ToList());
                            foreach (var newAttribute in newAttributes)
                            {
                                var path = string.IsNullOrWhiteSpace(fullPath) ? newAttribute.SchemaAttribute.Name : fullPath;
                                var schemaAttr = newAttribute.SchemaAttribute;
                                var parentAttribute = representation.GetParentAttribute(path);
                                if (schemaAttr.MultiValued && schemaAttr.Type != SCIMSchemaAttributeTypes.COMPLEX)
                                {
                                    var filteredAttribute = attributes.FirstOrDefault(_ => _.FullPath == path);
                                    if(filteredAttribute != null)
                                    {
                                        newAttribute.AttributeId = filteredAttribute.AttributeId;
                                    }

                                    attributes.Add(newAttribute);
                                    /*
                                    var filteredAttributes = attributes.Where(_ => _.FullPath == path);
                                    foreach(var attribute in filteredAttributes)
                                    {
                                        if (schemaAttr.Type == SCIMSchemaAttributeTypes.BOOLEAN)
                                        {
                                            foreach (var b in newAttribute.ValuesBoolean)
                                            {
                                                attribute.ValuesBoolean.Add(b);
                                            }
                                        }
                                        else if (schemaAttr.Type == SCIMSchemaAttributeTypes.DATETIME)
                                        {
                                            foreach (var d in newAttribute.ValuesDateTime)
                                            {
                                                attribute.ValuesDateTime.Add(d);
                                            }
                                        }
                                        else if (schemaAttr.Type == SCIMSchemaAttributeTypes.INTEGER)
                                        {
                                            foreach (var i in newAttribute.ValuesInteger)
                                            {
                                                attribute.ValuesInteger.Add(i);
                                            }
                                        }
                                        else if (schemaAttr.Type == SCIMSchemaAttributeTypes.REFERENCE)
                                        {
                                            foreach (var r in newAttribute.ValuesReference)
                                            {
                                                attribute.ValuesReference.Add(r);
                                            }
                                        }
                                        else if (schemaAttr.Type == SCIMSchemaAttributeTypes.STRING)
                                        {
                                            foreach (var s in newAttribute.ValuesString)
                                            {
                                                attribute.ValuesString.Add(s);
                                            }
                                        }
                                        else if (schemaAttr.Type == SCIMSchemaAttributeTypes.DECIMAL)
                                        {
                                            foreach (var d in newAttribute.ValuesDecimal)
                                            {
                                                attribute.ValuesDecimal.Add(d);
                                            }
                                        }
                                        else if (schemaAttr.Type == SCIMSchemaAttributeTypes.BINARY)
                                        {
                                            foreach (var b in newAttribute.ValuesBinary)
                                            {
                                                attribute.ValuesBinary.Add(b);
                                            }
                                        }
                                    }
                                    */
                                }
                                else if (parentAttribute != null)
                                {
                                    if (parentAttrs.Any())
                                    {
                                        foreach (var parentAttr in parentAttrs)
                                        {
                                            representation.AddAttribute(parentAttr, newAttribute);
                                        }
                                    }
                                    else
                                    {
                                        representation.AddAttribute(parentAttribute, newAttribute);
                                    }
                                }
                                else
                                {
                                    if (!representation.ContainsAttribute(newAttribute))
                                    {
                                        representation.Attributes.Add(newAttribute);
                                    }
                                }
                            }
                        }
                        catch (SCIMSchemaViolatedException)
                        {
                            continue;
                        }
                        break;
                    case SCIMPatchOperations.REMOVE:
                        {
                            if (attributeExpression == null)
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

                            var complexAttr = attributeExpression as SCIMComplexAttributeExpression;
                            if (complexAttr != null && !attributes.Any() && complexAttr.GroupingFilter != null)
                            {
                                throw new SCIMNoTargetException(Global.PatchMissingAttribute);
                            }

                            try
                            {
                                var newAttributes = ExtractRepresentationAttributesFromJSON(representation.Schemas, schemaAttributes.ToList(), patch.Value, ignoreUnsupportedCanonicalValues);
                                var parentFullPath = representation.GetParentAttribute(attributes.First())?.FullPath;
                                if (!string.IsNullOrWhiteSpace(parentFullPath))
                                {
                                    scimFilter = SCIMFilterParser.Parse(parentFullPath, representation.Schemas);
                                    var allAttrs = GetRepresentationAttributeFromPath(representation, queryableRepresentationAttributes, scimFilter);
                                    foreach (var attr in allAttrs)
                                    {
                                        if (!representation.GetChildren(attr).Any(_ => attributes.Any(a => a.SchemaAttribute.Name == _.SchemaAttribute.Name)))
                                        {
                                            representation.AddAttribute(attr, attributes.First());
                                        }
                                    }
                                }

                                Merge(attributes, newAttributes);
                            }
                            catch (SCIMSchemaViolatedException)
                            {
                                continue;
                            }
                        }
                        break;
                }
            }
        }

        public static JObject ToResponse(this SCIMRepresentation representation, string location, bool isGetRequest = false)
        {
            var meta = new JObject
            {
                { SCIMConstants.StandardSCIMMetaAttributes.ResourceType, representation.ResourceType },
                { SCIMConstants.StandardSCIMMetaAttributes.Created, representation.Created },
                { SCIMConstants.StandardSCIMMetaAttributes.LastModified, representation.LastModified },
                { SCIMConstants.StandardSCIMMetaAttributes.Version, representation.Version }
            };
            if (!string.IsNullOrWhiteSpace(location))
            {
                meta.Add(SCIMConstants.StandardSCIMMetaAttributes.Location, location);
            }

            var jObj = new JObject
            {
                { SCIMConstants.StandardSCIMRepresentationAttributes.Id, representation.Id },
                { SCIMConstants.StandardSCIMRepresentationAttributes.Schemas, new JArray(representation.Schemas.Select(s => s.Id)) },
                { SCIMConstants.StandardSCIMRepresentationAttributes.Meta, meta }
            };

            if (!string.IsNullOrWhiteSpace(representation.ExternalId))
            {
                jObj.Add(SCIMConstants.StandardSCIMRepresentationAttributes.ExternalId, representation.ExternalId);
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

        public static JObject ToResponseWithIncludedAttributes(this SCIMRepresentation representation, ICollection<SCIMExpression> includedAttributes)
        {
            // var result = new JObject();
            var filteredAttributes = new List<SCIMRepresentationAttribute>();
            var attributes = representation.Attributes.AsQueryable();
            foreach (var includedAttribute in includedAttributes)
            {
                filteredAttributes.AddRange(GetAttributes(includedAttribute as SCIMAttributeExpression, attributes).ToList());
                // representation.IncludeAttribute(includedAttribute, result);
            }

            // result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Id, representation.Id);
            representation.Attributes = filteredAttributes;
            return representation.ToResponse("");
            // return result;
        }

        public static JObject ToResponseWithExcludedAttributes(this SCIMRepresentation representation, ICollection<SCIMExpression> excludedAttributes, string location)
        {
            var result = (SCIMRepresentation)representation.Clone();
            foreach(var excludedAttribute in excludedAttributes)
            {
                var attributes = GetRepresentationAttributeFromPath(representation, result.Attributes.AsQueryable(), excludedAttribute).ToList();
                foreach(var attr in attributes)
                {
                    representation.RemoveAttribute(attr);
                }
            }

            var jObj = result.ToResponse(location, true);
            foreach(var excludedAttribute in excludedAttributes)
            {
                var fullPath = (excludedAttribute as SCIMAttributeExpression).GetFullPath();
                if (COMMNON_PROPERTY_NAMES.Contains(fullPath))
                {
                    jObj.SelectToken(fullPath).Parent.Remove();
                }
            }

            return jObj;
        }
        
        private static void Merge(List<SCIMRepresentationAttribute> attributes, ICollection<SCIMRepresentationAttribute> newAttributes)
        {
            foreach(var attribute in attributes)
            {
                var newAttr = newAttributes.FirstOrDefault(na => na.SchemaAttribute.Name == attribute.SchemaAttribute.Name);
                if (newAttr == null)
                {
                    continue;
                }

                attribute.ValueString = newAttr.ValueString;
                attribute.ValueBoolean = newAttr.ValueBoolean;
                attribute.ValueDateTime = newAttr.ValueDateTime;
                attribute.ValueDecimal = newAttr.ValueDecimal;
                attribute.ValueInteger = newAttr.ValueInteger;
                attribute.ValueReference = newAttr.ValueReference;
                attribute.ValueString = newAttr.ValueString;
                // TODO : Merge sub attributes ???
                // Merge(attribute.Values.ToList(), newAttr.Values);
            }
        }

        private static void IncludeAttribute(this SCIMRepresentation scimRepresentation, SCIMExpression scimExpression, JObject result)
        {
            var scimAttributeExpression = scimExpression as SCIMAttributeExpression;
            if (scimAttributeExpression == null)
            {
                throw new SCIMAttributeException(Global.InvalidAttributeExpression);
            }

            IncludeAttribute(scimRepresentation, scimAttributeExpression, scimRepresentation.Attributes.AsQueryable(), result);
            var fullPath = scimAttributeExpression.GetFullPath();
            if (!COMMNON_PROPERTY_NAMES.Contains(fullPath))
            {
                return;
            }

            if (fullPath == SCIMConstants.StandardSCIMRepresentationAttributes.ExternalId)
            {
                result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.ExternalId, scimRepresentation.ExternalId);
            } 
            else if (fullPath == $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.ResourceType}")
            {
                result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Meta, new JObject
                {
                    { SCIMConstants.StandardSCIMMetaAttributes.ResourceType, scimRepresentation.ResourceType }
                });
            }
            else if (fullPath == $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.Created}")
            {
                result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Meta, new JObject
                {
                    { SCIMConstants.StandardSCIMMetaAttributes.Created, scimRepresentation.Created }
                });
            }
            else if (fullPath == $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.LastModified}")
            {
                result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Meta, new JObject
                {
                    { SCIMConstants.StandardSCIMMetaAttributes.LastModified, scimRepresentation.LastModified }
                });
            }
            else if (fullPath == $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.Version}")
            {
                result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Meta, new JObject
                {
                    { SCIMConstants.StandardSCIMMetaAttributes.Version, scimRepresentation.Version }
                });
            }
        }

        private static void IncludeAttribute(SCIMRepresentation scimRepresentation, SCIMAttributeExpression scimExpression, IQueryable<SCIMRepresentationAttribute> attributes, JObject result)
        {
            var filteredAttributes = GetAttributes(scimExpression, attributes);
            if (!filteredAttributes.Any())
            {
                return;
            }

            if (scimExpression.Child != null)
            {
                foreach (var kvp in filteredAttributes.GroupBy(f => f.SchemaAttribute.Name))
                {
                    var representationAttr = kvp.First();
                    if (representationAttr.SchemaAttribute.MultiValued == false)
                    {
                        var jObjVal = new JObject();
                        var values = scimRepresentation.GetChildren(representationAttr).AsQueryable();
                        IncludeAttribute(scimRepresentation, scimExpression.Child, values, jObjVal);
                        result.Add(representationAttr.SchemaAttribute.Name, jObjVal);
                    }
                    else
                    {
                        var jArr = new JArray();
                        foreach (var attr in kvp)
                        {
                            var jObjVal = new JObject();
                            var values = scimRepresentation.GetChildren(attr).AsQueryable();
                            IncludeAttribute(scimRepresentation, scimExpression.Child, values, jObjVal);
                            jArr.Add(jObjVal);
                        }

                        result.Add(representationAttr.SchemaAttribute.Name, jArr);
                    }
                }

                return;
            }

            var enrichParameters = filteredAttributes.Select(at => new EnrichParameter(scimRepresentation.GetSchema(at), 0, at));
            EnrichResponse(scimRepresentation, enrichParameters, result);
        }

        private static IQueryable<SCIMRepresentationAttribute> GetRepresentationAttributeFromPath(SCIMRepresentation representation, IQueryable<SCIMRepresentationAttribute> representationAttributes, SCIMExpression scimExpression)
        {
            var scimAttributeExpression = scimExpression as SCIMAttributeExpression;
            if (scimAttributeExpression == null)
            {
                throw new SCIMAttributeException(Global.InvalidAttributeExpression);
            }

            var filteredAttributes = GetAttributes(scimAttributeExpression, representationAttributes);
            if (!filteredAttributes.Any())
            {
                return (new List<SCIMRepresentationAttribute>()).AsQueryable();
            }

            if (scimAttributeExpression.Child != null)
            {
                var values = filteredAttributes.SelectMany(v => representation.GetChildren(v));
                return GetRepresentationAttributeFromPath(representation, values, scimAttributeExpression.Child);
            }

            return filteredAttributes;
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
                schemaAttributes.First().Type == SCIMSchemaAttributeTypes.COMPLEX && 
                schemaAttributes.First().MultiValued)
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

        private static IQueryable<SCIMRepresentationAttribute> GetAttributes(SCIMAttributeExpression scimExpression, IQueryable<SCIMRepresentationAttribute> attributes)
        {
            var evaluatedExpression = scimExpression.Evaluate(attributes);
            return (IQueryable<SCIMRepresentationAttribute>)evaluatedExpression.Compile().DynamicInvoke(attributes);
            /*
            var filteredAttributes = attributes.Where(a => a.SchemaAttribute.Name == scimExpression.Name).AsQueryable();
            var scimComplexAttribute = scimExpression as SCIMComplexAttributeExpression;
            if (scimComplexAttribute != null)
            {
                var logicalExpr = scimComplexAttribute.GroupingFilter as SCIMLogicalExpression;
                Expression result;
                if (logicalExpr != null)
                {
                    var subParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), Guid.NewGuid().ToString("N"));
                    var leftExpr = logicalExpr.LeftExpression.Evaluate(subParameter);
                    var rightExpr = logicalExpr.RightExpression.Evaluate(subParameter);
                    var anyLeftLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(leftExpr, subParameter);
                    var anyRightLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(rightExpr, subParameter);
                    if (logicalExpr.LogicalOperator == SCIMLogicalOperators.AND)
                    {
                        result = Expression.AndAlso(anyLeftLambda, anyRightLambda);
                    }
                    else
                    {
                        result = Expression.Or(anyLeftLambda, anyRightLambda);
                    }
                }
                else
                {
                    var subParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), Guid.NewGuid().ToString("N"));
                    var lambdaExpression = scimComplexAttribute.GroupingFilter.Evaluate(subParameter);
                    var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(lambdaExpression, subParameter);
                    // result = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttribute) }, attributesProperty, anyLambda);
                }

                /*
                var whereLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(result, representationAttributeParameter);
                var enumarableType = typeof(Queryable);
                var whereMethod = enumarableType.GetMethods()
                     .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                     .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentationAttribute));
                var whereExpr = Expression.Call(whereMethod, Expression.Constant(filteredAttributes), whereLambda);
                var finalSelectArg = Expression.Parameter(typeof(IQueryable<SCIMRepresentationAttribute>), "f");
                var evaluatedExpression = Expression.Lambda(whereExpr, new ParameterExpression[] { finalSelectArg });
                filteredAttributes = (IQueryable<SCIMRepresentationAttribute>)evaluatedExpression.Compile().DynamicInvoke(filteredAttributes);
            }

            return filteredAttributes;
             */
        }

        public static void EnrichResponse(SCIMRepresentation representation, IEnumerable<EnrichParameter> attributes, JObject jObj, bool isGetRequest = false)
        {
            foreach (var kvp in attributes.OrderBy(at => at.Order).GroupBy(a => a.Attr.SchemaAttribute.Id))
            {
                var records = kvp.ToList();
                var firstRecord = records.First();
                if (!firstRecord.Attr.IsReadable(isGetRequest))
                {
                    continue;
                }

                var attributeName = firstRecord.Attr.SchemaAttribute.Name;
                if (firstRecord.Schema != null && !firstRecord.Schema.IsRootSchema && jObj.ContainsKey(attributeName))
                {
                    var oldJOBJ = jObj;
                    jObj = new JObject();
                    oldJOBJ.Add(firstRecord.Schema.Id, jObj);
                }

                switch (firstRecord.Attr.SchemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.STRING:
                        var valuesStr = records.Select(r => r.Attr.ValueString).Where(r => !string.IsNullOrWhiteSpace(r));
                        if (valuesStr.Any())
                            jObj.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesStr) : valuesStr.First());
                        break;
                    case SCIMSchemaAttributeTypes.REFERENCE:
                        var valuesRef = records.Select(r => r.Attr.ValueReference).Where(r => !string.IsNullOrWhiteSpace(r));
                        if (valuesRef.Any())
                            jObj.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesRef) : valuesRef.First());
                        break;
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        var valuesBoolean = records.Select(r => r.Attr.ValueBoolean).Where(r => r != null);
                        if (valuesBoolean.Any())
                            jObj.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesBoolean) : valuesBoolean.First());
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        var valuesInteger = records.Select(r => r.Attr.ValueInteger).Where(r => r != null);
                        if (valuesInteger.Any())
                            jObj.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesInteger) : valuesInteger.First());
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        var valuesDateTime = records.Select(r => r.Attr.ValueDateTime).Where(r => r != null);
                        if (valuesDateTime.Any())
                            jObj.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesDateTime) : valuesDateTime.First());
                        break;
                    case SCIMSchemaAttributeTypes.COMPLEX:
                        if (firstRecord.Attr.SchemaAttribute.MultiValued == false)
                        {
                            var jObjVal = new JObject();
                            var values = representation.GetChildren(firstRecord.Attr);
                            EnrichResponse(representation, values.Select(v => new EnrichParameter(firstRecord.Schema, 0, v)), jObjVal, isGetRequest);
                            if (jObjVal.Children().Count() > 0)
                            {
                                jObj.Add(firstRecord.Attr.SchemaAttribute.Name, jObjVal);
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

                            jObj.Add(firstRecord.Attr.SchemaAttribute.Name, jArr);
                        }
                        break;
                    case SCIMSchemaAttributeTypes.DECIMAL:
                        var valuesDecimal = records.Select(r => r.Attr.ValueDecimal).Where(r => r != null);
                        if (valuesDecimal.Any())
                            jObj.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesDecimal) : valuesDecimal.First());
                        break;
                    case SCIMSchemaAttributeTypes.BINARY:
                        var valuesBinary = records.Select(r => r.Attr.ValueBinary).Where(r => r != null);
                        if (valuesBinary.Any())
                            jObj.Add(firstRecord.Attr.SchemaAttribute.Name, firstRecord.Attr.SchemaAttribute.MultiValued ? (JToken)new JArray(valuesBinary) : valuesBinary.First());
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
