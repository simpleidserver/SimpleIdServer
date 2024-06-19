// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Jwt;
using SimpleIdServer.DPoP;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Resources;
using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Authorization;

public interface IAuthorizationCallbackRequestValidator
{
    Task<JsonObject> Validate(HandlerContext context, CancellationToken cancellationToken);
}

public class AuthorizationCallbackRequestValidator : IAuthorizationCallbackRequestValidator
{
    private readonly IGrantedTokenHelper _grantedTokenHelper;
    private readonly IDidFactoryResolver _resolver;

    public AuthorizationCallbackRequestValidator(
        IGrantedTokenHelper grantedTokenHelper,
        IDidFactoryResolver resolver)
    {
        _grantedTokenHelper = grantedTokenHelper;
        _resolver = resolver;
    }

    public async Task<JsonObject> Validate(HandlerContext context, CancellationToken cancellationToken)
    {
        var idToken = context.Request.RequestData.GetIdTokenFromAuthorizationRequestCallback();
        if (idToken == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(Global.MissingParameter, AuthorizationResponseParameters.IdToken));
        var handler = new JsonWebTokenHandler();
        if (!handler.CanReadToken(idToken)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.InvalidJwt);

        var jsonWebToken = handler.ReadJsonWebToken(idToken);
        var nonce = jsonWebToken.Nonce();
        if (string.IsNullOrWhiteSpace(nonce)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.IdTokenNonceMissing);
        var authorizationRequestCallback = await _grantedTokenHelper.GetAuthorizationRequestCallback(nonce, cancellationToken);
        if (authorizationRequestCallback == null) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.InvalidNonce);

        bool isValid = false;
        TokenValidationResult validationResult;
        try
        {
            var didHandler = DidJsonWebTokenHandler.New();
            isValid = await didHandler.CheckJwt(idToken, _resolver, cancellationToken);
        }
        catch(InvalidOperationException ex)
        {
            throw new OAuthException(ErrorCodes.INVALID_REQUEST, ex.Message);
        }

        if (!isValid)
            throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.BadIdToken);

        return authorizationRequestCallback;
    }
}