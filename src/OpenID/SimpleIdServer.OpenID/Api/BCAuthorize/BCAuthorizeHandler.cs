// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Options;
using SimpleIdServer.OpenID.Persistence;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.BCAuthorize
{
    public interface IBCAuthorizeHandler
    {
        Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken);
    }

    public class BCAuthorizeHandler: IBCAuthorizeHandler
    {
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;
        private readonly IBCAuthorizeRequestValidator _bcAuthorizeRequestValidator;
        private readonly IBCNotificationService _bcNotificationService;
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;
        private readonly OpenIDHostOptions _options;

        public BCAuthorizeHandler(
            IClientAuthenticationHelper clientAuthenticationHelper, 
            IBCAuthorizeRequestValidator bcAuthorizeRequestValidator,
            IBCNotificationService bcNotificationService,
            IBCAuthorizeRepository bcAuthorizeRepository,
            IOptions<OpenIDHostOptions> options)
        {
            _clientAuthenticationHelper = clientAuthenticationHelper;
            _bcAuthorizeRequestValidator = bcAuthorizeRequestValidator;
            _bcNotificationService = bcNotificationService;
            _bcAuthorizeRepository = bcAuthorizeRepository;
            _options = options.Value;
        }

        public async Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                var oauthClient = await _clientAuthenticationHelper.AuthenticateClient(context.Request.HttpHeader, context.Request.Data, context.Request.Certificate, context.Request.IssuerName, cancellationToken);
                context.SetClient(oauthClient);
                var user = await _bcAuthorizeRequestValidator.Validate(context, cancellationToken);
                context.SetUser(user);
                var requestedExpiry = context.Request.Data.GetRequestedExpiry();
                var interval = context.Request.Data.GetInterval();
                if (requestedExpiry == null)
                {
                    requestedExpiry = _options.AuthRequestExpirationTimeInSeconds;
                }

                var currentDateTime = DateTime.UtcNow;
                var openidClient = oauthClient as OpenIdClient;
                var bcAuthorize = new Domains.BCAuthorize
                {
                    Id = Guid.NewGuid().ToString(),
                    ExpirationDateTime = currentDateTime.AddSeconds(requestedExpiry.Value),
                    ClientId = oauthClient.ClientId,
                    Interval = interval ?? _options.DefaultBCAuthorizeWaitIntervalInSeconds,
                    NotificationEdp = openidClient.BCClientNotificationEndpoint,
                    NotificationMode = openidClient.BCTokenDeliveryMode,
                    Status = BCAuthorizeStatus.Pending,
                    Scopes = context.Request.Data.GetScopesFromAuthorizationRequest(),
                    UserId = context.User.Id,
                    NotificationToken = context.Request.Data.GetClientNotificationToken()
                };
                bcAuthorize.IncrementNextFetchTime();
                await _bcAuthorizeRepository.Add(bcAuthorize, cancellationToken);
                await _bcAuthorizeRepository.SaveChanges(cancellationToken);
                await _bcNotificationService.Notify(context, bcAuthorize.Id, cancellationToken);
                return new OkObjectResult(new JObject
                {
                    { BCAuthenticationResponseParameters.AuthReqId, bcAuthorize.Id },
                    { BCAuthenticationResponseParameters.ExpiresIn, requestedExpiry.Value }
                });
            }
            catch(OAuthException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }
    }
}
