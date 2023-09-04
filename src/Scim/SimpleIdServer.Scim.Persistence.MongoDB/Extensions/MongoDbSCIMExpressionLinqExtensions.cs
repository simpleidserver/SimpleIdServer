// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser;
using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Parser.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Extensions
{
    public static class MongoDbSCIMExpressionLinqExtensions
    {
        #region Filter attributes

        public static IQueryable<EnrichedAttribute> EvaluateMongoDbAttributes(this SCIMExpression expression, IQueryable<EnrichedAttribute> attributes)
        {
            var attr = expression as SCIMAttributeExpression;
            if (attr.SchemaAttribute == null || string.IsNullOrWhiteSpace(attr.SchemaAttribute.Id))
            {
                return new List<EnrichedAttribute>().AsQueryable();
            }
            else
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
                var childrenProperty = Expression.Property(parameterExpression, "Children");
                var enumerableType = typeof(Enumerable);
                var anyMethod = enumerableType.GetMethods()
                     .Where(m => m.Name == "Any" && m.IsGenericMethodDefinition)
                     .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(EnrichedAttribute));
                var childAttribute = Expression.Parameter(typeof(EnrichedAttribute), Guid.NewGuid().ToString());
                var anyLambdaBody = complex.GroupingFilter.EvaluateAttributes(childAttribute);
                var anyLambda = Expression.Lambda<Func<EnrichedAttribute, bool>>(anyLambdaBody, childAttribute);
                var anyCall = Expression.Call(anyMethod, childrenProperty, anyLambda);
                equal = Expression.AndAlso(equal, anyCall);
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

        public static LambdaExpression EvaluateMongoDbOrderBy(this SCIMExpression expression, IQueryable<EnrichedRepresentation> representations, SearchSCIMRepresentationOrders order)
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

        public static LambdaExpression EvaluateOrderByMetadata(SCIMAttributeExpression attrExpression, IQueryable<EnrichedRepresentation> representations, SearchSCIMRepresentationOrders order)
        {
            var fullPath = attrExpression.GetFullPath();
            var record = ParserConstants.MappingStandardAttributePathToProperty.FirstOrDefault(kvp => string.Equals(kvp.Key, fullPath, StringComparison.InvariantCultureIgnoreCase));
            if (record.Equals(default(KeyValuePair<string, string>)) || string.IsNullOrWhiteSpace(record.Key)) return null;
            var representationParameter = Expression.Parameter(typeof(EnrichedRepresentation), "rp");
            var propertyName = record.Value;
            var property = Expression.Property(Expression.Property(representationParameter, "Representation"), record.Value);
            var propertyType = typeof(SCIMRepresentation).GetProperty(propertyName).PropertyType;
            var orderBy = GetOrderByType(order, propertyType);
            var innerLambda = Expression.Lambda(property, new ParameterExpression[] { representationParameter });
            var orderExpr = Expression.Call(orderBy, Expression.Constant(representations), innerLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<EnrichedRepresentation>), "f");
            var finalOrderRequestBody = Expression.Lambda(orderExpr, new ParameterExpression[] { finalSelectArg });
            return finalOrderRequestBody;
        }

        public static LambdaExpression EvaluateOrderByProperty(SCIMAttributeExpression attrExpression, IQueryable<EnrichedRepresentation> representations, SearchSCIMRepresentationOrders order)
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
            var attrRepresentation = Expression.Parameter(typeof(EnrichedRepresentation), "r");
            var attrRepresentationAttribute = Expression.Parameter(typeof(SCIMRepresentationAttribute), "ra");
            var attributesProperty = Expression.Property(attrRepresentation, "FlatAttributes");
            var schemaAttributeIdProperty = Expression.Property(attrRepresentationAttribute, "SchemaAttributeId");
            var schemaAttributeIdEqualBody = Expression.Equal(schemaAttributeIdProperty, Expression.Constant(lastChild.SchemaAttribute.Id));
            var schemaAttributeIdEqualLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(schemaAttributeIdEqualBody, attrRepresentationAttribute);
            var whereExpr = Expression.Call(whereMethodInfo, attributesProperty, schemaAttributeIdEqualLambda);
            var selectExpr = Expression.Call(firstOrDefaultMethodInfo, whereExpr);
            var orderLambda = Expression.Lambda<Func<EnrichedRepresentation, SCIMRepresentationAttribute>>(selectExpr, attrRepresentation);
            var orderExpr = Expression.Call(orderBy, Expression.Constant(representations), orderLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<EnrichedRepresentation>), "f");
            var finalOrderRequestBody = Expression.Lambda(orderExpr, new ParameterExpression[] { finalSelectArg });
            return finalOrderRequestBody;
        }

        private static MethodInfo GetOrderByType(SearchSCIMRepresentationOrders order, Type lastChildType)
        {
            var orderBy = typeof(Enumerable).GetMethods()
                .Where(m => m.Name == "OrderBy" && m.IsGenericMethod)
                .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(EnrichedRepresentation), lastChildType);
            if (order == SearchSCIMRepresentationOrders.Descending)
            {
                orderBy = typeof(Enumerable).GetMethods()
                    .Where(m => m.Name == "OrderByDescending" && m.IsGenericMethod)
                    .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(EnrichedRepresentation), lastChildType);
            }

            return orderBy;
        }

        #endregion
    }
}
