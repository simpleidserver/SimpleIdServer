// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MongoDB.Driver;
using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Extensions
{
    /// <summary>
    /// MongoDB-specific optimized expression extensions that avoid $expr usage for better index performance
    /// </summary>
    public static class MongoDbOptimizedExpressionExtensions
    {
        /// <summary>
        /// Creates an optimized case-insensitive equality expression for MongoDB using regex instead of $expr
        /// This allows MongoDB to use indexes effectively
        /// </summary>
        public static Expression CreateOptimizedCaseInsensitiveEqual(Expression propertyExpression, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                // For null/empty values, use direct comparison
                return Expression.Equal(propertyExpression, Expression.Constant(null, typeof(string)));
            }

            // Escape regex special characters in the value
            var escapedValue = Regex.Escape(value);
            
            // Create case-insensitive regex pattern
            var regexPattern = $"^{escapedValue}$";
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            
            // Use MongoDB's regex matching which can use indexes when anchored
            var regexConstant = Expression.Constant(regex, typeof(Regex));
            var isMatchMethod = typeof(Regex).GetMethod("IsMatch", new[] { typeof(string) });
            
            // Handle null values by using null-conditional operator pattern
            var nullCheck = Expression.Equal(propertyExpression, Expression.Constant(null, typeof(string)));
            var regexMatch = Expression.Call(regexConstant, isMatchMethod, propertyExpression);
            
            // If property is null and value is empty, return true; otherwise use regex match
            if (string.IsNullOrEmpty(value))
            {
                return nullCheck;
            }
            
            // For non-null values, use: property != null && Regex.IsMatch(property, pattern)
            var notNullCheck = Expression.NotEqual(propertyExpression, Expression.Constant(null, typeof(string)));
            return Expression.AndAlso(notNullCheck, regexMatch);
        }

        /// <summary>
        /// Creates an optimized starts-with expression for MongoDB that can use indexes
        /// </summary>
        public static Expression CreateOptimizedCaseInsensitiveStartsWith(Expression propertyExpression, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                // For empty value, everything starts with empty string
                return Expression.Constant(true);
            }

            // Escape regex special characters in the value
            var escapedValue = Regex.Escape(value);
            
            // Create case-insensitive regex pattern for starts with
            var regexPattern = $"^{escapedValue}";
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            
            var regexConstant = Expression.Constant(regex, typeof(Regex));
            var isMatchMethod = typeof(Regex).GetMethod("IsMatch", new[] { typeof(string) });
            
            // Handle null values
            var notNullCheck = Expression.NotEqual(propertyExpression, Expression.Constant(null, typeof(string)));
            var regexMatch = Expression.Call(regexConstant, isMatchMethod, propertyExpression);
            
            return Expression.AndAlso(notNullCheck, regexMatch);
        }

        /// <summary>
        /// Creates an optimized contains expression for MongoDB
        /// </summary>
        public static Expression CreateOptimizedCaseInsensitiveContains(Expression propertyExpression, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                // For empty value, everything contains empty string
                return Expression.Constant(true);
            }

            // Escape regex special characters in the value
            var escapedValue = Regex.Escape(value);
            
            // Create case-insensitive regex pattern for contains
            var regexPattern = escapedValue;
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            
            var regexConstant = Expression.Constant(regex, typeof(Regex));
            var isMatchMethod = typeof(Regex).GetMethod("IsMatch", new[] { typeof(string) });
            
            // Handle null values
            var notNullCheck = Expression.NotEqual(propertyExpression, Expression.Constant(null, typeof(string)));
            var regexMatch = Expression.Call(regexConstant, isMatchMethod, propertyExpression);
            
            return Expression.AndAlso(notNullCheck, regexMatch);
        }

        /// <summary>
        /// Creates an optimized ends-with expression for MongoDB
        /// </summary>
        public static Expression CreateOptimizedCaseInsensitiveEndsWith(Expression propertyExpression, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                // For empty value, everything ends with empty string
                return Expression.Constant(true);
            }

            // Escape regex special characters in the value
            var escapedValue = Regex.Escape(value);
            
            // Create case-insensitive regex pattern for ends with
            var regexPattern = $"{escapedValue}$";
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
            
            var regexConstant = Expression.Constant(regex, typeof(Regex));
            var isMatchMethod = typeof(Regex).GetMethod("IsMatch", new[] { typeof(string) });
            
            // Handle null values
            var notNullCheck = Expression.NotEqual(propertyExpression, Expression.Constant(null, typeof(string)));
            var regexMatch = Expression.Call(regexConstant, isMatchMethod, propertyExpression);
            
            return Expression.AndAlso(notNullCheck, regexMatch);
        }
    }
}