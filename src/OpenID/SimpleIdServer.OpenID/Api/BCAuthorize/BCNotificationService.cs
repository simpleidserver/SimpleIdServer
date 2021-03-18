// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Options;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.Api.Authorization.ResponseTypes;
using SimpleIdServer.OpenID.Extensions;
using SimpleIdServer.OpenID.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.BCAuthorize
{
    public interface IBCNotificationService
    {
        Task Notify(HandlerContext handlerContext, string authReqId, CancellationToken cancellationToken);
    }

    public class BCNotificationService : IBCNotificationService
    {
        private readonly OpenIDHostOptions _options;

        public BCNotificationService(IOptions<OpenIDHostOptions> options)
        {
            _options = options.Value;
        }

        public async Task Notify(HandlerContext handlerContext, string authReqId, CancellationToken cancellationToken)
        {
            var deviceRegistrationToken = handlerContext.User.DeviceRegistrationToken;
            var clickAction = BuildExecutionUrl(handlerContext, authReqId);
            await FirebaseMessaging.DefaultInstance.SendAsync(new Message
            {   
                Token = deviceRegistrationToken,
                Android =new AndroidConfig
                {
                    Data = new Dictionary<string, string>
                    {
                        { "title", _options.FcmTitle },
                        { "body", _options.FcmBody },
                        { "clickAction", clickAction }
                    }
                }
            }, cancellationToken);
        }

        protected virtual string BuildExecutionUrl(HandlerContext handlerContext, string authReqId)
        {
            var scope = string.Join(" ", handlerContext.Request.Data.GetScopesFromAuthorizationRequest());
            var acrValues = string.Join(" ", handlerContext.Request.Data.GetAcrValuesFromAuthorizationRequest());
            var redirectUri = handlerContext.Request.Data.GetRedirectUriFromAuthorizationRequest();
            var dic = new Dictionary<string, string>
            {
                { AuthorizationRequestParameters.ClientId, handlerContext.Client.ClientId },
                { AuthorizationRequestParameters.ResponseType, IdTokenResponseTypeHandler.RESPONSE_TYPE },
                { AuthorizationRequestParameters.Scope, scope },
                { DTOs.AuthorizationRequestParameters.AcrValue, acrValues },
                { AuthorizationRequestParameters.RedirectUri, redirectUri },
                { AuthorizationRequestParameters.State, Guid.NewGuid().ToString() },
                { DTOs.AuthorizationRequestParameters.Nonce, Guid.NewGuid().ToString() },
                { DTOs.AuthorizationRequestParameters.Prompt, DTOs.PromptParameters.Consent },
                { DTOs.AuthorizationRequestParameters.AuthReqId, authReqId }
            };
            return $"{handlerContext.Request.IssuerName}/{OAuth.Constants.EndPoints.Authorization}?{string.Join("&", dic.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
        }
    }
}
