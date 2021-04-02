// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.Token.Handlers
{
    public interface ICIBAGrantTypeValidator
    {
        Task<Domains.BCAuthorize> Validate(HandlerContext context, CancellationToken cancellationToken);
    }
}
