// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.Domains;
using SimpleIdServer.OAuth;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Api.Token.Helpers;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.OpenID.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.BCAuthorize
{
    public interface IBCAuthorizeHandler
    {
        Task<IActionResult> Create(HandlerContext context, CancellationToken cancellationToken);
        Task<IActionResult> Confirm(HandlerContext context, CancellationToken cancellationToken);
        Task<IActionResult> Reject(HandlerContext context, CancellationToken cancellationToken);
    }

    public class BCAuthorizeHandler: IBCAuthorizeHandler
    {
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;
        private readonly IBCAuthorizeRequestValidator _bcAuthorizeRequestValidator;
        private readonly IBCNotificationService _bcNotificationService;
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;
        private readonly OAuthHostOptions _options;

        public BCAuthorizeHandler(
            IClientAuthenticationHelper clientAuthenticationHelper,
            IBCAuthorizeRequestValidator bcAuthorizeRequestValidator,
            IBCNotificationService bcNotificationService,
            IBCAuthorizeRepository bcAuthorizeRepository,
            IOptions<OAuthHostOptions> options)
        {
            _clientAuthenticationHelper = clientAuthenticationHelper;
            _bcAuthorizeRequestValidator = bcAuthorizeRequestValidator;
            _bcNotificationService = bcNotificationService;
            _bcAuthorizeRepository = bcAuthorizeRepository;
            _options = options.Value;
        }

        public async Task<IActionResult> Create(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                Client oauthClient = await _clientAuthenticationHelper.AuthenticateClient(context.Request.HttpHeader, context.Request.RequestData, context.Request.Certificate, context.Request.IssuerName, cancellationToken, ErrorCodes.INVALID_REQUEST);
                context.SetClient(oauthClient);
                var user = await _bcAuthorizeRequestValidator.ValidateCreate(context, cancellationToken);
                context.SetUser(user);
                var requestedExpiry = context.Request.RequestData.GetRequestedExpiry();
                var interval = context.Request.RequestData.GetInterval();
                if (requestedExpiry == null)
                    requestedExpiry = _options.GetAuthRequestExpirationTimeInSeconds();

                var currentDateTime = DateTime.UtcNow;
                var openidClient = oauthClient;
                var permissions = await GetPermissions(context.Client.ClientId, context.User.Id, cancellationToken);
                var bcAuthorize = Domains.BCAuthorize.Create(
                    currentDateTime.AddSeconds(requestedExpiry.Value),
                    oauthClient.ClientId,
                    interval ?? _options.GetDefaultBCAuthorizeWaitIntervalInSeconds(),
                    openidClient.GetBCClientNotificationEndpoint(),
                    openidClient.GetBCTokenDeliveryMode(),
                    context.Request.RequestData.GetScopesFromAuthorizationRequest(),
                    context.User.Id,
                    context.Request.RequestData.GetClientNotificationToken(),
                    permissions);
                bcAuthorize.IncrementNextFetchTime();
                await _bcAuthorizeRepository.Add(bcAuthorize, cancellationToken);
                await _bcAuthorizeRepository.SaveChanges(cancellationToken);
                foreach(var grp in permissions.GroupBy(p => p.ConsentId))
                    await _bcNotificationService.Notify(context, bcAuthorize.Id, grp.ToArray(), cancellationToken);

                return new OkObjectResult(new JsonObject
                {
                    { BCAuthenticationResponseParameters.AuthReqId, bcAuthorize.Id },
                    { BCAuthenticationResponseParameters.ExpiresIn, requestedExpiry.Value }
                });
            }
            catch(OAuthUnauthorizedException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.Unauthorized, ex.Code, ex.Message);
            }
            catch(OAuthException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        public async Task<IActionResult> Confirm(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                var validationResult = await _bcAuthorizeRequestValidator.ValidateConfirm(context, cancellationToken);
                context.SetUser(validationResult.User);
                validationResult.Authorize.Confirm(context.Request.RequestData.GetPermissionIds());
                await ConfirmPermissions(validationResult.Authorize.Permissions.Where(p => p.IsConfirmed), cancellationToken);
                await _bcAuthorizeRepository.Update(validationResult.Authorize, cancellationToken);
                await _bcAuthorizeRepository.SaveChanges(cancellationToken);
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        public async Task<IActionResult> Reject(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                var authRequest = await _bcAuthorizeRequestValidator.ValidateReject(context, cancellationToken);
                authRequest.Reject();
                await RejectPermissions(authRequest.Permissions, cancellationToken);
                await _bcAuthorizeRepository.Update(authRequest, cancellationToken);
                await _bcAuthorizeRepository.SaveChanges(cancellationToken);
                return new NoContentResult();
            }
            catch(OAuthException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        protected virtual Task<IEnumerable<BCAuthorizePermission>> GetPermissions(string clientId, string subject, CancellationToken cancellationToken)
        {
            IEnumerable<BCAuthorizePermission> result = new List<BCAuthorizePermission>();
            return Task.FromResult(result);
        }

        protected virtual Task ConfirmPermissions(IEnumerable<BCAuthorizePermission> permissions, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected virtual Task RejectPermissions(IEnumerable<BCAuthorizePermission> permissions, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
