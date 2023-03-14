// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Helpers;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.TokenIntrospection
{
    public interface ITokenIntrospectionRequestHandler
    {
        Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken);
    }

    public class TokenIntrospectionRequestHandler : ITokenIntrospectionRequestHandler
    {
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;
        private readonly IGrantedTokenHelper _grantedTokenHelper;
        private readonly IBusControl _busControl;

        public TokenIntrospectionRequestHandler(IClientAuthenticationHelper clientAuthenticationHelper, IGrantedTokenHelper grantedTokenHelper, IBusControl busControl)
        {
            _clientAuthenticationHelper = clientAuthenticationHelper;
            _grantedTokenHelper = grantedTokenHelper;
            _busControl = busControl;
        }

        public async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            string token = null, clientId = null;
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Introspect Token"))
            {
                try
                {
                    activity?.SetTag("realm", context.Realm);
                    token = context.Request.RequestData.GetToken();
                    if (string.IsNullOrWhiteSpace(token))
                        throw new OAuthException(ErrorCodes.INVALID_REQUEST, string.Format(ErrorMessages.MISSING_PARAMETER, IntrospectionRequestParameters.Token));

                    activity?.SetTag("token", token);
                    var client = await _clientAuthenticationHelper.AuthenticateClient(context.Realm, context.Request.HttpHeader, context.Request.RequestData, context.Request.Certificate, context.GetIssuer(), cancellationToken);
                    activity?.SetTag("client_id", client.ClientId);
                    clientId = client.ClientId;
                    var accessToken = await _grantedTokenHelper.GetAccessToken(token, cancellationToken);
                    var accessTokenClientId = string.Empty;
                    if (accessToken != null && accessToken.TryGetClaim(OpenIdConnectParameterNames.ClientId, out Claim value)) accessTokenClientId = value.Value;
                    if (accessToken == null || accessToken.ValidTo <= DateTime.UtcNow || accessTokenClientId != client.ClientId)
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
                    await _busControl.Publish(new TokenIntrospectionSuccessEvent
                    {
                        ClientId = client.ClientId,
                        Realm = context.Realm,
                        Token = token
                    });
                    activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Ok, "Token is introspected");
                    return new OkObjectResult(result);
                }
                catch (OAuthUnauthorizedException ex)
                {
                    await _busControl.Publish(new TokenIntrospectionFailureEvent
                    {
                        ClientId = clientId,
                        Realm = context.Realm,
                        ErrorMessage = ex.Message,
                        Token = token
                    });
                    activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                    throw ex;
                }
                catch (OAuthException ex)
                {
                    await _busControl.Publish(new TokenIntrospectionFailureEvent
                    {
                        ClientId = clientId,
                        Realm = context.Realm,
                        ErrorMessage = ex.Message,
                        Token = token
                    });
                    activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, ex.Message);
                    throw ex;
                }
            }
        }
    }
}
