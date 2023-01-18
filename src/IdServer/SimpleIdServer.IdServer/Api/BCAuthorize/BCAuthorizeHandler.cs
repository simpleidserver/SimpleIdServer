// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Api.BCDeviceRegistration;
using SimpleIdServer.IdServer.Api.Token.Handlers;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Store;
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
        Task<IActionResult> Confirm(HandlerContext context, CancellationToken cancellationToken);
        Task<IActionResult> Reject(HandlerContext context, CancellationToken cancellationToken);
    }

    public class BCAuthorizeHandler : IBCAuthorizeHandler
    {
        private readonly IClientAuthenticationHelper _clientAuthenticationHelper;
        private readonly IBCAuthorizeRequestValidator _bcAuthorizeRequestValidator;
        private readonly IBCNotificationService _bcNotificationService;
        private readonly IBCAuthorizeRepository _bcAuthorizeRepository;
        private readonly IEnumerable<IDevice> _devices;

        public BCAuthorizeHandler(
            IClientAuthenticationHelper clientAuthenticationHelper,
            IBCAuthorizeRequestValidator bcAuthorizeRequestValidator,
            IBCNotificationService bcNotificationService,
            IBCAuthorizeRepository bcAuthorizeRepository,
            IEnumerable<IDevice> devices)
        {
            _clientAuthenticationHelper = clientAuthenticationHelper;
            _bcAuthorizeRequestValidator = bcAuthorizeRequestValidator;
            _bcNotificationService = bcNotificationService;
            _bcAuthorizeRepository = bcAuthorizeRepository;
            _devices = devices;
        }

        public async Task<IActionResult> Create(HandlerContext context, CancellationToken cancellationToken)
        {
            try
            {
                // https://www.authlete.com/developers/grant_management/
                // https://danielfett.de/download/fapi-evolution.pdf
                // https://www.openbanking.org.uk/wp-content/uploads/Customer-Experience-Guidelines.pdf
                // https://datatracker.ietf.org/doc/html/draft-ietf-oauth-rar-04#name-request-parameter-authoriza
                // https://danielfett.de/download/fapi-evolution.pdf
                // https://bitbucket.org/openid/fapi/src/master/fapi-grant-management.md
                Client oauthClient = await _clientAuthenticationHelper.AuthenticateClient(context.Request.HttpHeader, context.Request.RequestData, context.Request.Certificate, context.Request.IssuerName, cancellationToken, ErrorCodes.INVALID_REQUEST);
                context.SetClient(oauthClient);
                var user = await _bcAuthorizeRequestValidator.ValidateCreate(context, cancellationToken);
                context.SetUser(user);
                var requestedExpiry = context.Request.RequestData.GetRequestedExpiry() ?? context.Client.AuthReqIdExpirationTimeInSeconds;
                var currentDateTime = DateTime.UtcNow;
                var openidClient = oauthClient;
                var permissions = await GetPermissions(context.Client.ClientId, context.User.Id, cancellationToken);
                var interval = oauthClient.BCIntervalSeconds;
                var bcAuthorize = Domains.BCAuthorize.Create(
                    currentDateTime.AddSeconds(requestedExpiry),
                    oauthClient.ClientId,
                    interval,
                    openidClient.BCClientNotificationEndpoint,
                    openidClient.BCTokenDeliveryMode,
                    context.Request.RequestData.GetScopesFromAuthorizationRequest(),
                    context.User.Id,
                    context.Request.RequestData.GetClientNotificationToken(),
                    permissions);
                bcAuthorize.IncrementNextFetchTime();
                _bcAuthorizeRepository.Add(bcAuthorize);
                await _bcAuthorizeRepository.SaveChanges(cancellationToken);
                foreach (var grp in permissions.GroupBy(p => p.ConsentId))
                    await _bcNotificationService.Notify(context, bcAuthorize.Id, grp.ToArray(), cancellationToken);

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
            catch (OAuthUnauthorizedException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.Unauthorized, ex.Code, ex.Message);
            }
            catch (OAuthException ex)
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
                await _bcAuthorizeRepository.SaveChanges(cancellationToken);
                return new NoContentResult();
            }
            catch (OAuthException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }

        /// <summary>
        /// Get permissions.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="subject"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Task<IEnumerable<BCAuthorizePermission>> GetPermissions(string clientId, string subject, CancellationToken cancellationToken)
        {
            IEnumerable<BCAuthorizePermission> result = new List<BCAuthorizePermission>();
            return Task.FromResult(result);
        }

        /// <summary>
        /// Confirm permissions.
        /// </summary>
        /// <param name="permissions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Task ConfirmPermissions(IEnumerable<BCAuthorizePermission> permissions, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Reject permissions.
        /// </summary>
        /// <param name="permissions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Task RejectPermissions(IEnumerable<BCAuthorizePermission> permissions, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
