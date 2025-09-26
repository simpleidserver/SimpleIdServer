using MongoDB.Driver.Linq;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Parser;
using SimpleIdServer.Scim.Parser.Expressions;
using SimpleIdServer.Scim.Persistence.MongoDB.Extensions;
using SimpleIdServer.Scim.Persistence.MongoDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Tests
{
    public class MongoDbOptimizedExpressionTests
    {
        [Fact]
        public void MongoDbOptimizedExpressionExtensions_CreateOptimizedCaseInsensitiveEqual_ShouldHandleNormalString()
        {
            // Arrange
            var propertyExpression = Expression.Property(Expression.Parameter(typeof(TestClass), "x"), nameof(TestClass.Value));
            var testValue = "TestValue";
            
            // Act
            var result = MongoDbOptimizedExpressionExtensions.CreateOptimizedCaseInsensitiveEqual(propertyExpression, testValue);
            
            // Assert
            Assert.NotNull(result);
            Assert.True(result.Type == typeof(bool));
        }

        [Fact]
        public void MongoDbOptimizedExpressionExtensions_CreateOptimizedCaseInsensitiveEqual_ShouldHandleNullValue()
        {
            // Arrange
            var propertyExpression = Expression.Property(Expression.Parameter(typeof(TestClass), "x"), nameof(TestClass.Value));
            
            // Act
            var result = MongoDbOptimizedExpressionExtensions.CreateOptimizedCaseInsensitiveEqual(propertyExpression, null);
            
            // Assert
            Assert.NotNull(result);
            Assert.True(result.Type == typeof(bool));
        }

        [Fact]
        public void MongoDbOptimizedExpressionExtensions_CreateOptimizedCaseInsensitiveStartsWith_ShouldWork()
        {
            // Arrange
            var propertyExpression = Expression.Property(Expression.Parameter(typeof(TestClass), "x"), nameof(TestClass.Value));
            var testValue = "Test";
            
            // Act
            var result = MongoDbOptimizedExpressionExtensions.CreateOptimizedCaseInsensitiveStartsWith(propertyExpression, testValue);
            
            // Assert
            Assert.NotNull(result);
            Assert.True(result.Type == typeof(bool));
        }

        [Fact]
        public void MongoDbOptimizedExpressionExtensions_CreateOptimizedCaseInsensitiveContains_ShouldWork()
        {
            // Arrange
            var propertyExpression = Expression.Property(Expression.Parameter(typeof(TestClass), "x"), nameof(TestClass.Value));
            var testValue = "est";
            
            // Act
            var result = MongoDbOptimizedExpressionExtensions.CreateOptimizedCaseInsensitiveContains(propertyExpression, testValue);
            
            // Assert
            Assert.NotNull(result);
            Assert.True(result.Type == typeof(bool));
        }

        [Fact]
        public void MongoDbOptimizedExpressionExtensions_CreateOptimizedCaseInsensitiveEndsWith_ShouldWork()
        {
            // Arrange
            var propertyExpression = Expression.Property(Expression.Parameter(typeof(TestClass), "x"), nameof(TestClass.Value));
            var testValue = "Value";
            
            // Act
            var result = MongoDbOptimizedExpressionExtensions.CreateOptimizedCaseInsensitiveEndsWith(propertyExpression, testValue);
            
            // Assert
            Assert.NotNull(result);
            Assert.True(result.Type == typeof(bool));
        }

        private class TestClass
        {
            public string Value { get; set; }
        }
    }
}