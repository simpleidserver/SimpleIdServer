// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Api.Authorization;
using SimpleIdServer.IdServer.Api.Authorization.ResponseTypes;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Jwt;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.VerifiablePresentation.Apis.Authorization;

/*
public class WalletOAuthAuthorizationService : IWalletOAuthAuthorizationService
{
    private readonly IJwtBuilder _jwtBuilder;

    public WalletOAuthAuthorizationService(IJwtBuilder jwtBuilder)
    {
        _jwtBuilder = jwtBuilder;
    }

    public string Amr { get; } = Constants.AMR;

    public async Task<AuthorizationResponse> Handle(HandlerContext context, CancellationToken cancellationToken)
    {
        // https://hub.ebsi.eu/conformance/learn/verifiable-credential-issuance#id-token-request
        var targetUri = context.Request.RequestData.GetRedirectUriFromAuthorizationRequest();
        var scopes = context.Request.RequestData.GetScopesFromAuthorizationRequest();
        var nonce = context.Request.RequestData.GetNonceFromAuthorizationRequest();
        var state = context.Request.RequestData.GetStateFromAuthorizationRequest();
        var clientId = $"{context.GetIssuer()}/{IdServer.Config.DefaultEndpoints.Authorization}";
        var redirectUri = $"{context.GetIssuer()}/{Constants.Endpoints.VpAuthorizePost}";
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = context.GetIssuer(),
            Audience = context.Client.ClientId,
            Claims = new Dictionary<string, object>
            {
                { AuthorizationRequestParameters.ResponseType, IdTokenResponseTypeHandler.RESPONSE_TYPE },
                { AuthorizationRequestParameters.ResponseMode, "direct_post" },
                { AuthorizationRequestParameters.ClientId, clientId },
                { AuthorizationRequestParameters.RedirectUri, redirectUri },
                { AuthorizationRequestParameters.Scope, string.Join(" ", scopes) },
                { AuthorizationRequestParameters.Nonce, nonce }
            }
        };
        if (!string.IsNullOrWhiteSpace(state))
            descriptor.Claims.Add(AuthorizationRequestParameters.State, state);
        var jwt = await _jwtBuilder.BuildClientToken(context.Realm, context.Client, descriptor, context.Client.AuthorizationSignedResponseAlg ?? SecurityAlgorithms.RsaSha256, context.Client.AuthorizationEncryptedResponseAlg, context.Client.AuthorizationEncryptedResponseEnc, cancellationToken);
        var dic = new Dictionary<string, string>
        {
            { AuthorizationRequestParameters.ClientId, clientId },
            { AuthorizationRequestParameters.ResponseType, IdTokenResponseTypeHandler.RESPONSE_TYPE },
            { AuthorizationRequestParameters.ResponseMode, "direct_post" },
            { AuthorizationRequestParameters.Scope, string.Join(" ", scopes) },
            { AuthorizationRequestParameters.RedirectUri, redirectUri },
            { AuthorizationRequestParameters.Request, jwt }
        };
        return new RedirectURLAuthorizationResponse(targetUri, dic);
    }
}
*/