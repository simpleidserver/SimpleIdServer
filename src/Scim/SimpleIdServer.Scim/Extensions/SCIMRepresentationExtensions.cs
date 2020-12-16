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

        public static void ApplyPatches(this SCIMRepresentation representation, ICollection<PatchOperationParameter> patches, bool ignoreUnsupportedCanonicalValues)
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
                    attributes = GetRepresentationAttributeFromPath(queryableRepresentationAttributes, scimFilter).ToList();
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
                        if (a.Parent != null)
                        {
                            a.Parent.Values.Remove(a);
                            result.Add(a.Parent);
                        }
                        else
                        {
                            representation.Attributes.Remove(a);
                        }
                    }

                    return result;
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

                            var newAttributes = ExtractRepresentationAttributesFromJSON(schemaAttributes.ToList(), patch.Value, ignoreUnsupportedCanonicalValues);
                            var parentAttrs = removeCallback(attributes.Where(a => !a.SchemaAttribute.MultiValued && newAttributes.Any(_ => _.SchemaAttribute.Name == a.SchemaAttribute.Name)).ToList());
                            foreach (var newAttribute in newAttributes)
                            {
                                var path = string.IsNullOrWhiteSpace(fullPath) ? newAttribute.SchemaAttribute.Name : fullPath;
                                var schemaAttr = newAttribute.SchemaAttribute;
                                var parentAttribute = representation.GetParentAttribute(path);
                                if (schemaAttr.MultiValued && schemaAttr.Type != SCIMSchemaAttributeTypes.COMPLEX)
                                {
                                    var filteredAttributes = attributes.Where(_ => _.GetFullPath() == path);
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
                                }
                                else if (parentAttribute != null)
                                {
                                    if (parentAttrs.Any())
                                    {
                                        foreach (var parentAttr in parentAttrs)
                                        {
                                            parentAttr.Add(newAttribute);
                                        }
                                    }
                                    else
                                    {
                                        parentAttribute.Add(newAttribute);
                                    }
                                }
                                else
                                {
                                    representation.Attributes.Add(newAttribute);
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
                                var newAttributes = ExtractRepresentationAttributesFromJSON(schemaAttributes.ToList(), patch.Value, ignoreUnsupportedCanonicalValues);
                                var parentFullPath = attributes.First().Parent?.GetFullPath();
                                if (!string.IsNullOrWhiteSpace(parentFullPath))
                                {
                                    scimFilter = SCIMFilterParser.Parse(parentFullPath, representation.Schemas);
                                    var allAttrs = GetRepresentationAttributeFromPath(queryableRepresentationAttributes, scimFilter);
                                    foreach (var attr in allAttrs)
                                    {
                                        if (!attr.Values.Any(_ => attributes.Any(a => a.SchemaAttribute.Name == _.SchemaAttribute.Name)))
                                        {
                                            attr.Add(attributes.First());
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
                { SCIMConstants.StandardSCIMMetaAttributes.Version, representation.Version },
                { SCIMConstants.StandardSCIMMetaAttributes.Location, location }
            };
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

            EnrichResponse(representation.Attributes.AsQueryable(), jObj, isGetRequest);
            return jObj;
        }

        public static JObject ToResponseWithIncludedAttributes(this SCIMRepresentation representation, ICollection<SCIMExpression> includedAttributes)
        {
            var result = new JObject();
            foreach (var includedAttribute in includedAttributes)
            {
                representation.IncludeAttribute(includedAttribute, result);
            }

            result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Id, representation.Id);
            return result;
        }

        public static JObject ToResponseWithExcludedAttributes(this SCIMRepresentation representation, ICollection<SCIMExpression> excludedAttributes, string location)
        {
            var result = (SCIMRepresentation)representation.Clone();
            foreach(var excludedAttribute in excludedAttributes)
            {
                var attributes = GetRepresentationAttributeFromPath(result.Attributes.AsQueryable(), excludedAttribute).ToList();
                foreach(var attr in attributes)
                {
                    if (attr.Parent != null)
                    {
                        attr.Parent.Values.Remove(attr);
                    }
                    else
                    {
                        result.Attributes.Remove(attr);
                    }
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

                attribute.ValuesBinary = newAttr.ValuesBinary;
                attribute.ValuesBoolean = newAttr.ValuesBoolean;
                attribute.ValuesDateTime = newAttr.ValuesDateTime;
                attribute.ValuesDecimal = newAttr.ValuesDecimal;
                attribute.ValuesInteger = newAttr.ValuesInteger;
                attribute.ValuesReference = newAttr.ValuesReference;
                attribute.ValuesString = newAttr.ValuesString;
                Merge(attribute.Values.ToList(), newAttr.Values);
            }
        }

        private static void IncludeAttribute(this SCIMRepresentation scimRepresentation, SCIMExpression scimExpression, JObject result)
        {
            var scimAttributeExpression = scimExpression as SCIMAttributeExpression;
            if (scimAttributeExpression == null)
            {
                throw new SCIMAttributeException(Global.InvalidAttributeExpression);
            }

            IncludeAttribute(scimAttributeExpression, scimRepresentation.Attributes.AsQueryable(), result);
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

        private static void IncludeAttribute(SCIMAttributeExpression scimExpression, IQueryable<SCIMRepresentationAttribute> attributes, JObject result)
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
                        IncludeAttribute(scimExpression.Child, representationAttr.Values.AsQueryable(), jObjVal);
                        result.Add(representationAttr.SchemaAttribute.Name, jObjVal);
                    }
                    else
                    {
                        var jArr = new JArray();
                        foreach (var attr in kvp)
                        {
                            var jObjVal = new JObject();
                            IncludeAttribute(scimExpression.Child, attr.Values.AsQueryable(), jObjVal);
                            jArr.Add(jObjVal);
                        }

                        result.Add(representationAttr.SchemaAttribute.Name, jArr);
                    }
                }

                return;
            }

            EnrichResponse(filteredAttributes, result);
        }

        private static IQueryable<SCIMRepresentationAttribute> GetRepresentationAttributeFromPath(IQueryable<SCIMRepresentationAttribute> representationAttributes, SCIMExpression scimExpression)
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
                return GetRepresentationAttributeFromPath(filteredAttributes.SelectMany(v => v.Values), scimAttributeExpression.Child);
            }

            return filteredAttributes;
        }

        private static ICollection<SCIMRepresentationAttribute> ExtractRepresentationAttributesFromJSON(ICollection<SCIMSchemaAttribute> schemaAttributes, object obj, bool ignoreUnsupportedCanonicalValues)
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

            if (jArr != null)
            {
                result.AddRange(SCIMRepresentationHelper.BuildAttributes(jArr, schemaAttributes.First(), ignoreUnsupportedCanonicalValues));
            }
            else if (jObj != null)
            {
                result.AddRange(SCIMRepresentationHelper.BuildRepresentationAttributes(jObj, schemaAttributes, ignoreUnsupportedCanonicalValues, true));
            }
            else if (schemaAttributes.Any() && schemaAttributes.Count() == 1)
            {
                var schemaAttribute = schemaAttributes.First();
                switch (schemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute, valuesBoolean: new List<bool> { bool.Parse(obj.ToString()) }));
                        break;
                    case SCIMSchemaAttributeTypes.STRING:
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute, valuesString: new List<string> { obj.ToString() }));
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute, valuesInteger: new List<int> { int.Parse(obj.ToString()) }));
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        result.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute, valuesDateTime: new List<DateTime> { DateTime.Parse(obj.ToString()) }));
                        break;
                }
            }

            return result;
        }

        private static IQueryable<SCIMRepresentationAttribute> GetAttributes(SCIMAttributeExpression scimExpression, IQueryable<SCIMRepresentationAttribute> attributes)
        {
            var filteredAttributes = attributes.Where(a => a.SchemaAttribute.Name == scimExpression.Name).AsQueryable();
            var scimComplexAttribute = scimExpression as SCIMComplexAttributeExpression;
            if (scimComplexAttribute != null)
            {
                var representationAttributeParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), "rp");
                var attributesProperty = Expression.Property(representationAttributeParameter, "Values");
                var subRepresentationAttributeParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), Guid.NewGuid().ToString("N"));
                var logicalExpr = scimComplexAttribute.GroupingFilter as SCIMLogicalExpression;
                Expression result;
                if (logicalExpr != null)
                {
                    var subParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), Guid.NewGuid().ToString("N"));
                    var leftExpr = logicalExpr.LeftExpression.Evaluate(subParameter);
                    var rightExpr = logicalExpr.RightExpression.Evaluate(subParameter);
                    var anyLeftLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(leftExpr, subParameter);
                    var anyRightLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(rightExpr, subParameter);
                    var anyLeftCall = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttribute) }, attributesProperty, anyLeftLambda);
                    var anyRightCall = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttribute) }, attributesProperty, anyRightLambda);
                    if (logicalExpr.LogicalOperator == SCIMLogicalOperators.AND)
                    {
                        result = Expression.AndAlso(anyLeftCall, anyRightCall);
                    }
                    else
                    {
                        result = Expression.Or(anyLeftCall, anyRightCall);
                    }
                }
                else
                {
                    var subParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), Guid.NewGuid().ToString("N"));
                    var lambdaExpression = scimComplexAttribute.GroupingFilter.Evaluate(subParameter);
                    var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(lambdaExpression, subParameter);
                    result = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttribute) }, attributesProperty, anyLambda);
                }

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
        }

        public static void EnrichResponse(IQueryable<SCIMRepresentationAttribute> attributes, JObject jObj, bool isGetRequest = false)
        {
            foreach (var kvp in attributes.GroupBy(a => a.SchemaAttribute.Name))
            {
                var representationAttr = kvp.First();
                if (!representationAttr.IsReadable(isGetRequest))
                {
                    continue;
                }

                switch (representationAttr.SchemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.STRING:
                        if (representationAttr.ValuesString != null && representationAttr.ValuesString.Any())
                            jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesString) : representationAttr.ValuesString.First());
                        break;
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        if (representationAttr.ValuesBoolean != null && representationAttr.ValuesBoolean.Any())
                            jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesBoolean) : representationAttr.ValuesBoolean.First());
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        if (representationAttr.ValuesInteger != null && representationAttr.ValuesInteger.Any())
                            jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesInteger) : representationAttr.ValuesInteger.First());
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        if (representationAttr.ValuesDateTime != null && representationAttr.ValuesDateTime.Any())
                            jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesDateTime) : representationAttr.ValuesDateTime.First());
                        break;
                    case SCIMSchemaAttributeTypes.COMPLEX:
                        if (representationAttr.SchemaAttribute.MultiValued == false)
                        {
                            var jObjVal = new JObject();
                            EnrichResponse(representationAttr.Values.AsQueryable(), jObjVal, isGetRequest);
                            if (jObjVal.Children().Count() > 0)
                            {
                                jObj.Add(representationAttr.SchemaAttribute.Name, jObjVal);
                            }
                        }
                        else
                        {
                            var jArr = new JArray();
                            foreach (var attr in kvp)
                            {
                                var jObjVal = new JObject();
                                EnrichResponse(attr.Values.AsQueryable(), jObjVal, isGetRequest);
                                if (jObjVal.Children().Count() > 0)
                                {
                                    jArr.Add(jObjVal);
                                }
                            }

                            jObj.Add(representationAttr.SchemaAttribute.Name, jArr);
                        }
                        break;
                    case SCIMSchemaAttributeTypes.DECIMAL:
                        if (representationAttr.ValuesDecimal != null && representationAttr.ValuesDecimal.Any())
                            jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesDecimal) : representationAttr.ValuesDecimal.First());
                        break;
                    case SCIMSchemaAttributeTypes.BINARY:
                        if (representationAttr.ValuesBinary != null && representationAttr.ValuesBinary.Any())
                            jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesBinary.Select(_ => Convert.ToBase64String(_))) : Convert.ToBase64String(representationAttr.ValuesBinary.First()));
                        break;
                }
            }
        }
    }
}
