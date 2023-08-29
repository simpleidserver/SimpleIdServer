// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser;
using SimpleIdServer.Scim.Parser.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF.Extensions
{
    public static class EFSCIMExpressionLinqExtensions
    {
        #region Build Result

        public static async Task<SCIMRepresentation> BuildResult(
            this IQueryable<SCIMRepresentation> representations,
            SCIMDbContext dbContext,
            IEnumerable<SCIMAttributeExpression> includedAttributes,
            IEnumerable<SCIMAttributeExpression> excludedAttributes,
            string id,
            string resourceType)
        {
            IQueryable<SCIMRepresentationAttribute> filteredAttrs = null;
            if (includedAttributes != null && includedAttributes.Any())
            {
                filteredAttrs = dbContext.SCIMRepresentationAttributeLst.Include(r => r.Children).FilterAttributes(includedAttributes, propertyName: "Children");
            }

            if (excludedAttributes != null && excludedAttributes.Any())
            {
                filteredAttrs = dbContext.SCIMRepresentationAttributeLst.Include(r => r.Children).FilterAttributes(excludedAttributes, false, "Children");
            }

            if (filteredAttrs != null)
            {
                filteredAttrs = filteredAttrs.Where(a => a.RepresentationId == id);
                var result = await representations.FirstOrDefaultAsync(r => r.Id == id && r.ResourceType == resourceType);
                result.FlatAttributes = filteredAttrs.ToList();
                return result;
            }
            
            return await representations.FirstOrDefaultAsync(r => r.Id == id && r.ResourceType == resourceType);
        }

        public static SearchSCIMRepresentationsResponse BuildResult(
            this IQueryable<SCIMRepresentation> representations,
            SCIMDbContext dbContext,
            IEnumerable<SCIMAttributeExpression> includedAttributes,
            IEnumerable<SCIMAttributeExpression> excludedAttributes,
            int total)
        {
            IQueryable<SCIMRepresentationAttribute> filteredAttrs= null;
            if (includedAttributes != null && includedAttributes.Any())
            {
                filteredAttrs = dbContext.SCIMRepresentationAttributeLst.Include(r => r.Children).FilterAttributes(includedAttributes, propertyName: "Children");
            }

            if(excludedAttributes != null && excludedAttributes.Any())
            {
                filteredAttrs = dbContext.SCIMRepresentationAttributeLst.Include(r => r.Children).FilterAttributes(excludedAttributes, false, "Children");
            }

            if (filteredAttrs != null)
            {
                var result = from rep in representations
                                join at in filteredAttrs on rep.Id equals at.RepresentationId into a
                                from attr in a.DefaultIfEmpty()
                                select new 
                                { 
                                    Attr = attr, 
                                    Id = rep.Id, 
                                    Created = rep.Created, 
                                    LastModified = rep.LastModified, 
                                    ExternalId = rep.ExternalId, 
                                    ResourceType = rep.ResourceType,
                                    DisplayName = rep.DisplayName,
                                    Version = rep.Version
                                };
                var content = result.AsEnumerable().GroupBy(r => r.Id);
                var includedFullPathLst = (includedAttributes != null && includedAttributes.Any()) ? includedAttributes.Where(i => i is SCIMComplexAttributeExpression).Select(i => i.GetFullPath()) : new List<string>();
                return new SearchSCIMRepresentationsResponse(total, content.Select(c => new SCIMRepresentation
                {
                    FlatAttributes = c.Where(_ => _.Attr != null).SelectMany(_ =>
                    {
                        var lst = new List<SCIMRepresentationAttribute>
                        {
                            _.Attr
                        };
                        lst.AddRange(_.Attr.Children.Where(c => includedFullPathLst.Any(f => c.FullPath.StartsWith(f))));
                        return lst;
                    }).ToList(),
                    Id = c.Key,
                    Created = c.First().Created,
                    DisplayName = c.First().DisplayName,
                    ExternalId = c.First().ExternalId,
                    LastModified = c.First().LastModified,
                    ResourceType = c.First().ResourceType,
                    Version = c.First().Version
                }));
            }

            var reps = representations.ToList();
            return new SearchSCIMRepresentationsResponse(total, reps);
        }

        #endregion

        #region Order By

        public static async Task<SearchSCIMRepresentationsResponse> EvaluateOrderBy(
            this SCIMExpression expression,
            SCIMDbContext dbContext,
            IQueryable<SCIMRepresentation> representations, 
            SearchSCIMRepresentationOrders order,
            int startIndex,
            int count,
            IEnumerable<SCIMAttributeExpression> includedAttributes,
            IEnumerable<SCIMAttributeExpression> excludedAttributes)
        {
            var attrExpression = expression as SCIMAttributeExpression;
            if (attrExpression == null)
            {
                return null;
            }

            var result = EvaluateOrderByMetadata(dbContext, 
                attrExpression, 
                representations, 
                order, 
                startIndex, 
                count,
                includedAttributes,
                excludedAttributes);
            if (result == null)
            {
                result = EvaluateOrderByProperty(
                    dbContext,
                    attrExpression, 
                    representations, 
                    order,
                    startIndex, 
                    count,
                    includedAttributes,
                    excludedAttributes);
            }

            return result;
        }

        private static SearchSCIMRepresentationsResponse EvaluateOrderByMetadata(
            SCIMDbContext dbContext,
            SCIMAttributeExpression attrExpression, 
            IQueryable<SCIMRepresentation> representations, 
            SearchSCIMRepresentationOrders order,
            int startIndex,
            int count,
            IEnumerable<SCIMAttributeExpression> includedAttributes,
            IEnumerable<SCIMAttributeExpression> excludedAttributes)
        {
            int total = representations.Count();
            var fullPath = attrExpression.GetFullPath();
            var record = ParserConstants.MappingStandardAttributePathToProperty.FirstOrDefault(kvp => string.Equals(kvp.Key, fullPath, StringComparison.InvariantCultureIgnoreCase));
            if (record.Equals(default(KeyValuePair<string, string>)) || string.IsNullOrWhiteSpace(record.Key)) return null;
            var representationParameter = Expression.Parameter(typeof(SCIMRepresentation), "rp");
            var propertyName = record.Value;
            var property = Expression.Property(representationParameter, record.Value);
            var propertyType = typeof(SCIMRepresentation).GetProperty(propertyName).PropertyType;
            var orderBy = GetOrderByType(order, propertyType);
            var innerLambda = Expression.Lambda(property, new ParameterExpression[] { representationParameter });
            var orderExpr = Expression.Call(orderBy, Expression.Constant(representations), innerLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<SCIMRepresentation>), "f");
            var finalOrderRequestBody = Expression.Lambda(orderExpr, new ParameterExpression[] { finalSelectArg });
            var result = (IQueryable<SCIMRepresentation>)finalOrderRequestBody.Compile().DynamicInvoke(representations);
            var content = result.Skip(startIndex <= 1 ? 0 : startIndex - 1).Take(count);
            return BuildResult(content, dbContext, includedAttributes, excludedAttributes, total);
        }

        private static SearchSCIMRepresentationsResponse EvaluateOrderByProperty(
            SCIMDbContext dbContext,
            SCIMAttributeExpression attrExpression, 
            IQueryable<SCIMRepresentation> representations, 
            SearchSCIMRepresentationOrders order,
            int startIndex,
            int count,
            IEnumerable<SCIMAttributeExpression> includedAttributes,
            IEnumerable<SCIMAttributeExpression> excludedAttributes)
        {
            int total = representations.Count();
            var lastChild = attrExpression.GetLastChild();
            Func<SCIMRepresentationAttribute, string> getResult = (attr) =>
            {
                switch(lastChild.SchemaAttribute.Type)
                {
                    case SCIMSchemaAttributeTypes.BOOLEAN:
                        return attr.ValueBoolean.ToString();
                    case SCIMSchemaAttributeTypes.INTEGER:
                        return attr.ValueInteger.ToString();
                    case SCIMSchemaAttributeTypes.DECIMAL:
                        return attr.ValueDecimal.ToString();
                    case SCIMSchemaAttributeTypes.DATETIME:
                        return attr.ValueDateTime.ToString();
                    case SCIMSchemaAttributeTypes.REFERENCE:
                        return attr.ValueReference.ToString();
                    case SCIMSchemaAttributeTypes.BINARY:
                        return attr.ValueBinary;
                    default:
                        return attr.ValueString;
                }
            };
            IQueryable<SCIMRepresentation> reprs;
            if(lastChild.SchemaAttribute != null)
            {
                var lastSchemaAttributeId = lastChild.SchemaAttribute.Id;
                var result = from s in representations
                             join attr in dbContext.SCIMRepresentationAttributeLst on new { id = s.Id, schemaAttrId = lastSchemaAttributeId } equals new { id = attr.RepresentationId, schemaAttrId = attr.SchemaAttributeId }
                             select new
                             {
                                 Attr = attr,
                                 Rep = s,
                                 orderedValueStr = (lastChild.SchemaAttribute.Id == attr.SchemaAttributeId) ? attr.ValueString : "",
                                 orderedValueBoo = (lastChild.SchemaAttribute.Id == attr.SchemaAttributeId) ? attr.ValueBoolean : default(bool),
                                 orderedValueInt = (lastChild.SchemaAttribute.Id == attr.SchemaAttributeId) ? attr.ValueInteger : default(int),
                                 orderedValueDec = (lastChild.SchemaAttribute.Id == attr.SchemaAttributeId) ? attr.ValueDecimal : default(decimal),
                                 orderedValueDat = (lastChild.SchemaAttribute.Id == attr.SchemaAttributeId) ? attr.ValueDateTime : default(DateTime),
                                 orderedValueRef = (lastChild.SchemaAttribute.Id == attr.SchemaAttributeId) ? attr.ValueReference : "",
                                 orderedValueBin = (lastChild.SchemaAttribute.Id == attr.SchemaAttributeId) ? attr.ValueBinary : "",
                             };
                switch (order)
                {
                    case SearchSCIMRepresentationOrders.Ascending:
                        switch (lastChild.SchemaAttribute.Type)
                        {
                            case SCIMSchemaAttributeTypes.INTEGER:
                                result = result.OrderBy(q => q.orderedValueInt);
                                break;
                            case SCIMSchemaAttributeTypes.BOOLEAN:
                                result = result.OrderBy(q => q.orderedValueBoo);
                                break;
                            case SCIMSchemaAttributeTypes.DECIMAL:
                                result = result.OrderBy(q => q.orderedValueDec);
                                break;
                            case SCIMSchemaAttributeTypes.DATETIME:
                                result = result.OrderBy(q => q.orderedValueDat);
                                break;
                            case SCIMSchemaAttributeTypes.REFERENCE:
                                result = result.OrderBy(q => q.orderedValueRef);
                                break;
                            case SCIMSchemaAttributeTypes.BINARY:
                                result = result.OrderBy(q => q.orderedValueBin);
                                break;
                            default:
                                result = result.OrderBy(q => q.orderedValueStr);
                                break;
                        }
                        break;
                    case SearchSCIMRepresentationOrders.Descending:
                        switch (lastChild.SchemaAttribute.Type)
                        {
                            case SCIMSchemaAttributeTypes.INTEGER:
                                result = result.OrderByDescending(q => q.orderedValueInt);
                                break;
                            case SCIMSchemaAttributeTypes.BOOLEAN:
                                result = result.OrderByDescending(q => q.orderedValueBoo);
                                break;
                            case SCIMSchemaAttributeTypes.DECIMAL:
                                result = result.OrderByDescending(q => q.orderedValueDec);
                                break;
                            case SCIMSchemaAttributeTypes.DATETIME:
                                result = result.OrderByDescending(q => q.orderedValueDat);
                                break;
                            case SCIMSchemaAttributeTypes.REFERENCE:
                                result = result.OrderByDescending(q => q.orderedValueRef);
                                break;
                            case SCIMSchemaAttributeTypes.BINARY:
                                result = result.OrderByDescending(q => q.orderedValueBin);
                                break;
                            default:
                                result = result.OrderByDescending(q => q.orderedValueStr);
                                break;
                        }
                        break;
                }

                reprs = result.Select(r => r.Rep);
            }
            else
            {
                reprs = representations.OrderBy(r => r.Id);
            }

            var content = reprs.Skip(startIndex <= 1 ? 0 : startIndex - 1).Take(count);
            return BuildResult(content, dbContext, includedAttributes, excludedAttributes, total);
        }

        private static MethodInfo GetOrderByType(SearchSCIMRepresentationOrders order, Type lastChildType)
        {
            var orderBy = typeof(Queryable).GetMethods()
                .Where(m => m.Name == "OrderBy" && m.IsGenericMethod)
                .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentation), lastChildType);
            if (order == SearchSCIMRepresentationOrders.Descending)
            {
                orderBy = typeof(Queryable).GetMethods()
                    .Where(m => m.Name == "OrderByDescending" && m.IsGenericMethod)
                    .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentation), lastChildType);
            }

            return orderBy;
        }

        private class RepresentationComparer : IComparer<SCIMRepresentation>
        {
            private readonly List<string> _orderedIds;

            public RepresentationComparer(List<string> orderedIds)
            {
                _orderedIds = orderedIds;
            }

            public int Compare(SCIMRepresentation x, SCIMRepresentation y)
            {
                var xIndex = _orderedIds.IndexOf(x.Id);
                var yIndex = _orderedIds.IndexOf(y.Id);
                if (xIndex < yIndex)
                {
                    return 1;
                }

                if (xIndex > yIndex)
                {
                    return -1;
                }

                return 0;
            }
        }

        #endregion
    }
}
