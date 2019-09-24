// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Domains.Users
{
    public class DefaultOAuthUserCommandRepository : InMemoryCommandRepository<OAuthUser>, IOAuthUserCommandRepository
    {
        public DefaultOAuthUserCommandRepository(List<OAuthUser> users) : base(users)
        {
        }
    }
}