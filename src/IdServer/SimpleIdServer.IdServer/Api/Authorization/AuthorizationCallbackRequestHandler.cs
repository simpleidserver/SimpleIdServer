// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Authorization;

public interface IAuthorizationCallbackRequestHandler
{
    Task<RedirectURLAuthorizationResponse> Handle(HandlerContext context, CancellationToken cancellationToken);
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

    public async Task<RedirectURLAuthorizationResponse> Handle(HandlerContext context, CancellationToken cancellationToken)
    {
        var record = await _validator.Validate(context, cancellationToken);
        var responseTypes = record.GetResponseTypesFromAuthorizationRequest();
        var authorizationDetails = record.GetAuthorizationDetailsFromAuthorizationRequest();
        var scopes = record.GetScopesFromAuthorizationRequest();
        var redirectUri = record.GetRedirectUriFromAuthorizationRequest();
        var state = record.GetStateFromAuthorizationRequest();
        var filteredResponseTypeHandlers = _responseTypes.Where(r => responseTypes.Contains(r.ResponseType));
        context.Request.SetRequestData(record);
        context.SetClient(new Domains.Client
        {
            IsPublic = true
        });
        foreach(var responseTypeHandler in filteredResponseTypeHandlers)
            await responseTypeHandler.Enrich(new EnrichParameter { AuthorizationDetails = authorizationDetails, Scopes = scopes }, context, cancellationToken);

        if (!string.IsNullOrWhiteSpace(state))
            context.Response.Add(AuthorizationRequestParameters.State, state);

        return new RedirectURLAuthorizationResponse(redirectUri, context.Response.Parameters);
    }
}