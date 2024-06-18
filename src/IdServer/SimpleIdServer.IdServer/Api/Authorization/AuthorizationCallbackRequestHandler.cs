// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Authorization;

public interface IAuthorizationCallbackRequestHandler
{
    Task Handle(HandlerContext context, CancellationToken cancellationToken);
}

public class AuthorizationCallbackRequestHandler : IAuthorizationCallbackRequestHandler
{
    private readonly IAuthorizationCallbackRequestValidator _validator;

    public AuthorizationCallbackRequestHandler(
        IAuthorizationCallbackRequestValidator validator)
    {
        _validator = validator;
    }

    public async Task Handle(HandlerContext context, CancellationToken cancellationToken)
    {
        // TODO : return the error to the redirect_uri.
        var record = await _validator.Validate(context, cancellationToken);
        // add the authorization code.
    }
}