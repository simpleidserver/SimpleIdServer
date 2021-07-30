// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;
using SimpleIdServer.Scim.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SimpleIdServer.Scim.Extensions
{
    public static class SCIMExpressionLinqExtensions
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

        #region Filter Attributes

        public static LambdaExpression Evaluate(this SCIMExpression expression, IQueryable<SCIMRepresentationAttribute> attributes)
        {
            var attrExpression = expression as SCIMAttributeExpression;
            if(attrExpression != null)
            {
                return attrExpression.Evaluate(attributes);
            }

            return null;
        }

        public static LambdaExpression Evaluate(this SCIMAttributeExpression expression, IQueryable<SCIMRepresentationAttribute> attributes)
        {
            var complex = expression as SCIMComplexAttributeExpression;
            if (complex == null)
            {
                var schemaAttr = expression.GetLastChild().SchemaAttribute;
                var representationParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), "ra");
                var propertySchemaAttribute = Expression.Property(representationParameter, "SchemaAttributeId");
                var enumarableType = typeof(Queryable);
                var body = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(
                    Expression.Equal(propertySchemaAttribute, Expression.Constant(schemaAttr.Id)),
                    representationParameter);
                var whereMethod = enumarableType.GetMethods()
                     .Where(m => m.Name == "Where" && m.IsGenericMethodDefinition)
                     .Where(m => m.GetParameters().Count() == 2).First().MakeGenericMethod(typeof(SCIMRepresentationAttribute));
                var whereExpr = Expression.Call(whereMethod, Expression.Constant(attributes), body);
                var finalSelectArg = Expression.Parameter(typeof(IQueryable<SCIMRepresentationAttribute>), "f");
                var finalSelectRequestBody = Expression.Lambda(whereExpr, new ParameterExpression[] { finalSelectArg });
                return finalSelectRequestBody;
            }

            // TODO : Continue the logic.
            return null;
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
            if (MAPPING_PATH_TO_PROPERTYNAMES.ContainsKey(fullPath))
            {
                var name = MAPPING_PATH_TO_PROPERTYNAMES[fullPath];
                var prop = Expression.Property(parameterExpression, name);
                return Expression.NotEqual(prop, Expression.Constant(null));
            }

            var schemaAttr = lastChild.SchemaAttribute;
            var representationParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), "ra");
            var attributes = Expression.Property(parameterExpression, "Attributes");
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
            var propertyValueString = Expression.Property(representationParameter, "ValueString");
            var propertyValueInteger = Expression.Property(representationParameter, "ValueInteger");
            var propertyValueDatetime = Expression.Property(representationParameter, "ValueDateTime");
            var schemaAttr = lastChild.SchemaAttribute;
            if (MAPPING_PATH_TO_PROPERTYNAMES.ContainsKey(fullPath))
            {
                isMetadata = true;
                var name = MAPPING_PATH_TO_PROPERTYNAMES[fullPath];
                var type = MAPPING_PROPERTY_TO_TYPES[fullPath];
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

            var attributes = Expression.Property(parameterExpression, "Attributes");
            var propertySchemaAttribute = Expression.Property(representationParameter, "SchemaAttributeId");
            var propertyValueBoolean = Expression.Property(representationParameter, "ValueBoolean");
            var propertyValueDecimal = Expression.Property(representationParameter, "ValueDecimal");
            var propertyValueBinary = Expression.Property(representationParameter, "ValueBinary");
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
