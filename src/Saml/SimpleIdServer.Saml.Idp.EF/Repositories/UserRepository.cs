// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.Domains;
using SimpleIdServer.Saml.Idp.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.EF.Repositories
{
    public class UserRepository : IUserRepository
    {
        public UserRepository()
        {

        }

        public Task<User> FindOAuthUserByLogin(string login, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> Get(string id, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Update(User user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
