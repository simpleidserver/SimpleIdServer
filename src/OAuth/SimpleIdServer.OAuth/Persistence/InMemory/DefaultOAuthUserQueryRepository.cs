// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence.InMemory
{
    public class DefaultOAuthUserQueryRepository : IOAuthUserQueryRepository
    {
        public List<OAuthUser> _users;

        public DefaultOAuthUserQueryRepository(List<OAuthUser> users)
        {
            _users = users;
        }

        public Task<OAuthUser> FindOAuthUserByClaim(string claimType, string claimValue)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Claims.Any(c => c.Type == claimType && c.Value == claimValue)));
        }

        public Task<OAuthUser> FindOAuthUserByLogin(string login, CancellationToken token)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == login));
        }

        public Task<OAuthUser> FindOAuthUserByLoginAndCredential(string login, string credentialType, string credentialValue)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == login && u.Credentials.Any(c => c.CredentialType == credentialType && c.Value == credentialValue)));
        }
    }
}
