// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace SimpleIdServer.OpenBankingApi.Infrastructure.Authorizations
{
    public class MtlsAccessTokenRequirement : IAuthorizationRequirement
    {
        public MtlsAccessTokenRequirement(IEnumerable<string> scopes)
        {
            Scopes = scopes;
        }

        public IEnumerable<string> Scopes { get; set; }
    }
}
