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
                var includedFullPathLst = (includedAttributes != null && includedAttributes.Any()) ? includedAttributes.Where(i => i is SCIMComplexAttributeExpression).Select(i => i.GetFullPath()) : new List<string>();
                result.FlatAttributes = filteredAttrs.ToList();
                return result;
            }

            representations = representations.Include(r => r.FlatAttributes).ThenInclude(s => s.SchemaAttribute);
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
            var result = from s in representations
                    join attr in dbContext.SCIMRepresentationAttributeLst on s.Id equals attr.RepresentationId
                    select new
                    {
                        Attr = attr,
                        Rep = s,
                        orderedValue = (lastChild.SchemaAttribute.Id == attr.SchemaAttributeId) ? attr.ValueString : ""
                    };
            switch (order)
            {
                case SearchSCIMRepresentationOrders.Ascending:
                    result = result.OrderBy(q => q.orderedValue);
                    break;
                case SearchSCIMRepresentationOrders.Descending:
                    result = result.OrderByDescending(q => q.orderedValue);
                    break;
            }

            var content = result.Select(r => r.Rep).Skip(startIndex <= 1 ? 0 : startIndex - 1).Take(count);
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
