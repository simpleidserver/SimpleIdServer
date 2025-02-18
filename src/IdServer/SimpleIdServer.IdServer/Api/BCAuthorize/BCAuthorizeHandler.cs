// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Helpers;
using SimpleIdServer.IdServer.Layout;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using static SimpleIdServer.IdServer.Constants;

namespace SimpleIdServer.IdServer.Api.BCAuthorize
{
    public interface IBCAuthorizeHandler
    {
        Task<IActionResult> Create(HandlerContext context, CancellationToken cancellationToken);
    }

    public class BCAuthorizeHandler : IBCAuthorizeHandler
    {
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;
        private readonly IBCAuthorizeRequestValidator _bcAuthorizeRequestValidator;
        private readonly IBCNotificationService _bcNotificationService;
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;
        private readonly IAmrHelper _amrHelper;
        private readonly ITransactionBuilder _transactionBuilder;

        public BCAuthorizeHandler(
            IClientAuthenticationHelper clientAuthenticationHelper,
            IBCAuthorizeRequestValidator bcAuthorizeRequestValidator,
            IBCNotificationService bcNotificationService,
            IBCAuthorizeRepository bcAuthorizeRepository,
            IAmrHelper amrHelper,
            ITransactionBuilder transactionBuilder)
        {
            _clientAuthenticationHelper = clientAuthenticationHelper;
            _bcAuthorizeRequestValidator = bcAuthorizeRequestValidator;
            _bcNotificationService = bcNotificationService;
            _bcAuthorizeRepository = bcAuthorizeRepository;
            _amrHelper = amrHelper;
            _transactionBuilder = transactionBuilder;
        }

        public async Task<IActionResult> Create(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                using (var transaction = _transactionBuilder.Build())
                {
                    Client oauthClient = await _clientAuthenticationHelper.AuthenticateClient(context.Realm, context.Request.HttpHeader, context.Request.RequestData, context.Request.Certificate, context.GetIssuer(), cancellationToken, ErrorCodes.INVALID_REQUEST);
                    context.SetClient(oauthClient);
                    var user = await _bcAuthorizeRequestValidator.ValidateCreate(context, cancellationToken);
                    context.SetUser(user, null);
                    var requestedExpiry = context.Request.RequestData.GetRequestedExpiry() ?? context.Client.AuthReqIdExpirationTimeInSeconds;
                    var currentDateTime = DateTime.UtcNow;
                    var openidClient = oauthClient;
                    var interval = oauthClient.BCIntervalSeconds;
                    var bcAuthorize = Domains.BCAuthorize.Create(
                        currentDateTime.AddSeconds(requestedExpiry),
                        oauthClient.ClientId,
                        interval,
                        openidClient.BCClientNotificationEndpoint,
                        openidClient.BCTokenDeliveryMode,
                        context.Request.RequestData.GetScopesFromAuthorizationRequest(),
                        context.Request.RequestData.GetAuthorizationDetailsFromAuthorizationRequest(),
                        context.User.Id,
                        context.Request.RequestData.GetClientNotificationToken(),
                        context.Realm);
                    bcAuthorize.IncrementNextFetchTime();
                    _bcAuthorizeRepository.Add(bcAuthorize);
                    await transaction.Commit(cancellationToken);

                    var bindingMessage = context.Request.RequestData.GetBindingMessage();
                    var acrLst = context.Request.RequestData.GetAcrValuesFromAuthorizationRequest();
                    var acrResult = await _amrHelper.FetchDefaultAcr(context.Realm, FormCategories.Authentication, acrLst, new List<AuthorizedClaim>(), context.Client, cancellationToken);
                    var amr = acrResult.AllAmrs.First();
                    await _bcNotificationService.Notify(context, new BCNotificationMessage
                    {
                        ClientId = context.Client.ClientId,
                        AuthReqId = bcAuthorize.Id,
                        BindingMessage = bindingMessage,
                        Scopes = bcAuthorize.Scopes,
                        AcrLst = acrLst,
                        Amr = amr,
                        AuthorizationDetails = bcAuthorize.AuthorizationDetails
                    }, cancellationToken);

                    var res = new JsonObject
                {
                    { BCAuthenticationResponseParameters.AuthReqId, bcAuthorize.Id },
                    { BCAuthenticationResponseParameters.ExpiresIn, requestedExpiry },
                };
                    if (oauthClient.BCTokenDeliveryMode == StandardNotificationModes.Ping ||
                        oauthClient.BCTokenDeliveryMode == StandardNotificationModes.Poll)
                        res.Add(BCAuthenticationResponseParameters.Interval, interval);

                    return new OkObjectResult(res);
                }
            }
            catch (OAuthUnauthorizedException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.Unauthorized, ex.Code, ex.Message);
            }
            catch (OAuthException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }
    }
}
