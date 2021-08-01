// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;
using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.EF.Extensions
{
    public static class EFSCIMExpressionLinqExtensions
    {
        #region Order By

        public static async Task<SearchSCIMRepresentationsResponse> EvaluateOrderBy(this SCIMExpression expression,
            SCIMDbContext dbContext,
            IQueryable<SCIMRepresentation> representations, 
            SearchSCIMRepresentationOrders order,
            int startIndex,
            int count,
            CancellationToken cancellationToken)
        {
            var attrExpression = expression as SCIMAttributeExpression;
            if (attrExpression == null)
            {
                return null;
            }

            var result = await EvaluateOrderByMetadata(attrExpression, representations, order, startIndex, count, cancellationToken);
            if (result == null)
            {
                result = await EvaluateOrderByProperty(dbContext, attrExpression, representations, order, startIndex, count, cancellationToken);
            }

            return result;
        }

        private static async Task<SearchSCIMRepresentationsResponse> EvaluateOrderByMetadata(
            SCIMAttributeExpression attrExpression, 
            IQueryable<SCIMRepresentation> representations, 
            SearchSCIMRepresentationOrders order,
            int startIndex,
            int count,
            CancellationToken cancellationToken)
        {
            var fullPath = attrExpression.GetFullPath();
            if (!SCIMConstants.MappingStandardAttributePathToProperty.ContainsKey(fullPath))
            {
                return null;
            }

            var representationParameter = Expression.Parameter(typeof(SCIMRepresentation), "rp");
            var propertyName = SCIMConstants.MappingStandardAttributePathToProperty[fullPath];
            var property = Expression.Property(representationParameter, SCIMConstants.MappingStandardAttributePathToProperty[fullPath]);
            var propertyType = typeof(SCIMRepresentation).GetProperty(propertyName).PropertyType;
            var orderBy = GetOrderByType(order, propertyType);
            var innerLambda = Expression.Lambda(property, new ParameterExpression[] { representationParameter });
            var orderExpr = Expression.Call(orderBy, Expression.Constant(representations), innerLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<SCIMRepresentation>), "f");
            var finalOrderRequestBody = Expression.Lambda(orderExpr, new ParameterExpression[] { finalSelectArg });
            var result = (IOrderedEnumerable<SCIMRepresentation>)finalOrderRequestBody.Compile().DynamicInvoke(representations);
            var content = result.Skip(startIndex).Take(count).ToList();
            var total = await representations.CountAsync(cancellationToken);
            return new SearchSCIMRepresentationsResponse(total, content);
        }

        private static async Task<SearchSCIMRepresentationsResponse> EvaluateOrderByProperty(
            SCIMDbContext dbContext,
            SCIMAttributeExpression attrExpression, 
            IQueryable<SCIMRepresentation> representations, 
            SearchSCIMRepresentationOrders order,
            int startIndex,
            int count,
            CancellationToken cancellationToken)
        {
            var lastChild = attrExpression.GetLastChild();
            IQueryable<string> query = null;
            switch(order)
            {
                case SearchSCIMRepresentationOrders.Ascending:
                    query = from rep in (from s in representations
                                         join attr in dbContext.SCIMRepresentationAttributeLst on s.Id equals attr.RepresentationId
                                         select new
                                         {
                                             representation = s,
                                             representationId = s.Id,
                                             orderedValue = (lastChild.SchemaAttribute.Id == attr.SchemaAttributeId) ? attr.ValueString : ""
                                         })
                            orderby rep.orderedValue ascending
                            select rep.representationId;
                    break;
                case SearchSCIMRepresentationOrders.Descending:
                    query = from rep in (from s in representations
                                         join attr in dbContext.SCIMRepresentationAttributeLst on s.Id equals attr.RepresentationId
                                         select new
                                         {
                                             representation = s,
                                             representationId = s.Id,
                                             orderedValue = (lastChild.SchemaAttribute.Id == attr.SchemaAttributeId) ? attr.ValueString : ""
                                         })
                            orderby rep.orderedValue descending
                            select rep.representationId;
                    break;
            }
            
            var orderedIds = await query.Skip(startIndex).Take(count).ToListAsync(cancellationToken);
            var result = await dbContext.SCIMRepresentationLst.Include(a => a.Attributes).Where(s => orderedIds.Contains(s.Id)).ToListAsync(cancellationToken);
            var comparer = new RepresentationComparer(orderedIds);
            List<SCIMRepresentation> content = null;
            switch (order)
            {
                case SearchSCIMRepresentationOrders.Ascending:
                    content = result.OrderBy(r => r, comparer).ToList();
                    break;
                case SearchSCIMRepresentationOrders.Descending:
                    content = result.OrderByDescending(r => r, comparer).ToList();
                    break;
            }

            var total = await representations.CountAsync(cancellationToken);
            return new SearchSCIMRepresentationsResponse(total, content);
        }

        private static MethodInfo GetOrderByType(SearchSCIMRepresentationOrders order, Type lastChildType)
        {
            var orderBy = typeof(Enumerable).GetMethods()
                .Where(m => m.Name == "OrderBy" && m.IsGenericMethod)
                .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentation), lastChildType);
            if (order == SearchSCIMRepresentationOrders.Descending)
            {
                orderBy = typeof(Enumerable).GetMethods()
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
