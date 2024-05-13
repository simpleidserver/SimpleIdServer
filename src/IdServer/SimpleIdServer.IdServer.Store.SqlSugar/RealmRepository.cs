// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store.SqlSugar
{
    public class RealmRepository
    {


        public RealmRepository()
        {
            
        }

        public IQueryable<Realm> Query() => _dbContext.Realms;
    }
}
