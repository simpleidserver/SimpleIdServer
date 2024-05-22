// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store.SqlSugar.Models;
using SimpleIdServer.IdServer.Stores;

namespace SimpleIdServer.IdServer.Store.SqlSugar
{
    public class GotiySessionStore : IGotiySessionStore
    {
        private readonly DbContext _dbContext;

        public GotiySessionStore(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(GotifySession session)
        {
            _dbContext.Client.Insertable(Transform(session)).ExecuteCommand();
        }

        public async Task<GotifySession> GetByClientToken(string clientToken, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Client.Queryable<SugarGotifySession>()
                .FirstAsync(c => c.ClientToken == clientToken, cancellationToken);
            return result?.ToDomain();
        }

        private static SugarGotifySession Transform(GotifySession session)
        {
            return new SugarGotifySession
            {
                ApplicationToken = session.ApplicationToken,
                ClientToken = session.ClientToken
            };
        }
    }
}
