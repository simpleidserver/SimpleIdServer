using SimpleIdServer.Persistence.Filters;
using SimpleIdServer.Persistence.Filters.SCIMExpressions;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SimpleIdServer.Scim.Extensions
{
    public static class SCIMExpressionLinqExtensions
    {
        public static LambdaExpression Evaluate(this SCIMExpression expression, IQueryable<SCIMRepresentation> representations)
        {
            var representationParameter = Expression.Parameter(typeof(SCIMRepresentation), "rp");
            var anyLambdaExpression = expression.Evaluate(representationParameter, true);
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

        private static Expression Evaluate(this SCIMExpression expression, ParameterExpression parameterExpression, bool isRoot = false)
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
                if (isRoot)
                {
                    throw new SCIMFilterException("bad_filter", "cannot be used to filter representations");
                }

                return attrExpression.Evaluate(parameterExpression);
            }

            if (logicalExpression != null)
            {
                return logicalExpression.Evaluate(parameterExpression);
            }

            if(notExpression != null)
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
            if(parameterExpression.Type == typeof(SCIMRepresentation))
            {
                var resourceAttrParameterExpr = Expression.Parameter(typeof(SCIMRepresentationAttribute), "ra");
                var anyLambdaExpression = presentExpression.Content.Evaluate(resourceAttrParameterExpr, (param) => BuildPresent(param));
                var attributesProperty = Expression.Property(parameterExpression, "Attributes");
                var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(anyLambdaExpression, resourceAttrParameterExpr);
                return Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttribute) }, attributesProperty, anyLambda);
            }

            return presentExpression.Content.Evaluate(parameterExpression, (param) => BuildPresent(param));
        }

        private static Expression Evaluate(this SCIMLogicalExpression logicalExpression, ParameterExpression parameterExpression)
        {
            var leftExpression = logicalExpression.LeftExpression.Evaluate(parameterExpression);
            var rightExpression = logicalExpression.RightExpression.Evaluate(parameterExpression);
            if (logicalExpression.LogicalOperator == SCIMLogicalOperators.AND)
            {
                return Expression.AndAlso(leftExpression, rightExpression);
            }

            return Expression.Or(leftExpression, rightExpression);
        }

        private static Expression Evaluate(this SCIMAttributeExpression attributeExpression, ParameterExpression parameterExpression, Func<ParameterExpression, Expression> callback = null)
        {
            var complexAttributeExpression = attributeExpression as SCIMComplexAttributeExpression;
            var schemaAttributeProperty = Expression.Property(parameterExpression, "SchemaAttribute");
            var valuesProperty = Expression.Property(parameterExpression, "Values");
            var nameProperty = Expression.Property(schemaAttributeProperty, "Name");
            var notEqual = Expression.NotEqual(schemaAttributeProperty, Expression.Constant(null));
            var equal = Expression.Equal(nameProperty, Expression.Constant(attributeExpression.Name));
            var result = Expression.AndAlso(notEqual, equal);
            if (attributeExpression.Child != null)
            {
                var firstSubParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), Guid.NewGuid().ToString("N"));
                var secondSubParameter = Expression.Parameter(typeof(SCIMRepresentationAttribute), Guid.NewGuid().ToString("N"));
                var lambdaExpression = attributeExpression.Child.Evaluate(firstSubParameter, callback);
                var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(lambdaExpression, firstSubParameter);
                var anyCall = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttribute) }, valuesProperty, anyLambda);
                result = Expression.AndAlso(result, anyCall);
                if (complexAttributeExpression != null)
                {
                    lambdaExpression = complexAttributeExpression.GroupingFilter.Evaluate(secondSubParameter);
                    var secondAnyLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(lambdaExpression, secondSubParameter);
                    var secondAnyCall = Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttribute) }, valuesProperty, secondAnyLambda);
                    result = Expression.AndAlso(result, secondAnyCall);
                }
            }

            if (callback != null && attributeExpression.Child == null)
            {
                result = Expression.AndAlso(result, callback(parameterExpression));
            }

            return result;
        }

        private static Expression Evaluate(this SCIMNotExpression notExpression, ParameterExpression parameterExpression, Func<ParameterExpression, Expression> callback = null)
        {
            if (parameterExpression.Type == typeof(SCIMRepresentation))
            {
                var resourceAttrParameterExpr = Expression.Parameter(typeof(SCIMRepresentationAttribute), "ra");
                var anyLambdaExpression = Expression.Not(notExpression.Content.Evaluate(parameterExpression));
                var attributesProperty = Expression.Property(parameterExpression, "Attributes");
                var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(anyLambdaExpression, resourceAttrParameterExpr);
                return Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttribute) }, attributesProperty, anyLambda);
            }

            return Expression.Not(notExpression.Content.Evaluate(parameterExpression));
        }

        private static Expression Evaluate(this SCIMComparisonExpression comparisonExpression, ParameterExpression arg)
        {
            if (arg.Type == typeof(SCIMRepresentation))
            {
                var resourceAttrParameterExpr = Expression.Parameter(typeof(SCIMRepresentationAttribute), "ra");
                var anyLambdaExpression = comparisonExpression.LeftExpression.Evaluate(resourceAttrParameterExpr, (param) => BuildComparison(comparisonExpression, param));
                var attributesProperty = Expression.Property(arg, "Attributes");
                var anyLambda = Expression.Lambda<Func<SCIMRepresentationAttribute, bool>>(anyLambdaExpression, resourceAttrParameterExpr);
                return Expression.Call(typeof(Enumerable), "Any", new[] { typeof(SCIMRepresentationAttribute) }, attributesProperty, anyLambda);
            }

            return comparisonExpression.LeftExpression.Evaluate(arg, (param) => BuildComparison(comparisonExpression, param));
        }

        private static Expression BuildPresent(Expression representationAttrExpr)
        {
            var propertySchemaAttribute = Expression.Property(representationAttrExpr, "SchemaAttribute");
            var propertySchemaType = Expression.Property(propertySchemaAttribute, "Type");
            var propertyValuesString = Expression.Property(representationAttrExpr, "ValuesString");
            var propertyValuesBoolean = Expression.Property(representationAttrExpr, "ValuesBoolean");
            var propertyValuesInt = Expression.Property(representationAttrExpr, "ValuesInteger");
            var propertyValuesDateTime = Expression.Property(representationAttrExpr, "ValuesDateTime");
            var attrInteger = Expression.Parameter(typeof(int), "prop");
            var attrDateTime = Expression.Parameter(typeof(DateTime), "prop");
            var attrString = Expression.Parameter(typeof(string), "prop");
            var attrBoolean = Expression.Parameter(typeof(bool), "prop");
            var countIntegerExpression = Expression.Call(typeof(Enumerable), "Count", new[] { typeof(int) }, propertyValuesInt);
            var countStringExpression = Expression.Call(typeof(Enumerable), "Count", new[] { typeof(string) }, propertyValuesString);
            var countDateTimeExpression = Expression.Call(typeof(Enumerable), "Count", new[] { typeof(DateTime) }, propertyValuesDateTime);
            var countBooleanExpression = Expression.Call(typeof(Enumerable), "Count", new[] { typeof(bool) }, propertyValuesBoolean);
            return Expression.Or(
                Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.INTEGER)), Expression.GreaterThan(countIntegerExpression, Expression.Constant(0))),
                Expression.Or(
                    Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.STRING)), Expression.GreaterThan(countStringExpression, Expression.Constant(0))),
                    Expression.Or(
                        Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.DATETIME)), Expression.GreaterThan(countDateTimeExpression, Expression.Constant(0))),
                        Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.BOOLEAN)), Expression.GreaterThan(countBooleanExpression, Expression.Constant(0)))
                    )
                )
            );
        }

        private static Expression BuildComparison(SCIMComparisonExpression comparisonExpression, Expression representationAttrExpr)
        {
            var propertySchemaAttribute = Expression.Property(representationAttrExpr, "SchemaAttribute");
            var propertySchemaType = Expression.Property(propertySchemaAttribute, "Type");
            var propertyValuesString = Expression.Property(representationAttrExpr, "ValuesString");
            var propertyValuesBoolean = Expression.Property(representationAttrExpr, "ValuesBoolean");
            var propertyValuesInt = Expression.Property(representationAttrExpr, "ValuesInteger");
            var propertyValuesDateTime = Expression.Property(representationAttrExpr, "ValuesDateTime");
            var attrInteger = Expression.Parameter(typeof(int), "prop");
            var attrDateTime = Expression.Parameter(typeof(DateTime), "prop");
            var attrString = Expression.Parameter(typeof(string), "prop");
            var attrBoolean = Expression.Parameter(typeof(bool), "prop");
            Expression anyIntegerLambda = Expression.Lambda<Func<int, bool>>(Expression.Constant(false), attrInteger),
                anyDateTimeLambda = Expression.Lambda<Func<DateTime, bool>>(Expression.Constant(false), attrDateTime),
                anyStringLambda = Expression.Lambda<Func<string, bool>>(Expression.Constant(false), attrString),
                anyBooleanLambda = Expression.Lambda<Func<bool, bool>>(Expression.Constant(false), attrBoolean);
            Expression equalValue = null;
            switch (comparisonExpression.ComparisonOperator)
            {
                case SCIMComparisonOperators.EQ:
                case SCIMComparisonOperators.GT:
                case SCIMComparisonOperators.GE:
                case SCIMComparisonOperators.LE:
                case SCIMComparisonOperators.LT:
                case SCIMComparisonOperators.SW:
                case SCIMComparisonOperators.EW:
                case SCIMComparisonOperators.CO:
                    switch (comparisonExpression.ComparisonOperator)
                    {
                        case SCIMComparisonOperators.GT:
                            anyIntegerLambda = Expression.Lambda<Func<int, bool>>(Expression.GreaterThan(attrInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrInteger);
                            anyDateTimeLambda = Expression.Lambda<Func<DateTime, bool>>(Expression.GreaterThan(attrDateTime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrDateTime);
                            break;
                        case SCIMComparisonOperators.GE:
                            anyIntegerLambda = Expression.Lambda<Func<int, bool>>(Expression.GreaterThanOrEqual(attrInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrInteger);
                            anyDateTimeLambda = Expression.Lambda<Func<DateTime, bool>>(Expression.GreaterThanOrEqual(attrDateTime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrDateTime);
                            break;
                        case SCIMComparisonOperators.LE:
                            anyIntegerLambda = Expression.Lambda<Func<int, bool>>(Expression.LessThanOrEqual(attrInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrInteger);
                            anyDateTimeLambda = Expression.Lambda<Func<DateTime, bool>>(Expression.LessThanOrEqual(attrDateTime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrDateTime);
                            break;
                        case SCIMComparisonOperators.LT:
                            anyIntegerLambda = Expression.Lambda<Func<int, bool>>(Expression.LessThan(attrInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrInteger);
                            anyDateTimeLambda = Expression.Lambda<Func<DateTime, bool>>(Expression.LessThan(attrDateTime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrDateTime);
                            break;
                        case SCIMComparisonOperators.EQ:
                            anyIntegerLambda = Expression.Lambda<Func<int, bool>>(Expression.Equal(attrInteger, Expression.Constant(ParseInt(comparisonExpression.Value))), attrInteger);
                            anyDateTimeLambda = Expression.Lambda<Func<DateTime, bool>>(Expression.Equal(attrDateTime, Expression.Constant(ParseDateTime(comparisonExpression.Value))), attrDateTime);
                            anyStringLambda = Expression.Lambda<Func<string, bool>>(Expression.Equal(attrString, Expression.Constant(comparisonExpression.Value)), attrString);
                            anyBooleanLambda = Expression.Lambda<Func<bool, bool>>(Expression.Equal(attrBoolean, Expression.Constant(ParseBoolean(comparisonExpression.Value))), attrBoolean);
                            break;
                        case SCIMComparisonOperators.SW:
                            var startWith = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                            anyStringLambda = Expression.Lambda<Func<string, bool>>(Expression.Call(attrString, startWith, Expression.Constant(comparisonExpression.Value)), attrString);
                            break;
                        case SCIMComparisonOperators.EW:
                            var endWith = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
                            anyStringLambda = Expression.Lambda<Func<string, bool>>(Expression.Call(attrString, endWith, Expression.Constant(comparisonExpression.Value)), attrString);
                            break;
                        case SCIMComparisonOperators.CO:
                            var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            anyStringLambda = Expression.Lambda<Func<string, bool>>(Expression.Call(attrString, contains, Expression.Constant(comparisonExpression.Value)), attrString);
                            break;
                    }

                    var allInteger = Expression.Call(typeof(Queryable).GetMethods().First(m2 => m2.Name == "All").MakeGenericMethod(typeof(int)), propertyValuesInt, anyIntegerLambda);
                    var allDateTime = Expression.Call(typeof(Queryable).GetMethods().First(m2 => m2.Name == "All").MakeGenericMethod(typeof(DateTime)), propertyValuesDateTime, anyDateTimeLambda);
                    var allString = Expression.Call(typeof(Queryable).GetMethods().First(m2 => m2.Name == "All").MakeGenericMethod(typeof(string)), propertyValuesString, anyStringLambda);
                    var allBoolean = Expression.Call(typeof(Queryable).GetMethods().First(m2 => m2.Name == "All").MakeGenericMethod(typeof(bool)), propertyValuesBoolean, anyBooleanLambda);
                    equalValue = Expression.Or(
                        Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.INTEGER)), allInteger),
                        Expression.Or(
                            Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.DATETIME)), allDateTime),
                            Expression.Or(
                                Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.STRING)), allString),
                                Expression.And(Expression.Equal(propertySchemaType, Expression.Constant(SCIMSchemaAttributeTypes.BOOLEAN)), allBoolean)
                            )
                        )
                    );
                    break;
            }

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
    }
}
