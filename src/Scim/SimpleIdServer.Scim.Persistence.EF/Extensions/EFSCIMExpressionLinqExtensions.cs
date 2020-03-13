// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Persistence.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        public static LambdaExpression Evaluate(this SCIMExpression expression, IQueryable<SCIMRepresentationModel> representations)
        {
            var representationParameter = Expression.Parameter(typeof(SCIMRepresentationModel), "rp");
            var anyLambdaExpression = expression.Evaluate(representationParameter);
            var enumarableType = typeof(Queryable);
            var whereMethod = enumarableType.GetMethods()
                 .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                 .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentationModel));
            var equalLambda = Expression.Lambda<Func<SCIMRepresentationModel, bool>>(anyLambdaExpression, representationParameter);
            var whereExpr = Expression.Call(whereMethod, Expression.Constant(representations), equalLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<SCIMRepresentationModel>), "f");
            var finalSelectRequestBody = Expression.Lambda(whereExpr, new ParameterExpression[] { finalSelectArg });
            return finalSelectRequestBody;
        }

        public static Expression Evaluate(this SCIMExpression expression, ParameterExpression parameterExpression)
        {
            var compAttrExpression = expression as SCIMComparisonExpression;
            var attrExpression = expression as SCIMAttributeExpression;
            var logicalExpression = expression as SCIMLogicalExpression;
            var notExpression = expression as SCIMNotExpression;
            var presentExpression = expression as SCIMPresentExpression;
            if (compAttrExpression != null)
            {
                return compAttrExpression.Evaluate(parameterExpression);
            }

            if (attrExpression != null)
            {
                return attrExpression.Evaluate(parameterExpression);
            }

            if (logicalExpression != null)
            {
                return logicalExpression.Evaluate(parameterExpression);
            }

            if (notExpression != null)
            {
                return notExpression.Evaluate(parameterExpression);
            }

            if (presentExpression != null)
            {
                return presentExpression.Evaluate(parameterExpression);
            }

            return null;
        }

        private static Expression Evaluate(this SCIMPresentExpression presentExpression, ParameterExpression parameterExpression)
        {
            if (parameterExpression.Type == typeof(SCIMRepresentationModel))
            {
                var commonAttribute = GetCommonAttribute(presentExpression.Content, parameterExpression);
                if (commonAttribute != null)
                {
                    return Expression.Equal(Expression.Constant(true), Expression.Constant(true));
                }

                var resourceAttrParameterExpr = Expression.Parameter(typeof(SCIMRepresentationAttributeModel), "ra");
                var anyLambdaExpression = presentExpression.Content.Evaluate(resourceAttrParameterExpr, (param) => BuildPresent(param));
                var attributesProperty = Expression.Property(parameterExpression, "Attributes");
                var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttributeModel, bool>>(anyLambdaExpression, resourceAttrParameterExpr);
                return Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttributeModel) }, attributesProperty, anyLambda);
            }

            return presentExpression.Content.Evaluate(parameterExpression, (param) => BuildPresent(param));
        }

        private static Expression Evaluate(this SCIMLogicalExpression logicalExpression, ParameterExpression parameterExpression)
        {
            if (parameterExpression.Type == typeof(SCIMRepresentationModel))
            {
                var raExpression = Expression.Parameter(typeof(SCIMRepresentationAttributeModel), "ra");
                var attributesProperty = Expression.Property(parameterExpression, "Attributes");
                Expression leftCall;
                Expression rightCall;
                if (logicalExpression.LeftExpression is SCIMComparisonExpression && IsCommonAttribute(((SCIMComparisonExpression)logicalExpression.LeftExpression).LeftExpression))
                {
                    leftCall = logicalExpression.LeftExpression.Evaluate(parameterExpression);
                }
                else
                {
                    var leftExpression = logicalExpression.LeftExpression.Evaluate(raExpression);
                    var anyLeftLambda = Expression.Lambda<Func<SCIMRepresentationAttributeModel, bool>>(leftExpression, raExpression);
                    leftCall = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttributeModel) }, attributesProperty, anyLeftLambda);
                }

                Expression rightExpression;
                if (logicalExpression.RightExpression is SCIMComparisonExpression && IsCommonAttribute(((SCIMComparisonExpression)logicalExpression.RightExpression).LeftExpression))
                {
                    rightCall = logicalExpression.RightExpression.Evaluate(parameterExpression);
                }
                else
                {
                    rightExpression = logicalExpression.RightExpression.Evaluate(raExpression);
                    var anyRightLambda = Expression.Lambda<Func<SCIMRepresentationAttributeModel, bool>>(rightExpression, raExpression);
                    rightCall = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttributeModel) }, attributesProperty, anyRightLambda);
                }

                if (logicalExpression.LogicalOperator == SCIMLogicalOperators.AND)
                {
                    return Expression.AndAlso(leftCall, rightCall);
                }

                return Expression.OrElse(leftCall, rightCall);
            }
            else
            {
                var leftExpression = logicalExpression.LeftExpression.Evaluate(parameterExpression);
                var rightExpression = logicalExpression.RightExpression.Evaluate(parameterExpression);
                if (logicalExpression.LogicalOperator == SCIMLogicalOperators.AND)
                {
                    return Expression.AndAlso(leftExpression, rightExpression);
                }

                return Expression.OrElse(leftExpression, rightExpression);
            }
        }

        private static Expression Evaluate(this SCIMAttributeExpression attributeExpression, ParameterExpression parameterExpression, Func<ParameterExpression, Expression> callback = null)
        {
            var originalParameterExpression = parameterExpression;
            if (parameterExpression.Type == typeof(SCIMRepresentationModel))
            {
                parameterExpression = Expression.Parameter(typeof(SCIMRepresentationAttributeModel), "ra");
            }

            var schemaAttributeProperty = Expression.Property(parameterExpression, "SchemaAttribute");
            var valuesProperty = Expression.Property(parameterExpression, "Children");
            var nameProperty = Expression.Property(schemaAttributeProperty, "Name");
            var notEqual = Expression.NotEqual(schemaAttributeProperty, Expression.Constant(null));
            var equal = Expression.Equal(nameProperty, Expression.Constant(attributeExpression.Name));
            var result = Expression.AndAlso(notEqual, equal);
            var complexAttributeExpression = attributeExpression as SCIMComplexAttributeExpression;
            if (complexAttributeExpression != null)
            {
                var logicalExpr = complexAttributeExpression.GroupingFilter as SCIMLogicalExpression;
                if (logicalExpr != null)
                {
                    var subParameter = Expression.Parameter(typeof(SCIMRepresentationAttributeModel), Guid.NewGuid().ToString("N"));
                    var leftExpr = logicalExpr.LeftExpression.Evaluate(subParameter);
                    var rightExpr = logicalExpr.RightExpression.Evaluate(subParameter);
                    var anyLeftLambda = Expression.Lambda<Func<SCIMRepresentationAttributeModel, bool>>(leftExpr, subParameter);
                    var anyRightLambda = Expression.Lambda<Func<SCIMRepresentationAttributeModel, bool>>(rightExpr, subParameter);
                    var anyLeftCall = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttributeModel) }, valuesProperty, anyLeftLambda);
                    var anyRightCall = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttributeModel) }, valuesProperty, anyRightLambda);
                    if (logicalExpr.LogicalOperator == SCIMLogicalOperators.AND)
                    {
                        result = Expression.AndAlso(result, Expression.AndAlso(anyLeftCall, anyRightCall));
                    }
                    else
                    {
                        result = Expression.AndAlso(result, Expression.Or(anyLeftCall, anyRightCall));
                    }
                }
                else
                {
                    var subParameter = Expression.Parameter(typeof(SCIMRepresentationAttributeModel), Guid.NewGuid().ToString("N"));
                    var lambdaExpression = complexAttributeExpression.GroupingFilter.Evaluate(subParameter);
                    var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttributeModel, bool>>(lambdaExpression, subParameter);
                    var anyCall = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttributeModel) }, valuesProperty, anyLambda);
                    result = Expression.AndAlso(result, anyCall);
                }
            }

            if (attributeExpression.Child != null)
            {
                var firstSubParameter = Expression.Parameter(typeof(SCIMRepresentationAttributeModel), Guid.NewGuid().ToString("N"));
                var secondSubParameter = Expression.Parameter(typeof(SCIMRepresentationAttributeModel), Guid.NewGuid().ToString("N"));
                var lambdaExpression = attributeExpression.Child.Evaluate(firstSubParameter, callback);
                var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttributeModel, bool>>(lambdaExpression, firstSubParameter);
                var anyCall = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttributeModel) }, valuesProperty, anyLambda);
                result = Expression.AndAlso(result, anyCall);
            }

            if (callback != null && attributeExpression.Child == null)
            {
                result = Expression.AndAlso(result, callback(parameterExpression));
            }

            if (originalParameterExpression.Type == typeof(SCIMRepresentationModel))
            {
                var attributesProperty = Expression.Property(originalParameterExpression, "Attributes");
                var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttributeModel, bool>>(result, parameterExpression);
                return Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttributeModel) }, attributesProperty, anyLambda);
            }

            return result;
        }

        private static Expression Evaluate(this SCIMNotExpression notExpression, ParameterExpression parameterExpression, Func<ParameterExpression, Expression> callback = null)
        {
            if (parameterExpression.Type == typeof(SCIMRepresentationModel))
            {
                var resourceAttrParameterExpr = Expression.Parameter(typeof(SCIMRepresentationAttributeModel), "ra");
                var anyLambdaExpression = Expression.Not(notExpression.Content.Evaluate(parameterExpression));
                var attributesProperty = Expression.Property(parameterExpression, "Attributes");
                var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttributeModel, bool>>(anyLambdaExpression, resourceAttrParameterExpr);
                return Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttributeModel) }, attributesProperty, anyLambda);
            }

            return Expression.Not(notExpression.Content.Evaluate(parameterExpression));
        }

        private static Expression Evaluate(this SCIMComparisonExpression comparisonExpression, ParameterExpression parameterExpression)
        {
            var originalParameterExpression = parameterExpression;
            if (parameterExpression.Type == typeof(SCIMRepresentationModel))
            {
                var commonAttribute = GetCommonAttribute(comparisonExpression.LeftExpression, parameterExpression);
                if (commonAttribute != null)
                {
                    return BuildComparison(comparisonExpression, commonAttribute);
                }

                parameterExpression = Expression.Parameter(typeof(SCIMRepresentationAttributeModel), "ra");
            }

            var result = comparisonExpression.LeftExpression.Evaluate(parameterExpression, (param) => BuildAttributeComparison(comparisonExpression, param));
            if (originalParameterExpression.Type == typeof(SCIMRepresentationModel))
            {
                var attributesProperty = Expression.Property(originalParameterExpression, "Attributes");
                var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttributeModel, bool>>(result, parameterExpression);
                return Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttributeModel) }, attributesProperty, anyLambda);
            }

            return result;
        }

        private static Expression BuildPresent(Expression representationAttrExpr)
        {
            var propertyValues = Expression.Property(representationAttrExpr, "Values");
            var countExpression = Expression.Call(typeof(Enumerable), "Count", new[] { typeof(SCIMRepresentationAttributeValueModel) }, propertyValues);
            return Expression.GreaterThanOrEqual(countExpression, Expression.Constant(1));
        }

        private static Expression BuildComparison(SCIMComparisonExpression comparisonExpression, MemberExpression representationExpr)
        {
            switch (comparisonExpression.ComparisonOperator)
            {
                case SCIMComparisonOperators.NE:
                    if (representationExpr.Type == typeof(DateTime))
                    {
                        return NotEqual(representationExpr, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(int))
                    {
                        return NotEqual(representationExpr, Expression.Constant(ParseInt(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(bool))
                    {
                        return NotEqual(representationExpr, Expression.Constant(ParseBoolean(comparisonExpression.Value)));
                    }

                    return NotEqual(representationExpr, Expression.Constant(comparisonExpression.Value));
                case SCIMComparisonOperators.GT:
                    if (representationExpr.Type == typeof(DateTime))
                    {
                        return GreaterThan(representationExpr, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(int))
                    {
                        return GreaterThan(representationExpr, Expression.Constant(ParseInt(comparisonExpression.Value)));
                    }

                    return GreaterThan(representationExpr, Expression.Constant(comparisonExpression.Value));
                case SCIMComparisonOperators.GE:
                    if (representationExpr.Type == typeof(DateTime))
                    {
                        return GreaterThanOrEqual(representationExpr, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(int))
                    {
                        return GreaterThanOrEqual(representationExpr, Expression.Constant(ParseInt(comparisonExpression.Value)));
                    }

                    return GreaterThanOrEqual(representationExpr, Expression.Constant(comparisonExpression.Value));
                case SCIMComparisonOperators.LE:
                    if (representationExpr.Type == typeof(DateTime))
                    {
                        return LessThanOrEqual(representationExpr, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(int))
                    {
                        return LessThanOrEqual(representationExpr, Expression.Constant(ParseInt(comparisonExpression.Value)));
                    }

                    return LessThanOrEqual(representationExpr, Expression.Constant(comparisonExpression.Value));
                case SCIMComparisonOperators.LT:
                    if (representationExpr.Type == typeof(DateTime))
                    {
                        return LessThan(representationExpr, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(int))
                    {
                        return LessThan(representationExpr, Expression.Constant(ParseInt(comparisonExpression.Value)));
                    }

                    return LessThan(representationExpr, Expression.Constant(comparisonExpression.Value));
                case SCIMComparisonOperators.EQ:
                    if (representationExpr.Type == typeof(DateTime))
                    {
                        return Equal(representationExpr, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(int))
                    {
                        return Equal(representationExpr, Expression.Constant(ParseInt(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(bool))
                    {
                        return Equal(representationExpr, Expression.Constant(ParseBoolean(comparisonExpression.Value)));
                    }

                    return Expression.Equal(representationExpr, Expression.Constant(comparisonExpression.Value));
                case SCIMComparisonOperators.SW:
                    var startWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                    return Expression.Call(representationExpr, startWith, Expression.Constant(comparisonExpression.Value));
                case SCIMComparisonOperators.EW:
                    var endWith = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
                    return Expression.Call(representationExpr, endWith, Expression.Constant(comparisonExpression.Value));
                case SCIMComparisonOperators.CO:
                    var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    return Expression.Call(representationExpr, contains, Expression.Constant(comparisonExpression.Value));
            }

            return null;
        }

        private static Expression BuildAttributeComparison(SCIMComparisonExpression comparisonExpression, Expression representationAttrExpr)
        {
            var propertySchemaAttribute = Expression.Property(representationAttrExpr, "SchemaAttribute");
            var propertySchemaType = Expression.Property(propertySchemaAttribute, "Type");
            var propertyValues = Expression.Property(representationAttrExpr, "Values");
            var attrValue = Expression.Parameter(typeof(SCIMRepresentationAttributeValueModel), "prop");
            var propertyValueString = Expression.Property(attrValue, "ValueString");
            var propertyValueInteger = Expression.Property(attrValue, "ValueInteger");
            var propertyValueDatetime = Expression.Property(attrValue, "ValueDateTime");
            var propertyValueBoolean = Expression.Property(attrValue, "ValueBoolean");
            var attrInteger = Expression.Parameter(typeof(int), "prop");
            var attrDateTime = Expression.Parameter(typeof(DateTime), "prop");
            var attrString = Expression.Parameter(typeof(string), "prop");
            var attrBoolean = Expression.Parameter(typeof(bool), "prop");
            Expression anyIntegerLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(Expression.Constant(false), attrValue),
                anyDateTimeLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(Expression.Constant(false), attrValue),
                anyStringLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(Expression.Constant(false), attrValue),
                anyBooleanLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(Expression.Constant(false), attrValue);
            switch (comparisonExpression.ComparisonOperator)
            {
                case SCIMComparisonOperators.NE:
                    anyIntegerLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(NotEqual(propertyValueInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrValue);
                    anyDateTimeLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(NotEqual(propertyValueDatetime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrValue);
                    anyStringLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(NotEqual(propertyValueString, Expression.Constant(comparisonExpression.Value)), attrValue);
                    anyBooleanLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(NotEqual(propertyValueBoolean, Expression.Constant(ParseBoolean(comparisonExpression.Value))), attrValue);
                    break;
                case SCIMComparisonOperators.GT:
                    anyIntegerLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(GreaterThan(propertyValueInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrValue);
                    anyDateTimeLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(GreaterThan(propertyValueDatetime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrValue);
                    break;
                case SCIMComparisonOperators.GE:
                    anyIntegerLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(GreaterThanOrEqual(propertyValueInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrValue);
                    anyDateTimeLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(GreaterThanOrEqual(propertyValueDatetime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrValue);
                    break;
                case SCIMComparisonOperators.LE:
                    anyIntegerLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(LessThanOrEqual(propertyValueInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrValue);
                    anyDateTimeLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(LessThanOrEqual(propertyValueDatetime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrValue);
                    break;
                case SCIMComparisonOperators.LT:
                    anyIntegerLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(LessThan(propertyValueInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrValue);
                    anyDateTimeLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(LessThan(propertyValueDatetime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrValue);
                    break;
                case SCIMComparisonOperators.EQ:
                    anyIntegerLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(Equal(propertyValueInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrValue);
                    anyDateTimeLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(Equal(propertyValueDatetime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrValue);
                    anyStringLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(Equal(propertyValueString, Expression.Constant(comparisonExpression.Value)), attrValue);
                    anyBooleanLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(Equal(propertyValueBoolean, Expression.Constant(ParseBoolean(comparisonExpression.Value))), attrValue);
                    break;
                case SCIMComparisonOperators.SW:
                    var startWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                    anyStringLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(Expression.Call(propertyValueString, startWith, Expression.Constant(comparisonExpression.Value)), attrValue);
                    break;
                case SCIMComparisonOperators.EW:
                    var endWith = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
                    anyStringLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(Expression.Call(propertyValueString, endWith, Expression.Constant(comparisonExpression.Value)), attrValue);
                    break;
                case SCIMComparisonOperators.CO:
                    var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    anyStringLambda = Expression.Lambda<Func<SCIMRepresentationAttributeValueModel, bool>>(Expression.Call(propertyValueString, contains, Expression.Constant(comparisonExpression.Value)), attrValue);
                    break;
            }

            var anyMethodType = typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Length == 2).MakeGenericMethod(typeof(SCIMRepresentationAttributeValueModel));
            var anyInteger = Expression.Call(anyMethodType, propertyValues, anyIntegerLambda);
            var anyDateTime = Expression.Call(anyMethodType, propertyValues, anyDateTimeLambda);
            var anyString = Expression.Call(anyMethodType, propertyValues, anyStringLambda);
            var anyBoolean = Expression.Call(anyMethodType, propertyValues, anyBooleanLambda);
            var equalValue = Expression.Or(
                Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.INTEGER)), anyInteger),
                Expression.Or(
                    Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.DATETIME)), anyDateTime),
                    Expression.Or(
                        Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.STRING)), anyString),
                        Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.BOOLEAN)), anyBoolean)
                    )
                )
            );

            return equalValue;
        }
        
        private static Expression LessThan(Expression e1, Expression e2)
        {
            if (IsNullableType(e1.Type) && !IsNullableType(e2.Type))
            {
                e2 = Expression.Convert(e2, e1.Type);
            }
            else if (!IsNullableType(e1.Type) && IsNullableType(e2.Type))
            {
                e1 = Expression.Convert(e1, e2.Type);
            }

            return Expression.LessThan(e1, e2);
        }

        private static Expression LessThanOrEqual(Expression e1, Expression e2)
        {
            if (IsNullableType(e1.Type) && !IsNullableType(e2.Type))
            {
                e2 = Expression.Convert(e2, e1.Type);
            }
            else if (!IsNullableType(e1.Type) && IsNullableType(e2.Type))
            {
                e1 = Expression.Convert(e1, e2.Type);
            }

            return Expression.LessThanOrEqual(e1, e2);
        }

        private static Expression Equal(Expression e1, Expression e2)
        {
            if (IsNullableType(e1.Type) && !IsNullableType(e2.Type))
            {
                e2 = Expression.Convert(e2, e1.Type);
            }
            else if (!IsNullableType(e1.Type) && IsNullableType(e2.Type))
            {
                e1 = Expression.Convert(e1, e2.Type);
            }

            return Expression.Equal(e1, e2);
        }

        private static Expression NotEqual(Expression e1, Expression e2)
        {
            if (IsNullableType(e1.Type) && !IsNullableType(e2.Type))
            {
                e2 = Expression.Convert(e2, e1.Type);
            }
            else if (!IsNullableType(e1.Type) && IsNullableType(e2.Type))
            {
                e1 = Expression.Convert(e1, e2.Type);
            }

            return Expression.NotEqual(e1, e2);
        }

        private static Expression GreaterThanOrEqual(Expression e1, Expression e2)
        {
            if (IsNullableType(e1.Type) && !IsNullableType(e2.Type))
            {
                e2 = Expression.Convert(e2, e1.Type);
            }
            else if (!IsNullableType(e1.Type) && IsNullableType(e2.Type))
            {
                e1 = Expression.Convert(e1, e2.Type);
            }

            return Expression.GreaterThanOrEqual(e1, e2);
        }

        private static Expression GreaterThan(Expression e1, Expression e2)
        {
            if (IsNullableType(e1.Type) && !IsNullableType(e2.Type))
            {
                e2 = Expression.Convert(e2, e1.Type);
            }
            else if (!IsNullableType(e1.Type) && IsNullableType(e2.Type))
            {
                e1 = Expression.Convert(e1, e2.Type);
            }

            return Expression.GreaterThan(e1, e2);
        }

        private static bool IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static bool ParseBoolean(string str)
        {
            bool result;
            if (bool.TryParse(str, out result))
            {
                return result;
            }

            return default(bool);
        }

        private static DateTime ParseDateTime(string str)
        {
            DateTime result;
            if (DateTime.TryParse(str, out result))
            {
                return result;
            }

            return default(DateTime);
        }

        private static int ParseInt(string str)
        {
            int result;
            if (int.TryParse(str, out result))
            {
                return result;
            }

            return default(int);
        }

        private static bool IsCommonAttribute(SCIMAttributeExpression scimAttributeExpression)
        {
            return MAPPING_PATH_TO_PROPERTYNAMES.ContainsKey(scimAttributeExpression.GetFullPath());
        }

        private static MemberExpression GetCommonAttribute(SCIMAttributeExpression scimAttributeExpression, ParameterExpression parameterExpression)
        {
            var fullPath = scimAttributeExpression.GetFullPath();
            if (!MAPPING_PATH_TO_PROPERTYNAMES.ContainsKey(fullPath))
            {
                return null;
            }

            return Expression.Property(parameterExpression, MAPPING_PATH_TO_PROPERTYNAMES[fullPath]);
        }
    }
}
