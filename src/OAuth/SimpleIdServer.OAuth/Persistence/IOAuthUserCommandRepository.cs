// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Domains;

namespace SimpleIdServer.OAuth.Persistence.Users
{
    public interface IOAuthUserCommandRepository : ICommandRepository<OAuthUser>
    {
    }
}
