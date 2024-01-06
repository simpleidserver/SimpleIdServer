// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Authorization.Validators;
using SimpleIdServer.IdServer.Authenticate;
using SimpleIdServer.IdServer.Authenticate.Handlers;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.PushedAuthorization;

[AllowAnonymous]
public class PushedAuthorizationController : Controller
{
    private readonly IAuthorizationRequestValidator _validator;
    private readonly IAuthenticateClient _authenticateClient;
    private readonly IBusControl _busControl;
    private readonly IDistributedCache _distributedCache;
    private readonly IEnumerable<IOAuthClientAuthenticationHandler> _authenticationHandlers;
    private readonly IdServerHostOptions _options;

    public PushedAuthorizationController(
        IAuthorizationRequestValidator validator, 
        IAuthenticateClient authenticateClient, 
        IBusControl busControl, 
        IDistributedCache distributedCache, 
        IEnumerable<IOAuthClientAuthenticationHandler> authenticationHandlers,
        IOptions<IdServerHostOptions> options)
    {
        _validator = validator;
        _authenticateClient = authenticateClient;
        _busControl = busControl;
        _distributedCache = distributedCache;
        _authenticationHandlers = authenticationHandlers;
        _options = options.Value;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromRoute] string prefix, CancellationToken token)
    {
        using (var activity = Tracing.IdServerActivitySource.StartActivity("Start Pushed Authorization Request"))
        {
            var jObjBody = Request.Form.ToJsonObject();
            prefix = prefix ?? Constants.DefaultRealm;
            var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), null, jObjBody, null, Request.Cookies), prefix, _options, new HandlerContextResponse(Response.Cookies));
            activity?.SetTag("realm", context.Realm);
            try
            {
                Validate(context);
                string clientId;
                var authenticateInstruction = new AuthenticateInstruction
                {
                    ClientAssertion = jObjBody.GetClientAssertion(),
                    ClientAssertionType = jObjBody.GetClientAssertionType(),
                    ClientIdFromHttpRequestBody = jObjBody.GetClientId()
                };
                if (!_authenticateClient.TryGetClientId(authenticateInstruction, out clientId)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.MISSING_CLIENT_ID);
                var validationResult = await _validator.ValidateAuthorizationRequest(context, clientId, token);
                if(!string.IsNullOrWhiteSpace(authenticateInstruction.ClientAssertion))
                {
                    var authHandler = _authenticationHandlers.Single(a => a.AuthMethod == OAuthClientPrivateKeyJwtAuthenticationHandler.AUTH_METHOD);
                    if (!(await authHandler.Handle(authenticateInstruction, context.Client, context.GetIssuer(), token)))
                        throw new OAuthException(ErrorCodes.INVALID_CLIENT, ErrorMessages.BAD_CLIENT_CREDENTIAL);
                }

                activity?.SetStatus(ActivityStatusCode.Ok, "Pushed Authorization Request is granted");
                var pushedAuthorizationRequestId = $"{Constants.ParFormatKey}:{Guid.NewGuid()}";
                await _distributedCache.SetAsync(pushedAuthorizationRequestId, Encoding.UTF8.GetBytes(context.Request.RequestData.ToJsonString()), new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(_options.PARExpirationTimeInSeconds)
                }, token);
                await _busControl.Publish(new PushedAuthorizationRequestSuccessEvent
                {
                    ClientId = context.Client?.ClientId,
                    Realm = context.Realm,
                    RequestJSON = jObjBody.ToString()
                });
                var jObj = new JsonObject
                {
                    { AuthorizationRequestParameters.RequestUri, pushedAuthorizationRequestId },
                    { AuthorizationResponseParameters.ExpiresIn, _options.PARExpirationTimeInSeconds }
                };
                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = jObj.ToJsonString()
                };
            }
            catch(OAuthException ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                await _busControl.Publish(new PushedAuthorizationRequestFailureEvent
                {
                    ClientId = context.Client?.ClientId,
                    Realm = context.Realm,
                    RequestJSON = jObjBody.ToString(),
                    ErrorMessage = ex.Message
                });
                return BuildErrorResponse(ex);
            }
        }

        void Validate(HandlerContext context)
        {
            var requestUri = context.Request.RequestData.GetRequestUriFromAuthorizationRequest();
            if (!string.IsNullOrWhiteSpace(requestUri))
                throw new OAuthException(ErrorCodes.INVALID_REQUEST, ErrorMessages.UNEXPECTED_REQUEST_URI_PARAMETER);
        }
    }

    private IActionResult BuildErrorResponse(OAuthException ex)
    {
        var jObj = new JsonObject
        {
            [ErrorResponseParameters.Error] = ex.Code,
            [ErrorResponseParameters.ErrorDescription] = ex.Message
        };
        return new BadRequestObjectResult(jObj);
    }
}
