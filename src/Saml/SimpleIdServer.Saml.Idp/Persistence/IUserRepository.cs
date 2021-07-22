// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Common.Domains;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Saml.Idp.Persistence
{
    public interface IUserRepository
    {
        Task<User> FindOAuthUserByLogin(string login, CancellationToken cancellationToken);
        Task<User> Get(string id, CancellationToken cancellationToken);
        Task<bool> Update(User user, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
