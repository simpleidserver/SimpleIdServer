// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;
using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SimpleIdServer.Scim.Extensions
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

        #region Filter Attributes

        public static ICollection<SCIMRepresentationAttribute> EvaluateAttributes(this SCIMExpression expression, IQueryable<SCIMRepresentationAttribute> attributes, bool isStrictPath)
        {
            var attr = expression as SCIMAttributeExpression;
            if (attr.SchemaAttribute == null || string.IsNullOrWhiteSpace(attr.SchemaAttribute.Id))
            {
                return new List<SCIMRepresentationAttribute>();
            }
            else
            {
                var treeNodeParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), "tn");
                var anyWhereExpression = expression.EvaluateAttributes(treeNodeParameter);
                var enumarableType = typeof(Queryable);
                var whereMethod = enumarableType.GetMethods()
                     .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                     .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentationAttribute));
                var equalLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(anyWhereExpression, treeNodeParameter);
                var whereExpr = Expression.Call(whereMethod, Expression.Constant(attributes), equalLambda);
                var finalSelectArg = Expression.Parameter(typeof(IQueryable<SCIMRepresentationAttribute>), "f");
                var finalSelectRequestBody = Expression.Lambda(whereExpr, new ParameterExpression[] { finalSelectArg });
                var result = (IQueryable<SCIMRepresentationAttribute>)finalSelectRequestBody.Compile().DynamicInvoke(attributes);
                var fullPath = attr.GetFullPath();
                var res = SCIMRepresentation.BuildFlatAttributes(result.ToList());
                return res.Where((a) => a != null && (isStrictPath ? fullPath == a.FullPath : fullPath.StartsWith(a.FullPath) || a.FullPath.StartsWith(fullPath))).ToList();
            }
        }
        public static Expression EvaluateAttributes(this SCIMExpression expression, ParameterExpression parameterExpression)
        {
            var attrExpression = expression as SCIMAttributeExpression;
            var logicalExpression = expression as SCIMLogicalExpression;
            var comparisonExpression = expression as SCIMComparisonExpression;
            if (attrExpression != null)
            {
                return attrExpression.EvaluateAttributes(parameterExpression);
            }

            if (logicalExpression != null)
            {
                return logicalExpression.EvaluateAttributes(parameterExpression);
            }

            if (comparisonExpression != null)
            {
                return comparisonExpression.EvaluateAttributes(parameterExpression);
            }

            return null;
        }

        public static Expression EvaluateAttributes(this SCIMAttributeExpression expression, ParameterExpression parameterExpression)
        {
            var schemaAttributeIdProperty = Expression.Property(parameterExpression, "SchemaAttributeId");
            var equal = Expression.Equal(schemaAttributeIdProperty, Expression.Constant(expression.SchemaAttribute.Id));
            var complex = expression as SCIMComplexAttributeExpression;
            if (complex != null)
            {
                var childrenProperty = Expression.Property(parameterExpression, "Children");
                var enumerableType = typeof(Enumerable);
                var anyMethod = enumerableType.GetMethods()
                     .Where(m => m.Name == "Any" && m.IsGenericMethodDefinition)
                     .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentationAttribute));
                var childAttribute = Expression.Parameter(typeof(SCIMRepresentationAttribute), Guid.NewGuid().ToString());
                var anyLambdaBody = complex.GroupingFilter.EvaluateAttributes(childAttribute);
                var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(anyLambdaBody, childAttribute);
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
            var schemaAttributeId = Expression.Property(parameterExpression, "SchemaAttributeId");
            var propertyValueString = Expression.Property(parameterExpression, "ValueString");
            var propertyValueInteger = Expression.Property(parameterExpression, "ValueInteger");
            var propertyValueDatetime = Expression.Property(parameterExpression, "ValueDateTime");
            var propertyValueBoolean = Expression.Property(parameterExpression, "ValueBoolean");
            var propertyValueDecimal = Expression.Property(parameterExpression, "ValueDecimal");
            var propertyValueBinary = Expression.Property(parameterExpression, "ValueBinary");
            var comparison = BuildComparisonExpression(expression, expression.LeftExpression.SchemaAttribute,
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

        #region Filter Representations

        public static LambdaExpression Evaluate(this SCIMExpression expression, IQueryable<SCIMRepresentation> representations)
        {
            var representationParameter = Expression.Parameter(typeof(SCIMRepresentation), "rp");
            var anyLambdaExpression = expression.Evaluate(representationParameter);
            var enumarableType = typeof(Queryable);
            var whereMethod = enumarableType.GetMethods()
                 .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                 .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentation));
            var equalLambda = Expression.Lambda<Func<SCIMRepresentation, bool>>(anyLambdaExpression, representationParameter);
            var whereExpr = Expression.Call(whereMethod, Expression.Constant(representations), equalLambda);
            var finalSelectArg = Expression.Parameter(typeof(IQueryable<SCIMRepresentation>), "f");
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

        private static Expression Evaluate(this SCIMLogicalExpression logicalExpression, ParameterExpression parameterExpression)
        {
            switch (logicalExpression.LogicalOperator)
            {
                case SCIMLogicalOperators.AND:
                    return Expression.AndAlso(logicalExpression.LeftExpression.Evaluate(parameterExpression), logicalExpression.RightExpression.Evaluate(parameterExpression));
                default:
                    return Expression.OrElse(logicalExpression.LeftExpression.Evaluate(parameterExpression), logicalExpression.RightExpression.Evaluate(parameterExpression));
            }
        }

        private static Expression Evaluate(this SCIMNotExpression notExpression, ParameterExpression parameterExpression, Func<ParameterExpression, Expression> callback = null)
        {
            return Expression.Not(notExpression.Content.Evaluate(parameterExpression));
        }

        private static Expression Evaluate(this SCIMPresentExpression presentExpression, ParameterExpression parameterExpression)
        {
            var lastChild = presentExpression.Content.GetLastChild();
            var fullPath = presentExpression.Content.GetFullPath();
            if (SCIMConstants.MappingStandardAttributePathToProperty.ContainsKey(fullPath))
            {
                var name = SCIMConstants.MappingStandardAttributePathToProperty[fullPath];
                var prop = Expression.Property(parameterExpression, name);
                return Expression.NotEqual(prop, Expression.Constant(null));
            }

            var schemaAttr = lastChild.SchemaAttribute;
            var representationParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), "ra");
            var attributes = Expression.Property(parameterExpression, "FlatAttributes");
            var propertySchemaAttribute = Expression.Property(representationParameter, "SchemaAttributeId");
            var body = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(
                Expression.Equal(propertySchemaAttribute, Expression.Constant(schemaAttr.Id)),
                representationParameter);
            var anyMethodType = typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Length == 2).MakeGenericMethod(typeof(SCIMRepresentationAttribute));
            return Expression.Call(anyMethodType, attributes, body);
        }

        private static Expression Evaluate(this SCIMComparisonExpression comparisonExpression, ParameterExpression parameterExpression)
        {
            bool isMetadata = false;
            var lastChild = comparisonExpression.LeftExpression.GetLastChild();
            var fullPath = comparisonExpression.LeftExpression.GetFullPath();
            var representationParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), "ra");
            var propertySchemaAttribute = Expression.Property(representationParameter, "SchemaAttributeId");
            var propertyValueString = Expression.Property(representationParameter, "ValueString");
            var propertyValueInteger = Expression.Property(representationParameter, "ValueInteger");
            var propertyValueDatetime = Expression.Property(representationParameter, "ValueDateTime");
            var schemaAttr = lastChild.SchemaAttribute;
            if (SCIMConstants.MappingStandardAttributePathToProperty.ContainsKey(fullPath))
            {
                isMetadata = true;
                var name = SCIMConstants.MappingStandardAttributePathToProperty[fullPath];
                var type = SCIMConstants.MappingStandardAttributeTypeToType[fullPath];
                switch (type)
                {
                    case SCIMSchemaAttributeTypes.STRING:
                        propertyValueString = Expression.Property(parameterExpression, name);
                        schemaAttr = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                        {
                            Type = SCIMSchemaAttributeTypes.STRING
                        };
                        break;
                    case SCIMSchemaAttributeTypes.INTEGER:
                        propertyValueInteger = Expression.Property(parameterExpression, name);
                        schemaAttr = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                        {
                            Type = SCIMSchemaAttributeTypes.INTEGER
                        };
                        break;
                    case SCIMSchemaAttributeTypes.DATETIME:
                        propertyValueDatetime = Expression.Property(parameterExpression, name);
                        schemaAttr = new SCIMSchemaAttribute(Guid.NewGuid().ToString())
                        {
                            Type = SCIMSchemaAttributeTypes.DATETIME
                        };
                        break;
                }
            }

            var attributes = Expression.Property(parameterExpression, "FlatAttributes");
            var comparison = BuildComparisonExpression(comparisonExpression,
                schemaAttr, 
                propertyValueString, 
                propertyValueInteger, 
                propertyValueDatetime, 
                representationParameter: representationParameter);

            if (isMetadata)
            {
                return comparison;
            }

            var body = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(
                Expression.AndAlso(Expression.Equal(propertySchemaAttribute, Expression.Constant(schemaAttr.Id)), comparison),
                representationParameter);
            var anyMethodType = typeof(Enumerable).GetMethods().First(m => m.Name == "Any" && m.GetParameters().Length == 2).MakeGenericMethod(typeof(SCIMRepresentationAttribute));
            return Expression.Call(anyMethodType, attributes, body);
        }

        private static Expression Evaluate(this SCIMAttributeExpression attributeExpression, ParameterExpression parameterExpression)
        {
            var complexAttr = attributeExpression as SCIMComplexAttributeExpression;
            if (complexAttr == null)
            {
                return Expression.IsTrue(Expression.Constant(true));
            }

            return complexAttr.GroupingFilter.Evaluate(parameterExpression);
        }

        private static Expression BuildComparisonExpression(
            SCIMComparisonExpression comparisonExpression,
            SCIMSchemaAttribute schemaAttr,
            MemberExpression propertyValueString = null,
            MemberExpression propertyValueInteger = null,
            MemberExpression propertyValueDatetime = null,
            MemberExpression propertyValueBoolean = null,
            MemberExpression propertyValueDecimal = null,
            MemberExpression propertyValueBinary = null,
            ParameterExpression representationParameter = null)
        {
            representationParameter = representationParameter ?? Expression.Parameter(typeof(SCIMRepresentationAttribute), "ra");
            propertyValueString = propertyValueString ?? Expression.Property(representationParameter, "ValueString");
            propertyValueInteger = propertyValueInteger ?? Expression.Property(representationParameter, "ValueInteger");
            propertyValueDatetime = propertyValueDatetime ?? Expression.Property(representationParameter, "ValueDateTime");
            propertyValueBoolean = propertyValueBoolean ?? Expression.Property(representationParameter, "ValueBoolean");
            propertyValueDecimal = propertyValueDecimal ?? Expression.Property(representationParameter, "ValueDecimal");
            propertyValueBinary = propertyValueBinary ?? Expression.Property(representationParameter, "ValueBinary");
            Expression comparison = null;
            switch (comparisonExpression.ComparisonOperator)
            {
                case SCIMComparisonOperators.NE:
                    switch (schemaAttr.Type)
                    {
                        case SCIMSchemaAttributeTypes.STRING:
                            comparison = NotEqual(propertyValueString, Expression.Constant(comparisonExpression.Value));
                            break;
                        case SCIMSchemaAttributeTypes.INTEGER:
                            comparison = NotEqual(propertyValueInteger, Expression.Constant(ParseInt(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.DATETIME:
                            comparison = NotEqual(propertyValueDatetime, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.BOOLEAN:
                            comparison = NotEqual(propertyValueBoolean, Expression.Constant(ParseBoolean(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.DECIMAL:
                            comparison = NotEqual(propertyValueDecimal, Expression.Constant(ParseDecimal(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.BINARY:
                            comparison = NotEqual(propertyValueBinary, Expression.Constant(comparisonExpression.Value));
                            break;
                    }
                    break;
                case SCIMComparisonOperators.GT:
                    switch (schemaAttr.Type)
                    {
                        case SCIMSchemaAttributeTypes.INTEGER:
                            comparison = GreaterThan(propertyValueInteger, Expression.Constant(ParseInt(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.DATETIME:
                            comparison = GreaterThan(propertyValueDatetime, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.DECIMAL:
                            comparison = GreaterThan(propertyValueDecimal, Expression.Constant(ParseDecimal(comparisonExpression.Value)));
                            break;
                    }
                    break;
                case SCIMComparisonOperators.GE:
                    switch (schemaAttr.Type)
                    {
                        case SCIMSchemaAttributeTypes.INTEGER:
                            comparison = GreaterThanOrEqual(propertyValueInteger, Expression.Constant(ParseInt(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.DATETIME:
                            comparison = GreaterThanOrEqual(propertyValueDatetime, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.DECIMAL:
                            comparison = GreaterThanOrEqual(propertyValueDecimal, Expression.Constant(ParseDecimal(comparisonExpression.Value)));
                            break;
                    }
                    break;
                case SCIMComparisonOperators.LE:
                    switch (schemaAttr.Type)
                    {
                        case SCIMSchemaAttributeTypes.INTEGER:
                            comparison = LessThanOrEqual(propertyValueInteger, Expression.Constant(ParseInt(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.DATETIME:
                            comparison = LessThanOrEqual(propertyValueDatetime, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.DECIMAL:
                            comparison = LessThanOrEqual(propertyValueDecimal, Expression.Constant(ParseDecimal(comparisonExpression.Value)));
                            break;
                    }
                    break;
                case SCIMComparisonOperators.LT:
                    switch (schemaAttr.Type)
                    {
                        case SCIMSchemaAttributeTypes.INTEGER:
                            comparison = LessThan(propertyValueInteger, Expression.Constant(ParseInt(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.DATETIME:
                            comparison = LessThan(propertyValueDatetime, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.DECIMAL:
                            comparison = LessThan(propertyValueDecimal, Expression.Constant(ParseDecimal(comparisonExpression.Value)));
                            break;
                    }
                    break;
                case SCIMComparisonOperators.EQ:
                    switch (schemaAttr.Type)
                    {
                        case SCIMSchemaAttributeTypes.STRING:
                            comparison = Equal(propertyValueString, Expression.Constant(comparisonExpression.Value));
                            break;
                        case SCIMSchemaAttributeTypes.INTEGER:
                            comparison = Equal(propertyValueInteger, Expression.Constant(ParseInt(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.DATETIME:
                            comparison = Equal(propertyValueDatetime, Expression.Constant(ParseDateTime(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.BOOLEAN:
                            comparison = Equal(propertyValueBoolean, Expression.Constant(ParseBoolean(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.DECIMAL:
                            comparison = Equal(propertyValueDecimal, Expression.Constant(ParseDecimal(comparisonExpression.Value)));
                            break;
                        case SCIMSchemaAttributeTypes.BINARY:
                            comparison = Equal(propertyValueBinary, Expression.Constant(comparisonExpression.Value));
                            break;
                    }
                    break;
                case SCIMComparisonOperators.SW:
                    switch (schemaAttr.Type)
                    {
                        case SCIMSchemaAttributeTypes.STRING:
                            var startWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                            comparison = Expression.Call(propertyValueString, startWith, Expression.Constant(comparisonExpression.Value));
                            break;
                    }
                    break;
                case SCIMComparisonOperators.EW:
                    switch (schemaAttr.Type)
                    {
                        case SCIMSchemaAttributeTypes.STRING:
                            var endWith = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
                            comparison = Expression.Call(propertyValueString, endWith, Expression.Constant(comparisonExpression.Value));
                            break;
                    }
                    break;
                case SCIMComparisonOperators.CO:
                    if (schemaAttr.Type == SCIMSchemaAttributeTypes.STRING)
                    {
                        var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        comparison = Expression.Call(propertyValueString, contains, Expression.Constant(comparisonExpression.Value));
                    }
                    break;
            }

            return comparison;
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

        #endregion
    }
}
