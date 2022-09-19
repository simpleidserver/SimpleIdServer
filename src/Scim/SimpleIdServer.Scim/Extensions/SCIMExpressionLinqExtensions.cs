// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Scim.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleIdServer.Scim.Parser.Expressions
{
    public static class SCIMExpressionLinqExtensions
    {
        #region Order By

        public static LambdaExpression EvaluateOrderBy(this SCIMExpression expression, IQueryable<SCIMRepresentation> representations, SearchSCIMRepresentationOrders order)
        {
            var attrExpression = expression as SCIMAttributeExpression;
            if (attrExpression == null)
            {
                return null;
            }

            var result = EvaluateOrderByMetadata(attrExpression, representations, order);
            if (result == null)
            {
                result = EvaluateOrderByProperty(attrExpression, representations, order);
            }

            return result;
        }

        public static LambdaExpression EvaluateOrderByMetadata(SCIMAttributeExpression attrExpression, IQueryable<SCIMRepresentation> representations, SearchSCIMRepresentationOrders order)
        {
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
            return finalOrderRequestBody;
        }

        public static LambdaExpression EvaluateOrderByProperty(SCIMAttributeExpression attrExpression, IQueryable<SCIMRepresentation> representations, SearchSCIMRepresentationOrders order)
        {
            var lastChild = attrExpression.GetLastChild();
            var lastChildType = SCIMConstants.MappingSchemaAttrTypeToType[lastChild.SchemaAttribute.Type];
            var orderBy = GetOrderByType(order, typeof(SCIMRepresentationAttribute));
            var firstOrDefaultMethodInfo = typeof(Enumerable).GetMethods()
                .Where(m => m.Name == "FirstOrDefault" && m.IsGenericMethod)
                .Where(m => m.GetParameters().Count() == 1).First().MakeGenericMethod(typeof(SCIMRepresentationAttribute));
            var whereMethodInfo = typeof(Enumerable).GetMethods()
                .Where(m => m.Name == "Where" && m.IsGenericMethod)
                .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentationAttribute));
            var attrRepresentation = Expression.Parameter(typeof(SCIMRepresentation), "r");
            var attrRepresentationAttribute = Expression.Parameter(typeof(SCIMRepresentationAttribute), "ra");
            var attributesProperty = Expression.Property(attrRepresentation, "FlatAttributes");
            var schemaAttributeIdProperty = Expression.Property(attrRepresentationAttribute, "SchemaAttributeId");
            var schemaAttributeIdEqualBody = Expression.Equal(schemaAttributeIdProperty, Expression.Constant(lastChild.SchemaAttribute.Id));
            var schemaAttributeIdEqualLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(schemaAttributeIdEqualBody, attrRepresentationAttribute);
            var whereExpr = Expression.Call(whereMethodInfo, attributesProperty, schemaAttributeIdEqualLambda);
            var selectExpr = Expression.Call(firstOrDefaultMethodInfo, whereExpr);
            var orderLambda = Expression.Lambda<Func<SCIMRepresentation, SCIMRepresentationAttribute>>(selectExpr, attrRepresentation);
            var orderExpr = Expression.Call(orderBy, Expression.Constant(representations), orderLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<SCIMRepresentation>), "f");
            var finalOrderRequestBody = Expression.Lambda(orderExpr, new ParameterExpression[] { finalSelectArg });
            return finalOrderRequestBody;
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


        #endregion
    }
}
