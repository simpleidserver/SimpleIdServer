// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Threading.Tasks;
using System.Threading;

namespace SimpleIdServer.IdServer.Api.Authorization;

public interface IWalletOAuthAuthorizationService
{
    string Amr { get; }
    Task<AuthorizationResponse> Handle(HandlerContext context, CancellationToken cancellationToken);
}