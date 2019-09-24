// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Domains.Users
{
    public class DefaultOAuthUserQueryRepository : IOAuthUserQueryRepository
    {
        public List<OAuthUser> _users;

        public DefaultOAuthUserQueryRepository(List<OAuthUser> users)
        {
            _users = users;
        }

        public Task<OAuthUser> FindOAuthUserByClaim(string claimKey, string claimValue)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Claims.Any(c => c.Key == claimKey && c.Value == claimValue)));
        }

        public Task<OAuthUser> FindOAuthUserByLogin(string login)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == login));
        }

        public Task<OAuthUser> FindOAuthUserByLoginAndCredential(string login, string credentialType, string credentialValue)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == login && u.Credentials.Any(c => c.CredentialType == credentialType && c.Value == credentialValue)));
        }
    }
}
