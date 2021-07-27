// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.Domains;
using SimpleIdServer.OAuth.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.EF.Repositories
{
    public class OAuthUserRepository : IOAuthUserRepository
    {
        private readonly OpenIdDBContext _dbContext;

        public OAuthUserRepository(OpenIdDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<OAuthUser> FindOAuthUserByClaim(string claimType, string claimValue, CancellationToken cancellationToken)
        {
            return GetUsers().FirstOrDefaultAsync(u => u.OAuthUserClaims.Any(c => c.Name == claimType && c.Value == claimValue), cancellationToken);
        }

        public Task<OAuthUser> FindOAuthUserByLogin(string login, CancellationToken token)
        {
            return GetUsers().FirstOrDefaultAsync(u => u.Id == login, token);
        }

        public Task<OAuthUser> FindOAuthUserByLoginAndCredential(string login, string credentialType, string credentialValue, CancellationToken cancellationToken)
        {
            return GetUsers().Include(u => u.Credentials).FirstOrDefaultAsync(u => u.Id == login && u.Credentials.Any(c => c.CredentialType == credentialType && c.Value == credentialValue), cancellationToken);
        }

        public Task<OAuthUser> FindOAuthUserByExternalAuthProvider(string scheme, string subject, CancellationToken cancellationToken)
        {
            return GetUsers().FirstOrDefaultAsync(u => u.ExternalAuthProviders.Any(e => e.Scheme == scheme && e.Subject == subject));
        }

        private IQueryable<OAuthUser> GetUsers()
        {
            return _dbContext.Users
                .Include(u => u.Consents).ThenInclude(c => c.Scopes).ThenInclude(c => c.Claims)
                .Include(u => u.Sessions)
                .Include(u => u.OAuthUserClaims)
                .Include(u => u.Credentials);
        }

        public Task<bool> Add(OAuthUser data, CancellationToken token)
        {
            _dbContext.Users.Add(data);
            return Task.FromResult(true);
        }

        public Task<bool> Delete(OAuthUser data, CancellationToken token)
        {
            _dbContext.Users.Remove(data);
            return Task.FromResult(true);
        }

        public Task<bool> Update(OAuthUser data, CancellationToken token)
        {
            _dbContext.Users.Update(data);
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken token)
        {
            return _dbContext.SaveChangesAsync(token);
        }

        public void Dispose()
        {
        }
    }
}
