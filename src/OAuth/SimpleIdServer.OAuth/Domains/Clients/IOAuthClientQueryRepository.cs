// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Domains.Clients
{
    public interface IOAuthClientQueryRepository
    {
        Task<OAuthClient> FindOAuthClientById(string clientId);
        Task<IEnumerable<OAuthClient>> FindOAuthClientByIds(IEnumerable<string> clientIds);
    }
}