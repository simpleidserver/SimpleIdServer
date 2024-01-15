// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver.Linq;
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Parser.Operators;
using SimpleIdServer.Scim.Persistence.MongoDB.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Extensions
{
    public static class MongoDbSCIMExpressionLinqExtensions
    {
        #region Filter attributes

        public static IQueryable<EnrichedAttribute> EvaluateMongoDbAttributes(this SCIMExpression expression, IQueryable<EnrichedAttribute> attributes)
        {
            var treeNodeParameter = Expression.Parameter(typeof(EnrichedAttribute), "tn");
            var anyWhereExpression = expression.EvaluateAttributes(treeNodeParameter);
            var enumarableType = typeof(Queryable);
            var whereMethod = enumarableType.GetMethods()
                 .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                 .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(EnrichedAttribute));
            var equalLambda = Expression.Lambda<Func<EnrichedAttribute, bool>>(anyWhereExpression, treeNodeParameter);
            var whereExpr = Expression.Call(whereMethod, Expression.Constant(attributes), equalLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<EnrichedAttribute>), "f");
            var finalSelectRequestBody = Expression.Lambda(whereExpr, new ParameterExpression[] { finalSelectArg });
            var result = (IQueryable<EnrichedAttribute>)finalSelectRequestBody.Compile().DynamicInvoke(attributes);
            return result;
        }

        public static Expression EvaluateAttributes(this SCIMExpression expression, ParameterExpression parameterExpression)
        {
            var attrExpression = expression as SCIMAttributeExpression;
            var logicalExpression = expression as SCIMLogicalExpression;
            var comparisonExpression = expression as SCIMComparisonExpression;
            if (attrExpression != null) return attrExpression.EvaluateAttributes(parameterExpression);
            if (logicalExpression != null) return logicalExpression.EvaluateAttributes(parameterExpression);
            if (comparisonExpression != null) return comparisonExpression.EvaluateAttributes(parameterExpression);
            return null;
        }

        public static Expression EvaluateAttributes(this SCIMAttributeExpression expression, ParameterExpression parameterExpression)
        {
            var schemaAttributeIdProperty = Expression.Property(Expression.Property(parameterExpression, "Attribute"), "SchemaAttributeId");
            var equal = Expression.Equal(schemaAttributeIdProperty, Expression.Constant(expression.SchemaAttribute.Id));
            var complex = expression as SCIMComplexAttributeExpression;
            if (complex != null)
            {
                return complex.GroupingFilter.EvaluateAttributes(parameterExpression);
            }

            return equal;
        }

        public static Expression EvaluateAttributes(this SCIMLogicalExpression expression, ParameterExpression parameterExpression)
        {
            switch (expression.LogicalOperator)
            {
                case SCIMLogicalOperators.AND:
                    return Expression.AndAlso(expression.LeftExpression.EvaluateAttributes(parameterExpression), expression.RightExpression.EvaluateAttributes(parameterExpression));
                default:
                    return Expression.OrElse(expression.LeftExpression.EvaluateAttributes(parameterExpression), expression.RightExpression.EvaluateAttributes(parameterExpression));
            }
        }

        public static Expression EvaluateAttributes(this SCIMComparisonExpression expression, ParameterExpression parameterExpression)
        {
            var attr = Expression.Property(parameterExpression, "Attribute");
            var schemaAttributeId = Expression.Property(attr, "SchemaAttributeId");
            var propertyValueString = Expression.Property(attr, "ValueString");
            var propertyValueInteger = Expression.Property(attr, "ValueInteger");
            var propertyValueDatetime = Expression.Property(attr, "ValueDateTime");
            var propertyValueBoolean = Expression.Property(attr, "ValueBoolean");
            var propertyValueDecimal = Expression.Property(attr, "ValueDecimal");
            var propertyValueBinary = Expression.Property(attr, "ValueBinary");
            var comparison = SCIMExpressionLinqExtensions.BuildComparisonExpression(expression, expression.LeftExpression.SchemaAttribute,
                propertyValueString,
                propertyValueInteger,
                propertyValueDatetime,
                propertyValueBoolean,
                propertyValueDecimal,
                propertyValueBinary,
                parameterExpression);
            return Expression.And(Expression.Equal(schemaAttributeId, Expression.Constant(expression.LeftExpression.SchemaAttribute.Id)), comparison);
        }

        #endregion

        #region Order By

        public static IOrderedMongoQueryable<SCIMRepresentationModel> EvaluateMongoDbOrderBy(this SCIMExpression expression, IMongoQueryable<SCIMRepresentationModel> representations, SearchSCIMRepresentationOrders order)
        {
            var attrExpression = expression as SCIMAttributeExpression;
            if (attrExpression == null)
            {
                return null;
            }

            var result = EvaluateOrderByMetadata(attrExpression, representations, order);
            return result;
        }

        public static IOrderedMongoQueryable<SCIMRepresentationModel> EvaluateOrderByMetadata(SCIMAttributeExpression attrExpression, IMongoQueryable<SCIMRepresentationModel> representations, SearchSCIMRepresentationOrders order)
        {
            var fullPath = attrExpression.GetFullPath();
            switch(fullPath)
            {
                case StandardSCIMRepresentationAttributes.Id:
                    return order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.Id) : representations.OrderByDescending(r => r.Id);
                case StandardSCIMRepresentationAttributes.ExternalId:
                    return order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.ExternalId) : representations.OrderByDescending(r => r.ExternalId);
                case $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.ResourceType}":
                    return order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.ResourceType) : representations.OrderByDescending(r => r.ResourceType);
                case $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Created}":
                    return order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.Created) : representations.OrderByDescending(r => r.Created);
                case $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.LastModified}":
                    return order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.LastModified) : representations.OrderByDescending(r => r.LastModified);
                case $"{StandardSCIMRepresentationAttributes.Meta}.{StandardSCIMMetaAttributes.Version}":
                    return order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.Version) : representations.OrderByDescending(r => r.Version);
                case StandardSCIMRepresentationAttributes.DisplayName:
                    return order == SearchSCIMRepresentationOrders.Ascending ? representations.OrderBy(r => r.DisplayName) : representations.OrderByDescending(r => r.DisplayName);
            }

            return representations.OrderBy(r => r.Id);
        }

        #endregion
    }
}
