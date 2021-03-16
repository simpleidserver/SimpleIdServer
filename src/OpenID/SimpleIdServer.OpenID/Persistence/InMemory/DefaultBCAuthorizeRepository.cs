// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Persistence.InMemory
{
    public class DefaultBCAuthorizeRepository : IBCAuthorizeRepository
    {
        private readonly List<BCAuthorize> _bcAuthorizeLst;

        public DefaultBCAuthorizeRepository()
        {
            _bcAuthorizeLst = new List<BCAuthorize>();
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
