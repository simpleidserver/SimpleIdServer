// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Helpers;
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
            { SCIMConstants.StandardSCIMRepresentationAttributes.Id },
            { SCIMConstants.StandardSCIMRepresentationAttributes.ExternalId },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.ResourceType}" },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.Created}" },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.LastModified}" },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.Version}" }
        };

        public static void ApplyPatches(this SCIMRepresentation representation, ICollection<SCIMPatchOperationRequest> patches)
        {
            var queryableRepresentationAttributes = representation.Attributes.AsQueryable();
            foreach (var patch in patches)
            {
                var attributes = GetRepresentationAttributeFromPath(queryableRepresentationAttributes, SCIMFilterParser.Parse(patch.Path, representation.Schemas)).ToList();
                if (!attributes.Any())
                {
                    throw new SCIMAttributeException("PATCH can be applied only on existing attributes");
                }

                var removeCallback = new Action<ICollection<SCIMRepresentationAttribute>>((attrs) =>
                {
                    foreach (var a in attrs)
                    {
                        if (a.Parent != null)
                        {
                            a.Parent.Values.Remove(a);
                        }
                        else
                        {
                            representation.Attributes.Remove(a);
                        }
                    }
                });
                if (patch.Operation == SCIMPatchOperations.REMOVE || patch.Operation == SCIMPatchOperations.REPLACE)
                {
                    removeCallback(attributes);
                }

                if (patch.Operation == SCIMPatchOperations.ADD)
                {
                    removeCallback(attributes.Where(a => !a.SchemaAttribute.MultiValued).ToList());
                }

                if (patch.Operation == SCIMPatchOperations.ADD || patch.Operation == SCIMPatchOperations.REPLACE)
                {
                    var firstAttribute = attributes.First();
                    var newAttributes = ExtractRepresentationAttributesFromJSON(firstAttribute.SchemaAttribute, patch.Value);
                    foreach (var newAttribute in newAttributes)
                    {
                        if (firstAttribute.Parent != null)
                        {
                            firstAttribute.Parent.Values.Add(newAttribute);
                        }
                        else
                        {
                            representation.Attributes.Add(newAttribute);
                        }
                    }
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
        
        private static void IncludeAttribute(this SCIMRepresentation scimRepresentation, SCIMExpression scimExpression, JObject result)
        {
            var scimAttributeExpression = scimExpression as SCIMAttributeExpression;
            if (scimAttributeExpression == null)
            {
                throw new SCIMAttributeException("not a valid attribute expression");
            }

            IncludeAttribute(scimAttributeExpression, scimRepresentation.Attributes.AsQueryable(), result);
            var fullPath = scimAttributeExpression.GetFullPath();
            if (!COMMNON_PROPERTY_NAMES.Contains(fullPath))
            {
                return;
            }

            if (fullPath == SCIMConstants.StandardSCIMRepresentationAttributes.Id)
            {
                result.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Id, scimRepresentation.Id);
            }
            else if (fullPath == SCIMConstants.StandardSCIMRepresentationAttributes.ExternalId)
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
                throw new SCIMAttributeException("not a valid attribute expression");
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

        private static ICollection<SCIMRepresentationAttribute> ExtractRepresentationAttributesFromJSON(SCIMSchemaAttribute schemaAttribute, object jObj)
        {
            var jArr = jObj as JArray;
            var parsedRepresentationAttributes = new List<SCIMRepresentationAttribute>();
            if (schemaAttribute.Type == SCIMSchemaAttributeTypes.COMPLEX)
            {
                if (jArr != null)
                {
                    foreach (JObject record in jArr)
                    {
                        var attribute = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute);
                        foreach(var attr in SCIMRepresentationHelper.BuildRepresentationAttributes(record, schemaAttribute.SubAttributes))
                        {
                            attribute.Add(attr);
                        }

                        parsedRepresentationAttributes.Add(attribute);
                    }
                }
                else
                {
                    var attribute = new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute);
                    foreach (var attr in SCIMRepresentationHelper.BuildRepresentationAttributes(jObj as JObject, schemaAttribute.SubAttributes))
                    {
                        attribute.Add(attr);
                    }

                    parsedRepresentationAttributes.Add(attribute);
                }
            }
            else
            {
                switch (schemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        parsedRepresentationAttributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute, valuesBoolean: new List<bool> { bool.Parse(jObj.ToString()) }));
                        break;
                    case SCIMSchemaAttributeTypes.STRING:
                        parsedRepresentationAttributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute, valuesString: new List<string> { jObj.ToString() }));
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        parsedRepresentationAttributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute, valuesInteger: new List<int> { int.Parse(jObj.ToString()) }));
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        parsedRepresentationAttributes.Add(new SCIMRepresentationAttribute(Guid.NewGuid().ToString(), schemaAttribute, valuesDateTime: new List<DateTime> { DateTime.Parse(jObj.ToString()) }));
                        break;
                }
            }

            return parsedRepresentationAttributes;
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
                var anyLambdaExpression = scimComplexAttribute.GroupingFilter.Evaluate(subRepresentationAttributeParameter);
                var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(anyLambdaExpression, subRepresentationAttributeParameter);
                var whereLambdaExpression = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttribute) }, attributesProperty, anyLambda);
                var whereLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(whereLambdaExpression, representationAttributeParameter);
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
                        jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesString) : representationAttr.ValuesString.First());
                        break;
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesBoolean) : representationAttr.ValuesBoolean.First());
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesInteger) : representationAttr.ValuesInteger.First());
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        jObj.Add(representationAttr.SchemaAttribute.Name, representationAttr.SchemaAttribute.MultiValued ? (JToken)new JArray(representationAttr.ValuesDateTime) : representationAttr.ValuesDateTime.First());
                        break;
                    case SCIMSchemaAttributeTypes.COMPLEX:
                        if (representationAttr.SchemaAttribute.MultiValued == false)
                        {
                            var jObjVal = new JObject();
                            EnrichResponse(representationAttr.Values.AsQueryable(), jObjVal, isGetRequest);
                            jObj.Add(representationAttr.SchemaAttribute.Name, jObjVal);
                        }
                        else
                        {
                            var jArr = new JArray();
                            foreach (var attr in kvp)
                            {
                                var jObjVal = new JObject();
                                EnrichResponse(attr.Values.AsQueryable(), jObjVal, isGetRequest);
                                jArr.Add(jObjVal);
                            }

                            jObj.Add(representationAttr.SchemaAttribute.Name, jArr);
                        }
                        break;
                }
            }
        }
    }
}
