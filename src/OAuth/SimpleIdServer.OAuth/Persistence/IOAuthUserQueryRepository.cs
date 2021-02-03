// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence
{
    public interface IOAuthUserQueryRepository
    {
        Task<OAuthUser> FindOAuthUserByLogin(string login, CancellationToken token);
        Task<OAuthUser> FindOAuthUserByLoginAndCredential(string login, string credentialType, string credentialValue);
        Task<OAuthUser> FindOAuthUserByClaim(string claimType, string claimValue);
    }
}