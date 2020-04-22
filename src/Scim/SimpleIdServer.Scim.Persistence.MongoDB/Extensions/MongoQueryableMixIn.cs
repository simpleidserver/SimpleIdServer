// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Extensions
{
    public static class MongoQueryableMixIn
    {
        public static Task<List<T>> ToMongoListAsync<T>(this IQueryable<T> mongoQueryOnly)
        {
            return ((IMongoQueryable<T>)mongoQueryOnly).ToListAsync();
        }

        public static Task<T> ToMongoFirstAsync<T>(this IQueryable<T> mongoQueryOnly)
        {
            return ((IMongoQueryable<T>)mongoQueryOnly).FirstOrDefaultAsync();
        }
    }
}
