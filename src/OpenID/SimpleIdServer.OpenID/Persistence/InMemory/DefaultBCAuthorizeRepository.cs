// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.Domains;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence.InMemory
{
    public class DefaultBCAuthorizeRepository : IBCAuthorizeRepository
    {
        private readonly ConcurrentBag<BCAuthorize> _bcAuthorizeLst;

        public DefaultBCAuthorizeRepository(ConcurrentBag<BCAuthorize> bcAuthorizeLst)
        {
            _bcAuthorizeLst = bcAuthorizeLst;
        }

        public Task<IEnumerable<BCAuthorize>> GetConfirmedAuthorizeRequest(CancellationToken cancellationToken)
        {
            var currentDateTime = DateTime.UtcNow;
            return Task.FromResult(_bcAuthorizeLst.Where(bc => bc.Status == BCAuthorizeStatus.Confirmed && bc.ExpirationDateTime > currentDateTime));
        }

        public Task<BCAuthorize> Get(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_bcAuthorizeLst.FirstOrDefault(a => a.Id == id));
        }

        public Task Update(BCAuthorize bcAuthorize, CancellationToken cancellationToken)
        {
            var bcAuth = _bcAuthorizeLst.First(_ => _.Id == bcAuthorize.Id);
            _bcAuthorizeLst.Remove(bcAuth);
            _bcAuthorizeLst.Add(bcAuthorize);
            return Task.CompletedTask;
        }

        public Task Add(BCAuthorize bcAuthorize, CancellationToken cancellationToken)
        {
            _bcAuthorizeLst.Add((BCAuthorize)bcAuthorize.Clone());
            return Task.CompletedTask;
        }

        public Task SaveChanges(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
