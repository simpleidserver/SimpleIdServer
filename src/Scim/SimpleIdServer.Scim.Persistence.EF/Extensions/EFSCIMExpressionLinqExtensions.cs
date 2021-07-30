// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.Persistence.EF.Extensions
{
    public static class EFSCIMExpressionLinqExtensions
    {
        private static Dictionary<string, string> MAPPING_PATH_TO_PROPERTYNAMES = new Dictionary<string, string>
        {
            { SCIMConstants.StandardSCIMRepresentationAttributes.Id, "Id" },
            { SCIMConstants.StandardSCIMRepresentationAttributes.ExternalId, "ExternalId" },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.ResourceType}", "ResourceType" },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.Created}", "Created" },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.LastModified}", "LastModified" },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.Version}", "Version" },
        };

        private static Dictionary<string, SCIMSchemaAttributeTypes> MAPPING_PROPERTY_TO_TYPES = new Dictionary<string, SCIMSchemaAttributeTypes>
        {
            { SCIMConstants.StandardSCIMRepresentationAttributes.Id, SCIMSchemaAttributeTypes.STRING},
            { SCIMConstants.StandardSCIMRepresentationAttributes.ExternalId, SCIMSchemaAttributeTypes.STRING},
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.ResourceType}", SCIMSchemaAttributeTypes.STRING },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.Created}", SCIMSchemaAttributeTypes.DATETIME },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.LastModified}", SCIMSchemaAttributeTypes.DATETIME },
            { $"{SCIMConstants.StandardSCIMRepresentationAttributes.Meta}.{SCIMConstants.StandardSCIMMetaAttributes.Version}", SCIMSchemaAttributeTypes.INTEGER }
        };

        /*
        #region Order By

        public static async Task<SearchSCIMRepresentationsResponse> EvaluateOrderBy(this SCIMExpression expression,
            SCIMDbContext dbContext,
            IQueryable<SCIMRepresentationModel> representations, 
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
            IQueryable<SCIMRepresentationModel> representations, 
            SearchSCIMRepresentationOrders order,
            int startIndex,
            int count,
            CancellationToken cancellationToken)
        {
            var fullPath = attrExpression.GetFullPath();
            if (!MAPPING_PATH_TO_PROPERTYNAMES.ContainsKey(fullPath))
            {
                return null;
            }

            var representationParameter = Expression.Parameter(typeof(SCIMRepresentationModel), "rp");
            var propertyName = MAPPING_PATH_TO_PROPERTYNAMES[fullPath];
            var property = Expression.Property(representationParameter, MAPPING_PATH_TO_PROPERTYNAMES[fullPath]);
            var propertyType = typeof(SCIMRepresentationModel).GetProperty(propertyName).PropertyType;
            var orderBy = GetOrderByType(order, propertyType);
            var innerLambda = Expression.Lambda(property, new ParameterExpression[] { representationParameter });
            var orderExpr = Expression.Call(orderBy, Expression.Constant(representations), innerLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<SCIMRepresentationModel>), "f");
            var finalOrderRequestBody = Expression.Lambda(orderExpr, new ParameterExpression[] { finalSelectArg });
            var result = (IOrderedEnumerable<SCIMRepresentationModel>)finalOrderRequestBody.Compile().DynamicInvoke(representations);
            var content = result.Skip(startIndex).Take(count).ToList();
            var total = await representations.CountAsync(cancellationToken);
            return new SearchSCIMRepresentationsResponse(total, content.Select(r => r.ToDomain()));
        }

        private static async Task<SearchSCIMRepresentationsResponse> EvaluateOrderByProperty(
            SCIMDbContext dbContext,
            SCIMAttributeExpression attrExpression, 
            IQueryable<SCIMRepresentationModel> representations, 
            SearchSCIMRepresentationOrders order,
            int startIndex,
            int count,
            CancellationToken cancellationToken)
        {
            var lastChild = attrExpression.GetLastChild();
            var reps = await representations.ToListAsync(cancellationToken);
            var query = reps.Select(r =>  
            {
                var orderValue = string.Empty;
                var attr = r.Attributes.FirstOrDefault(a => a.SchemaAttributeId == lastChild.SchemaAttribute.Id);
                if (attr != null && attr.Values.Any())
                {
                    orderValue = attr.Values.First().ValueString;
                }

                return new
                {
                    result = r,
                    orderValue = orderValue
                };
            });
            List<SCIMRepresentationModel> content= null;
            switch(order)
            {
                case SearchSCIMRepresentationOrders.Ascending:
                    content = query.OrderBy(r => r.orderValue).Select(r => r.result).ToList();
                    break;
                case SearchSCIMRepresentationOrders.Descending:
                    content = query.OrderByDescending(r => r.orderValue).Select(r => r.result).ToList();
                    break;
            }

            content = content.Skip(startIndex).Take(count).ToList();
            var total = reps.Count();
            return new SearchSCIMRepresentationsResponse(total, content.Select(r => r.ToDomain()));
        }

        public class GroupedResult
        {
            public string RepresentationId { get; set; }
            public string OrderedValue { get; set; }
        }

        private static MethodInfo GetOrderByType(SearchSCIMRepresentationOrders order, Type lastChildType)
        {
            var orderBy = typeof(Enumerable).GetMethods()
                .Where(m => m.Name == "OrderBy" && m.IsGenericMethod)
                .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentationModel), lastChildType);
            if (order == SearchSCIMRepresentationOrders.Descending)
            {
                orderBy = typeof(Enumerable).GetMethods()
                    .Where(m => m.Name == "OrderByDescending" && m.IsGenericMethod)
                    .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentationModel), lastChildType);
            }

            return orderBy;
        }

        #endregion
        */
    }
}
