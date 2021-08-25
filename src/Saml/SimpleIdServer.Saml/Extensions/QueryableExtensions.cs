// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Persistence;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SimpleIdServer.Saml.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> InvokeOrderBy<T>(this IQueryable<T> source, string propertyName, SortOrders order)
        {
            var piParametr = Expression.Parameter(typeof(T), "r");
            var property = Expression.Property(piParametr, propertyName);
            var lambdaExpr = Expression.Lambda(property, piParametr);
            return (IQueryable<T>)Expression.Call(
                typeof(Queryable),
                order == SortOrders.ASC ? "OrderBy" : "OrderByDescending",
                new Type[] { typeof(T), property.Type },
                source.Expression,
                lambdaExpr)
                .Method.Invoke(null, new object[] { source, lambdaExpr });
        }
    }
}
