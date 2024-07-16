// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
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
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Resources;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.PushedAuthorization
{
    public class PushedAuthorizationController : Controller
    {
        private readonly IAuthorizationRequestValidator _validator;
        private readonly IAuthenticateClient _authenticateClient;
        private readonly IBusControl _busControl;
        private readonly IDistributedCache _distributedCache;
        private readonly IEnumerable<IOAuthClientAuthenticationHandler> _authenticationHandlers;
        private readonly IClientHelper _clientHelper;
        private readonly ITransactionBuilder _transactionBuilder;
        private readonly IdServerHostOptions _options;

        public PushedAuthorizationController(
            IAuthorizationRequestValidator validator, 
            IAuthenticateClient authenticateClient, 
            IBusControl busControl, 
            IDistributedCache distributedCache, 
            IEnumerable<IOAuthClientAuthenticationHandler> authenticationHandlers,
            IClientHelper clientHelper,
            ITransactionBuilder transactionBuilder,
            IOptions<IdServerHostOptions> options)
        {
            _validator = validator;
            _authenticateClient = authenticateClient;
            _busControl = busControl;
            _distributedCache = distributedCache;
            _authenticationHandlers = authenticationHandlers;
            _clientHelper = clientHelper;
            _transactionBuilder = transactionBuilder;
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
                    using (var transaction = _transactionBuilder.Build())
                    {
                        Validate(context);
                        string clientId;
                        var authenticateInstruction = new AuthenticateInstruction
                        {
                            ClientAssertion = jObjBody.GetClientAssertion(),
                            ClientAssertionType = jObjBody.GetClientAssertionType(),
                            ClientIdFromHttpRequestBody = jObjBody.GetClientId()
                        };
                        if (!_authenticateClient.TryGetClientId(authenticateInstruction, out clientId)) throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.MissingClientId);
                        var client = await _clientHelper.ResolveClient(context.Realm, clientId, token);
                        if (client != null)
                            context.SetClient(client);
                        var validationResult = await _validator.ValidateStandardAuthorizationRequest(context, clientId, token);
                        if (!string.IsNullOrWhiteSpace(authenticateInstruction.ClientAssertion))
                        {
                            var authHandler = _authenticationHandlers.Single(a => a.AuthMethod == OAuthClientPrivateKeyJwtAuthenticationHandler.AUTH_METHOD);
                            if (!(await authHandler.Handle(authenticateInstruction, context.Client, context.GetIssuer(), token)))
                                throw new OAuthException(ErrorCodes.INVALID_CLIENT, Global.BadClientCredential);
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
                        await transaction.Commit(token);
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
                    throw new OAuthException(ErrorCodes.INVALID_REQUEST, Global.UnexpectedRequestUriParameter);
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
}
