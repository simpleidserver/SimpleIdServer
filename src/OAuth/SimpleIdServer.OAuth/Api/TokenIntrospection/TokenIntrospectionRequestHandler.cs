// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Helpers;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.TokenIntrospection
{
    public interface ITokenIntrospectionRequestHandler
    {
        Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken);
    }

    public class TokenIntrospectionRequestHandler : ITokenIntrospectionRequestHandler
    {
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;
        private readonly IGrantedTokenHelper _grantedTokenHelper;

        public TokenIntrospectionRequestHandler(IClientAuthenticationHelper clientAuthenticationHelper, IGrantedTokenHelper grantedTokenHelper)
        {
            _clientAuthenticationHelper = clientAuthenticationHelper;
            _grantedTokenHelper = grantedTokenHelper;
        }

        public async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            var token = context.Request.RequestData.GetToken();
            if(string.IsNullOrWhiteSpace(token)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, IntrospectionRequestParameters.Token));
            var client = await _clientAuthenticationHelper.AuthenticateClient(context.Request.HttpHeader, context.Request.RequestData, context.Request.Certificate, context.Request.IssuerName, cancellationToken);
            var accessToken = await _grantedTokenHelper.GetAccessToken(token, cancellationToken);
            var accessTokenClientId = string.Empty;
            if (accessToken.TryGetClaim(OpenIdConnectParameterNames.ClientId, out Claim value)) accessTokenClientId = value.Value;
            if(accessToken == null || accessToken.ValidTo <= DateTime.UtcNow || accessTokenClientId != client.ClientId)
            {
                var obj = new JsonObject
                {
                    [IntrospectionResponseParameters.Active] = false
                };
                return new OkObjectResult(obj);
            }

            var result = new JsonObject
            {
                [IntrospectionResponseParameters.Active] = true
            };
            foreach (var cl in accessToken.Claims.GroupBy(c => c.Type))
                result.Add(cl.Key, cl.Count() == 1 ? cl.First().Value : JsonSerializer.Serialize(cl.Select(c => c.Value)));
            return new OkObjectResult(result);
        }
    }
}
