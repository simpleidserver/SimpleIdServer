// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver.Linq;
using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Parser.Operators;
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
            var lastChild = expression.LeftExpression.GetLastChild();
            var comparison = SCIMExpressionLinqExtensions.BuildComparisonExpression(expression, lastChild.SchemaAttribute,
                propertyValueString,
                propertyValueInteger,
                propertyValueDatetime,
                propertyValueBoolean,
                propertyValueDecimal,
                propertyValueBinary,
                parameterExpression);
            return Expression.And(Expression.Equal(schemaAttributeId, Expression.Constant(lastChild.SchemaAttribute.Id)), comparison);
        }

        #endregion
    }
}
