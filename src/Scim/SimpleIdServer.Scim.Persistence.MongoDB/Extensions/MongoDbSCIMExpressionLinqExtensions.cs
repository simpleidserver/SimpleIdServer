// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver.Linq;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser;
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
        #region Filter Metadata

        public static IMongoQueryable<SCIMRepresentationModel> EvaluateMongoDbRepresentations(this SCIMExpression expression, IMongoQueryable<SCIMRepresentationModel> representations)
        {
            var treeNodeParameter = Expression.Parameter(typeof(SCIMRepresentationModel), "tn");
            var anyWhereExpression = expression.EvaluateRepresentations(treeNodeParameter);
            if (anyWhereExpression == null) return null;
            var enumarableType = typeof(Queryable);
            var whereMethod = enumarableType.GetMethods()
                 .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                 .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentationModel));
            var equalLambda = Expression.Lambda<Func<SCIMRepresentationModel, bool>>(anyWhereExpression, treeNodeParameter);
            var whereExpr = Expression.Call(whereMethod, Expression.Constant(representations), equalLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<SCIMRepresentationModel>), "f");
            var finalSelectRequestBody = Expression.Lambda(whereExpr, new ParameterExpression[] { finalSelectArg });
            var result = (IMongoQueryable<SCIMRepresentationModel>)finalSelectRequestBody.Compile().DynamicInvoke(representations);
            return result;
        }

        public static Expression EvaluateRepresentations(this SCIMExpression expression, ParameterExpression parameterExpression)
        {
            var logicalExpression = expression as SCIMLogicalExpression;
            var comparisonExpression = expression as SCIMComparisonExpression;
            if (logicalExpression != null) return logicalExpression.EvaluateRepresentations(parameterExpression);
            if (comparisonExpression != null) return comparisonExpression.EvaluateRepresentations(parameterExpression);
            return null;
        }

        public static Expression EvaluateRepresentations(this SCIMLogicalExpression expression, ParameterExpression parameterExpression)
        {
            var leftExpression = expression.LeftExpression.EvaluateRepresentations(parameterExpression);
            var rightExpression = expression.RightExpression.EvaluateRepresentations(parameterExpression);
            if (leftExpression == null && rightExpression == null) return null;
            if (leftExpression != null && rightExpression == null) return leftExpression;
            if (leftExpression == null && rightExpression != null) return rightExpression;
            switch (expression.LogicalOperator)
            {
                case SCIMLogicalOperators.AND:
                    return Expression.AndAlso(leftExpression, rightExpression);
                default:
                    return Expression.OrElse(leftExpression, rightExpression);
            }
        }

        public static Expression EvaluateRepresentations(this SCIMComparisonExpression expression, ParameterExpression parameterExpression)
        {
            var lastChild = expression.LeftExpression.GetLastChild();
            var fullPath = lastChild.GetFullPath();
            if (!ParserConstants.MappingStandardAttributePathToProperty.ContainsKey(fullPath)) return null;
            var propertyName = ParserConstants.MappingStandardAttributePathToProperty[fullPath];
            var type = ParserConstants.MappingStandardAttributeTypeToType[fullPath];
            MemberExpression propertyValueString = null,
                propertyValueInteger = null,
                propertyValueDatetime = null;
            var schemaAttr = lastChild.SchemaAttribute;
            switch (type)
            {
                case SCIMSchemaAttributeTypes.STRING:
                    propertyValueString = Expression.Property(parameterExpression, propertyName);
                    schemaAttr = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                    {
                        Type = SCIMSchemaAttributeTypes.STRING
                    };
                    break;
                case SCIMSchemaAttributeTypes.INTEGER:
                    propertyValueInteger = Expression.Property(parameterExpression, propertyName);
                    schemaAttr = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                    {
                        Type = SCIMSchemaAttributeTypes.INTEGER
                    };
                    break;
                case SCIMSchemaAttributeTypes.DATETIME:
                    propertyValueDatetime = Expression.Property(parameterExpression, propertyName);
                    schemaAttr = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                    {
                        Type = SCIMSchemaAttributeTypes.DATETIME
                    };
                    break;
            }

            return SCIMExpressionLinqExtensions.BuildComparisonExpression(
                expression,
                schemaAttr,
                propertyValueString,
                propertyValueInteger,
                propertyValueDatetime,
                representationParameter: Expression.Parameter(typeof(SCIMRepresentationAttribute), "tn"));
        }


        #endregion

        #region Filter attributes

        public static IMongoQueryable<EnrichedAttribute> EvaluateMongoDbAttributes(this SCIMExpression expression, IMongoQueryable<EnrichedAttribute> attributes)
        {
            var treeNodeParameter = Expression.Parameter(typeof(EnrichedAttribute), "tn");
            var anyWhereExpression = expression.EvaluateAttributes(treeNodeParameter);
            if (anyWhereExpression == null) return null;
            var enumarableType = typeof(Queryable);
            var whereMethod = enumarableType.GetMethods()
                 .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                 .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(EnrichedAttribute));
            var equalLambda = Expression.Lambda<Func<EnrichedAttribute, bool>>(anyWhereExpression, treeNodeParameter);
            var whereExpr = Expression.Call(whereMethod, Expression.Constant(attributes), equalLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<EnrichedAttribute>), "f");
            var finalSelectRequestBody = Expression.Lambda(whereExpr, new ParameterExpression[] { finalSelectArg });
            var result = (IMongoQueryable<EnrichedAttribute>)finalSelectRequestBody.Compile().DynamicInvoke(attributes);
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
            var leftExpression = expression.LeftExpression.EvaluateAttributes(parameterExpression);
            var rightExpression = expression.RightExpression.EvaluateAttributes(parameterExpression);
            if (leftExpression == null && rightExpression == null) return null;
            if (leftExpression != null && rightExpression == null) return leftExpression;
            if (leftExpression == null && rightExpression != null) return rightExpression;
            switch (expression.LogicalOperator)
            {
                case SCIMLogicalOperators.AND:
                    return Expression.AndAlso(leftExpression, rightExpression);
                default:
                    return Expression.OrElse(leftExpression, rightExpression);
            }
        }

        public static Expression EvaluateAttributes(this SCIMComparisonExpression expression, ParameterExpression parameterExpression)
        {
            var lastChild = expression.LeftExpression.GetLastChild();
            if (ParserConstants.MappingStandardAttributePathToProperty.ContainsKey(lastChild.GetFullPath())) return null;
            var attr = Expression.Property(parameterExpression, "Attribute");
            var schemaAttributeId = Expression.Property(attr, "SchemaAttributeId");
            var propertyValueString = Expression.Property(attr, "ValueString");
            var propertyValueInteger = Expression.Property(attr, "ValueInteger");
            var propertyValueDatetime = Expression.Property(attr, "ValueDateTime");
            var propertyValueBoolean = Expression.Property(attr, "ValueBoolean");
            var propertyValueDecimal = Expression.Property(attr, "ValueDecimal");
            var propertyValueBinary = Expression.Property(attr, "ValueBinary");
            
            // Use MongoDB-optimized comparison expressions for string types to avoid $expr performance issues
            Expression comparison;
            if (lastChild.SchemaAttribute.Type == SCIMSchemaAttributeTypes.STRING && !lastChild.SchemaAttribute.CaseExact)
            {
                comparison = BuildMongoDbOptimizedStringComparison(expression, propertyValueString);
            }
            else
            {
                comparison = SCIMExpressionLinqExtensions.BuildComparisonExpression(expression, lastChild.SchemaAttribute,
                    propertyValueString,
                    propertyValueInteger,
                    propertyValueDatetime,
                    propertyValueBoolean,
                    propertyValueDecimal,
                    propertyValueBinary,
                    parameterExpression);
            }
            
            return Expression.And(Expression.Equal(schemaAttributeId, Expression.Constant(lastChild.SchemaAttribute.Id)), comparison);
        }

        /// <summary>
        /// Builds MongoDB-optimized string comparison expressions that avoid $expr and can use indexes effectively
        /// </summary>
        private static Expression BuildMongoDbOptimizedStringComparison(SCIMComparisonExpression expression, MemberExpression propertyValueString)
        {
            var value = expression.Value;
            
            switch (expression.ComparisonOperator)
            {
                case SCIMComparisonOperators.EQ:
                    return MongoDbOptimizedExpressionExtensions.CreateOptimizedCaseInsensitiveEqual(propertyValueString, value);
                
                case SCIMComparisonOperators.NE:
                    var equalExpr = MongoDbOptimizedExpressionExtensions.CreateOptimizedCaseInsensitiveEqual(propertyValueString, value);
                    return Expression.Not(equalExpr);
                
                case SCIMComparisonOperators.SW:
                    return MongoDbOptimizedExpressionExtensions.CreateOptimizedCaseInsensitiveStartsWith(propertyValueString, value);
                
                case SCIMComparisonOperators.EW:
                    return MongoDbOptimizedExpressionExtensions.CreateOptimizedCaseInsensitiveEndsWith(propertyValueString, value);
                
                case SCIMComparisonOperators.CO:
                    return MongoDbOptimizedExpressionExtensions.CreateOptimizedCaseInsensitiveContains(propertyValueString, value);
                
                default:
                    // For other operators (GT, LT, GE, LE, PR), fall back to default behavior
                    // as they don't typically benefit from case-insensitive optimization
                    return SCIMExpressionLinqExtensions.BuildComparisonExpression(
                        expression, 
                        new SCIMSchemaAttribute("temp") { Type = SCIMSchemaAttributeTypes.STRING, CaseExact = false },
                        propertyValueString);
            }
        }

        #endregion
    }
}
