// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Persistence
{
    public interface IOAuthUserRepository : ICommandRepository<OAuthUser>
    {
        Task<OAuthUser> FindOAuthUserByLogin(string login, CancellationToken cancellationToken);
        Task<OAuthUser> FindOAuthUserByLoginAndCredential(string login, string credentialType, string credentialValue, CancellationToken token);
        Task<OAuthUser> FindOAuthUserByClaim(string claimType, string claimValue, CancellationToken cancellationToken);
        Task<OAuthUser> FindOAuthUserByExternalAuthProvider(string scheme, string subject, CancellationToken cancellationToken);
    }
}
