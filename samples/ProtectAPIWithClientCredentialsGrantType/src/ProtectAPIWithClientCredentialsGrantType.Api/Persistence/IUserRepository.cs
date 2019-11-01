// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using ProtectAPIWithClientCredentialsGrantType.Api.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProtectAPIWithClientCredentialsGrantType.Api.Persistence
{
    public interface IUserRepository
    {
        void AddUser(User user);
        Task<IEnumerable<User>> FindAllUsers();
        Task<User> FindUserByIdentifier(string id);
        void DeleteUser(User user);
        Task<int> SaveChanges();
    }
}
