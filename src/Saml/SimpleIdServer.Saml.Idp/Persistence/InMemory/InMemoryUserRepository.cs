// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Persistence.InMemory
{
    public class InMemoryUserRepository : IUserRepository
    {
        private ICollection<User> _users;

        public InMemoryUserRepository(ICollection<User> users)
        {
            _users = users;
        }

        public Task<User> FindOAuthUserByLogin(string login, CancellationToken cancellationToken)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == login));
        }

        public Task<User> Get(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }

        public Task<bool> Update(User user, CancellationToken cancellationToken)
        {
            var record = _users.First(u => u.Id == user.Id);
            _users.Remove(record);
            _users.Add((User)record.Clone());
            return Task.FromResult(true);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
