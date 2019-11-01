// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using ProtectAPIWithClientCredentialsGrantType.Api.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProtectAPIWithClientCredentialsGrantType.Api.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public UserRepository()
        {
            _users = new List<User>();
        }

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public Task<User> FindUserByIdentifier(string id)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }

        public Task<IEnumerable<User>> FindAllUsers()
        {
            return Task.FromResult((IEnumerable<User>)_users);
        }

        public void DeleteUser(User user)
        {
            var record = _users.First(u => u.Id == user.Id);
            _users.Remove(record);
        }

        public Task<int> SaveChanges()
        {
            return Task.FromResult(1);
        }
    }
}
