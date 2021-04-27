// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence.InMemory
{
    public class DefaultOAuthUserCommandRepository : InMemoryCommandRepository<OAuthUser>, IOAuthUserCommandRepository
    {
        public DefaultOAuthUserCommandRepository(List<OAuthUser> users) : base(users)
        {
        }

        public Task RemoveAllConsents(string clientId, CancellationToken cancellationToken)
        {
            foreach(var user in LstData)
            {
                user.Consents = user.Consents.Where(c => c.ClientId != clientId).ToList();
            }

            return Task.CompletedTask;
        }
    }
}