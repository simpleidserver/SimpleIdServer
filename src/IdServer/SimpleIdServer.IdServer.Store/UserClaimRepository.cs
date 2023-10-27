// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Store;

public interface IUserClaimRepository
{
    IQueryable<UserClaim> Query();
}

public class UserClaimRepository : IUserClaimRepository
{
    public IQueryable<UserClaim> Query()
    {
        throw new NotImplementedException();
    }
}
