// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Authorization;

public interface IAuthorizationCallbackRequestHandler
{
    Task<AuthorizationResponse> Handle(HandlerContext context, CancellationToken cancellationToken);
}

public class AuthorizationCallbackRequestHandler : IAuthorizationCallbackRequestHandler
{
    private readonly IAuthorizationCallbackRequestValidator _validator;
    private readonly IEnumerable<IResponseTypeHandler> _responseTypes;

    public AuthorizationCallbackRequestHandler(
        IAuthorizationCallbackRequestValidator validator,
        IEnumerable<IResponseTypeHandler> responseTypes)
    {
        _validator = validator;
        _responseTypes = responseTypes;
    }

    public async Task<AuthorizationResponse> Handle(HandlerContext context, CancellationToken cancellationToken)
    {
        // TODO : return the error to the redirect_uri.
        var record = await _validator.Validate(context, cancellationToken);
        var filteredResponseTypeHandlers = _responseTypes.Where(r => record.ResponseTypes.Contains(r.ResponseType));
        foreach(var responseTypeHandler in filteredResponseTypeHandlers)
            await responseTypeHandler.Enrich(new EnrichParameter { AuthorizationDetails = record.AuthorizationDetails, Scopes = record.Scopes }, context, cancellationToken);

        return new RedirectURLAuthorizationResponse(record.RedirectUri, context.Response.Parameters);
    }
}