// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Authorization.ResponseTypes
{
    public interface IResponseTypeHandler
    {
        string GrantType { get; }
        string ResponseType { get; }
        int Order { get; }
        Task Enrich(IEnumerable<string> scopes, IEnumerable<string> audiences, IEnumerable<AuthorizationRequestClaimParameter> claims, HandlerContext context, CancellationToken cancellationToken);
    }
}