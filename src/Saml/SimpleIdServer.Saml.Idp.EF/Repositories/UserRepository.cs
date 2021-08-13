// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Common.Domains;
using SimpleIdServer.Saml.Idp.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.EF.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SamlIdpDBContext _dbContext;

        public UserRepository(SamlIdpDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<User> FindOAuthUserByLogin(string login, CancellationToken cancellationToken)
        {
            return GetUsers().FirstOrDefaultAsync(u => u.Id == login, cancellationToken);
        }

        public Task<User> Get(string id, CancellationToken cancellationToken)
        {
            return GetUsers().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public Task<bool> Update(User user, CancellationToken cancellationToken)
        {
            _dbContext.Users.Update(user);
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        private IQueryable<User> GetUsers()
        {
            return _dbContext.Users
                .Include(u => u.Sessions)
                .Include(u => u.OAuthUserClaims)
                .Include(u => u.Credentials)
                .Include(u => u.ExternalAuthProviders);
        }
    }
}
