// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Persistence.MongoDB.Models;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Extensions
{
    public static class MongoDBSCIMExpressionLinqExtensions
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
                if (originalParameterExpression.Type == typeof(SCIMRepresentationModel))
                {
                    var lambdaExpression = complexAttributeExpression.GroupingFilter.Evaluate(parameterExpression);
                    result = Expression.AndAlso(result, lambdaExpression);
                }
                else
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
            var propertySchemaAttribute = Expression.Property(representationAttrExpr, "SchemaAttribute");
            var propertySchemaType = Expression.Property(propertySchemaAttribute, "Type");
            var propertyValuesString = Expression.Property(representationAttrExpr, "ValuesString");
            var propertyValuesBoolean = Expression.Property(representationAttrExpr, "ValuesBoolean");
            var propertyValuesInt = Expression.Property(representationAttrExpr, "ValuesInteger");
            var propertyValuesDateTime = Expression.Property(representationAttrExpr, "ValuesDateTime");
            var propertyValuesDecimal = Expression.Property(representationAttrExpr, "ValuesDecimal");
            var propertyValuesBinary = Expression.Property(representationAttrExpr, "ValuesBinary");
            var countIntegerExpression = Expression.Call(typeof(Enumerable), "Count", new[] { typeof(int) }, propertyValuesInt);
            var countStringExpression = Expression.Call(typeof(Enumerable), "Count", new[] { typeof(string) }, propertyValuesString);
            var countDateTimeExpression = Expression.Call(typeof(Enumerable), "Count", new[] { typeof(DateTime) }, propertyValuesDateTime);
            var countBooleanExpression = Expression.Call(typeof(Enumerable), "Count", new[] { typeof(bool) }, propertyValuesBoolean);
            var countDecimalExpression = Expression.Call(typeof(Enumerable), "Count", new[] { typeof(decimal) }, propertyValuesDecimal);
            var countBinaryExpression = Expression.Call(typeof(Enumerable), "Count", new[] { typeof(byte[]) }, propertyValuesBinary);
            return Expression.Or(
                Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.INTEGER)), Expression.GreaterThan(countIntegerExpression, Expression.Constant(0))),
                Expression.Or(
                    Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.STRING)), Expression.GreaterThan(countStringExpression, Expression.Constant(0))),
                    Expression.Or(
                        Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.DATETIME)), Expression.GreaterThan(countDateTimeExpression, Expression.Constant(0))),
                        Expression.Or(
                            Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.BOOLEAN)), Expression.GreaterThan(countBooleanExpression, Expression.Constant(0))),
                            Expression.Or(
                                Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.DECIMAL)), Expression.GreaterThan(countDecimalExpression, Expression.Constant(0))),
                                Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.BINARY)), Expression.GreaterThan(countBinaryExpression, Expression.Constant(0)))
                            )
                        )
                    )
                )
            );
        }

        private static Expression BuildComparison(SCIMComparisonExpression comparisonExpression, MemberExpression representationExpr)
        {
            switch (comparisonExpression.ComparisonOperator)
            {
                case SCIMComparisonOperators.NE:
                    if (representationExpr.Type == typeof(DateTime))
                    {
                        return Expression.NotEqual(representationExpr, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(int))
                    {
                        return Expression.NotEqual(representationExpr, Expression.Constant(ParseInt(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(bool))
                    {
                        return Expression.NotEqual(representationExpr, Expression.Constant(ParseBoolean(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(decimal))
                    {
                        return Expression.NotEqual(representationExpr, Expression.Constant(ParseDecimal(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(byte[]))
                    {
                        return Expression.NotEqual(representationExpr, Expression.Constant(ParseBinary(comparisonExpression.Value)));
                    }

                    return Expression.NotEqual(representationExpr, Expression.Constant(comparisonExpression.Value));
                case SCIMComparisonOperators.GT:
                    if (representationExpr.Type == typeof(byte[]) || representationExpr.Type == typeof(bool))
                    {
                        throw new SCIMFilterException(string.Format(Global.GreaterThanNotSupported, representationExpr.Type.Name));
                    }

                    if (representationExpr.Type == typeof(DateTime))
                    {
                        return Expression.GreaterThan(representationExpr, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(int))
                    {
                        return Expression.GreaterThan(representationExpr, Expression.Constant(ParseInt(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(decimal))
                    {
                        return Expression.GreaterThan(representationExpr, Expression.Constant(ParseDecimal(comparisonExpression.Value)));
                    }


                    return Expression.GreaterThan(representationExpr, Expression.Constant(comparisonExpression.Value));
                case SCIMComparisonOperators.GE:
                    if (representationExpr.Type == typeof(byte[]) || representationExpr.Type == typeof(bool))
                    {
                        throw new SCIMFilterException(string.Format(Global.GreaterThanOrEqualNotSupported, representationExpr.Type.Name));
                    }

                    if (representationExpr.Type == typeof(DateTime))
                    {
                        return Expression.GreaterThanOrEqual(representationExpr, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(int))
                    {
                        return Expression.GreaterThanOrEqual(representationExpr, Expression.Constant(ParseInt(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(decimal))
                    {
                        return Expression.GreaterThanOrEqual(representationExpr, Expression.Constant(ParseDecimal(comparisonExpression.Value)));
                    }

                    return Expression.GreaterThanOrEqual(representationExpr, Expression.Constant(comparisonExpression.Value));
                case SCIMComparisonOperators.LE:
                    if (representationExpr.Type == typeof(byte[]) || representationExpr.Type == typeof(bool))
                    {
                        throw new SCIMFilterException(string.Format(Global.LessThanOrEqualNotSupported, representationExpr.Type.Name));
                    }

                    if (representationExpr.Type == typeof(DateTime))
                    {
                        return Expression.LessThanOrEqual(representationExpr, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(int))
                    {
                        return Expression.LessThanOrEqual(representationExpr, Expression.Constant(ParseInt(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(decimal))
                    {
                        return Expression.LessThanOrEqual(representationExpr, Expression.Constant(ParseDecimal(comparisonExpression.Value)));
                    }

                    return Expression.LessThanOrEqual(representationExpr, Expression.Constant(comparisonExpression.Value));
                case SCIMComparisonOperators.LT:
                    if (representationExpr.Type == typeof(byte[]) || representationExpr.Type == typeof(bool))
                    {
                        throw new SCIMFilterException(string.Format(Global.LessThanNotSupported, representationExpr.Type.Name));
                    }

                    if (representationExpr.Type == typeof(DateTime))
                    {
                        return Expression.LessThan(representationExpr, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(int))
                    {
                        return Expression.LessThan(representationExpr, Expression.Constant(ParseInt(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(decimal))
                    {
                        return Expression.LessThan(representationExpr, Expression.Constant(ParseDecimal(comparisonExpression.Value)));
                    }

                    return Expression.LessThan(representationExpr, Expression.Constant(comparisonExpression.Value));
                case SCIMComparisonOperators.EQ:
                    if (representationExpr.Type == typeof(DateTime))
                    {
                        return Expression.Equal(representationExpr, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(int))
                    {
                        return Expression.Equal(representationExpr, Expression.Constant(ParseInt(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(bool))
                    {
                        return Expression.Equal(representationExpr, Expression.Constant(ParseBoolean(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(decimal))
                    {
                        return Expression.Equal(representationExpr, Expression.Constant(ParseDecimal(comparisonExpression.Value)));
                    }

                    if (representationExpr.Type == typeof(byte[]))
                    {
                        return Expression.Equal(representationExpr, Expression.Constant(ParseBinary(comparisonExpression.Value)));
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
            var caseExactType = Expression.Property(propertySchemaAttribute, "CaseExact");
            var propertySchemaType = Expression.Property(propertySchemaAttribute, "Type");
            var propertyValuesString = Expression.Property(representationAttrExpr, "ValuesString");
            var propertyValuesBoolean = Expression.Property(representationAttrExpr, "ValuesBoolean");
            var propertyValuesInt = Expression.Property(representationAttrExpr, "ValuesInteger");
            var propertyValuesDateTime = Expression.Property(representationAttrExpr, "ValuesDateTime");
            var propertyValuesDecimal = Expression.Property(representationAttrExpr, "ValuesDecimal");
            var propertyValuesBinary = Expression.Property(representationAttrExpr, "ValuesByte");
            var attrInteger = Expression.Parameter(typeof(int), "prop");
            var attrDateTime = Expression.Parameter(typeof(DateTime), "prop");
            var attrString = Expression.Parameter(typeof(string), "prop");
            var attrBoolean = Expression.Parameter(typeof(bool), "prop");
            var attrDecimal = Expression.Parameter(typeof(decimal), "prop");
            var attrBinary = Expression.Parameter(typeof(byte[]), "prop");
            Expression anyIntegerLambda = Expression.Lambda<Func<int, bool>>(Expression.Constant(false), attrInteger),
                anyDateTimeLambda = Expression.Lambda<Func<DateTime, bool>>(Expression.Constant(false), attrDateTime),
                anyCaseSensitiveStringLambda = Expression.Lambda<Func<string, bool>>(Expression.Constant(false), attrString),
                anyCaseNotSensitiveStringLambda = Expression.Equal(Expression.Constant(true), Expression.Constant(false)),
                anyBooleanLambda = Expression.Lambda<Func<bool, bool>>(Expression.Constant(false), attrBoolean),
                anyDecimalLambda = Expression.Lambda<Func<decimal, bool>>(Expression.Constant(false), attrDecimal),
                anyBinaryLambda = Expression.Lambda<Func<byte[], bool>>(Expression.Constant(false), attrBinary);
            var toLower = Expression.Call(Expression.Constant(comparisonExpression.Value), typeof(string).GetMethod("ToLower", Type.EmptyTypes));
            var containsStringMethod = typeof(Enumerable).GetMethods().First(m2 => m2.Name == "Contains" && m2.GetParameters().Count() == 2).MakeGenericMethod(typeof(string));
            Expression equalValue = null;
            switch (comparisonExpression.ComparisonOperator)
            {
                case SCIMComparisonOperators.NE:
                    anyIntegerLambda = Expression.Lambda<Func<int, bool>>(Expression.NotEqual(attrInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrInteger);
                    anyDateTimeLambda = Expression.Lambda<Func<DateTime, bool>>(Expression.NotEqual(attrDateTime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrDateTime);
                    anyCaseSensitiveStringLambda = Expression.Lambda<Func<string, bool>>(Expression.NotEqual(attrString, Expression.Constant(comparisonExpression.Value)), attrString);
                    anyCaseNotSensitiveStringLambda = Expression.Not(Expression.Call(containsStringMethod, propertyValuesString, toLower));
                    anyBooleanLambda = Expression.Lambda<Func<bool, bool>>(Expression.NotEqual(attrBoolean, Expression.Constant(ParseBoolean(comparisonExpression.Value))), attrBoolean);
                    anyDecimalLambda = Expression.Lambda<Func<decimal, bool>>(Expression.NotEqual(attrDecimal, Expression.Constant(ParseDecimal(comparisonExpression.Value))), attrDecimal);
                    anyBinaryLambda = Expression.Lambda<Func<byte[], bool>>(Expression.NotEqual(attrBinary, Expression.Constant(ParseBinary(comparisonExpression.Value))), attrBinary);
                    break;
                case SCIMComparisonOperators.GT:
                    anyIntegerLambda = Expression.Lambda<Func<int, bool>>(Expression.GreaterThan(attrInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrInteger);
                    anyDateTimeLambda = Expression.Lambda<Func<DateTime, bool>>(Expression.GreaterThan(attrDateTime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrDateTime);
                    anyDecimalLambda = Expression.Lambda<Func<decimal, bool>>(Expression.GreaterThan(attrDecimal, Expression.Constant(ParseDecimal(comparisonExpression.Value))), attrDecimal);
                    break;
                case SCIMComparisonOperators.GE:
                    anyIntegerLambda = Expression.Lambda<Func<int, bool>>(Expression.GreaterThanOrEqual(attrInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrInteger);
                    anyDateTimeLambda = Expression.Lambda<Func<DateTime, bool>>(Expression.GreaterThanOrEqual(attrDateTime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrDateTime);
                    anyDecimalLambda = Expression.Lambda<Func<decimal, bool>>(Expression.GreaterThanOrEqual(attrDecimal, Expression.Constant(ParseDecimal(comparisonExpression.Value))), attrDecimal);
                    break;
                case SCIMComparisonOperators.LE:
                    anyIntegerLambda = Expression.Lambda<Func<int, bool>>(Expression.LessThanOrEqual(attrInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrInteger);
                    anyDateTimeLambda = Expression.Lambda<Func<DateTime, bool>>(Expression.LessThanOrEqual(attrDateTime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrDateTime);
                    anyDecimalLambda = Expression.Lambda<Func<decimal, bool>>(Expression.LessThanOrEqual(attrDecimal, Expression.Constant(ParseDecimal(comparisonExpression.Value))), attrDecimal);
                    break;
                case SCIMComparisonOperators.LT:
                    anyIntegerLambda = Expression.Lambda<Func<int, bool>>(Expression.LessThan(attrInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrInteger);
                    anyDateTimeLambda = Expression.Lambda<Func<DateTime, bool>>(Expression.LessThan(attrDateTime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrDateTime);
                    anyDecimalLambda = Expression.Lambda<Func<decimal, bool>>(Expression.LessThan(attrDecimal, Expression.Constant(ParseDecimal(comparisonExpression.Value))), attrDecimal);
                    break;
                case SCIMComparisonOperators.EQ:
                    anyIntegerLambda = Expression.Lambda<Func<int, bool>>(Expression.Equal(attrInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrInteger);
                    anyDateTimeLambda = Expression.Lambda<Func<DateTime, bool>>(Expression.Equal(attrDateTime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrDateTime);
                    anyCaseSensitiveStringLambda = Expression.Lambda<Func<string, bool>>(Expression.Equal(attrString, Expression.Constant(comparisonExpression.Value)), attrString);
                    anyCaseNotSensitiveStringLambda = Expression.Call(containsStringMethod, propertyValuesString, toLower);
                    anyBooleanLambda = Expression.Lambda<Func<bool, bool>>(Expression.Equal(attrBoolean, Expression.Constant(ParseBoolean(comparisonExpression.Value))), attrBoolean);
                    anyDecimalLambda = Expression.Lambda<Func<decimal, bool>>(Expression.Equal(attrDecimal, Expression.Constant(ParseDecimal(comparisonExpression.Value))), attrDecimal);
                    anyBinaryLambda = Expression.Lambda<Func<byte[], bool>>(Expression.NotEqual(attrBinary, Expression.Constant(ParseBinary(comparisonExpression.Value))), attrBinary);
                    break;
                case SCIMComparisonOperators.SW:
                    {
                        var startWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                        var toLowerAttrStr = Expression.Call(attrString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                        var anyCaseNotSensitiveStringLambdaBody = Expression.Lambda<Func<string, bool>>(Expression.Call(toLowerAttrStr, startWith, toLower), attrString);
                        anyCaseSensitiveStringLambda = Expression.Lambda<Func<string, bool>>(Expression.Call(attrString, startWith, Expression.Constant(comparisonExpression.Value)), attrString);
                        anyCaseNotSensitiveStringLambda = Expression.Call(typeof(Enumerable).GetMethods().First(m2 => m2.Name == "Any" && m2.GetParameters().Count() == 2).MakeGenericMethod(typeof(string)), propertyValuesString, anyCaseNotSensitiveStringLambdaBody);
                    }
                    break;
                case SCIMComparisonOperators.EW:
                    {
                        var endWith = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
                        var toLowerAttrStr = Expression.Call(attrString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                        var anyCaseNotSensitiveStringLambdaBody = Expression.Lambda<Func<string, bool>>(Expression.Call(toLowerAttrStr, endWith, toLower), attrString);
                        anyCaseSensitiveStringLambda = Expression.Lambda<Func<string, bool>>(Expression.Call(attrString, endWith, Expression.Constant(comparisonExpression.Value)), attrString);
                        anyCaseNotSensitiveStringLambda = Expression.Call(typeof(Enumerable).GetMethods().First(m2 => m2.Name == "Any" && m2.GetParameters().Count() == 2).MakeGenericMethod(typeof(string)), propertyValuesString, anyCaseNotSensitiveStringLambdaBody);
                    }
                    break;
                case SCIMComparisonOperators.CO:
                    {
                        var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        var toLowerAttrStr = Expression.Call(attrString, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
                        var anyCaseNotSensitiveStringLambdaBody = Expression.Lambda<Func<string, bool>>(Expression.Call(toLowerAttrStr, contains, toLower), attrString);
                        anyCaseSensitiveStringLambda = Expression.Lambda<Func<string, bool>>(Expression.Call(attrString, contains, Expression.Constant(comparisonExpression.Value)), attrString);
                        anyCaseNotSensitiveStringLambda = Expression.Call(typeof(Enumerable).GetMethods().First(m2 => m2.Name == "Any" && m2.GetParameters().Count() == 2).MakeGenericMethod(typeof(string)), propertyValuesString, anyCaseNotSensitiveStringLambdaBody);
                    }
                    break;
            }

            var allInteger = Expression.Call(typeof(Enumerable).GetMethods().First(m2 => m2.Name == "Any" && m2.GetParameters().Count() == 2).MakeGenericMethod(typeof(int)), propertyValuesInt, anyIntegerLambda);
            var allDateTime = Expression.Call(typeof(Enumerable).GetMethods().First(m2 => m2.Name == "Any" && m2.GetParameters().Count() == 2).MakeGenericMethod(typeof(DateTime)), propertyValuesDateTime, anyDateTimeLambda);
            var allString = Expression.Or(
                    Expression.And(Expression.Equal(caseExactType, Expression.Constant(true)), Expression.Call(typeof(Enumerable).GetMethods().First(m2 => m2.Name == "Any" && m2.GetParameters().Count() == 2).MakeGenericMethod(typeof(string)), propertyValuesString, anyCaseSensitiveStringLambda)),
                    Expression.And(Expression.Equal(caseExactType, Expression.Constant(false)), anyCaseNotSensitiveStringLambda)
                );
            var allBoolean = Expression.Call(typeof(Enumerable).GetMethods().First(m2 => m2.Name == "Any" && m2.GetParameters().Count() == 2).MakeGenericMethod(typeof(bool)), propertyValuesBoolean, anyBooleanLambda);
            var allDecimal = Expression.Call(typeof(Enumerable).GetMethods().First(m2 => m2.Name == "Any" && m2.GetParameters().Count() == 2).MakeGenericMethod(typeof(decimal)), propertyValuesDecimal, anyDecimalLambda);
            var allBinary = Expression.Call(typeof(Enumerable).GetMethods().First(m2 => m2.Name == "Any" && m2.GetParameters().Count() == 2).MakeGenericMethod(typeof(byte[])), propertyValuesBinary, anyBinaryLambda);
            equalValue = Expression.Or(
                Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.INTEGER)), allInteger),
                Expression.Or(
                    Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.DATETIME)), allDateTime),
                    Expression.Or(
                        Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.STRING)), allString),
                        Expression.Or(
                            Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.BOOLEAN)), allBoolean),
                            Expression.Or(
                                Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.DECIMAL)), allDecimal),
                                Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.BINARY)), allBinary)
                            )
                        )
                    )
                )
            );
            return equalValue;
        }

        private static bool ParseBoolean(string str)
        {
            bool result;
            if (!bool.TryParse(str, out result))
            {
                return result;
            }

            return default(bool);
        }

        private static byte[] ParseBinary(string str)
        {
            var result = new byte[0];
            if (string.IsNullOrWhiteSpace(str))
            {
                return result;
            }

            try
            {
                return Convert.FromBase64String(str);
            }
            catch
            {
                return result;
            }
        }

        private static decimal ParseDecimal(string str)
        {
            decimal result;
            if (decimal.TryParse(str, out result))
            {
                return result;
            }

            return default(decimal);
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
