// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OAuth.Persistence;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Jobs
{
    public class PushNotificationHandler : IBCNotificationHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PingNotificationHandler> _logger;
        private readonly IOAuthUserQueryRepository _oauthUserQueryRepository;
        private readonly IOAuthClientQueryRepository _oauthClientQueryRepository;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;

        public PushNotificationHandler(
            IHttpClientFactory httpClientFactory,
            ILogger<PingNotificationHandler> logger,
            IOAuthUserQueryRepository oauthUserQueryRepository,
            IOAuthClientQueryRepository oauthClientQueryRepository,
            IEnumerable<ITokenBuilder> tokenBuilders)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _oauthUserQueryRepository = oauthUserQueryRepository;
            _oauthClientQueryRepository = oauthClientQueryRepository;
            _tokenBuilders = tokenBuilders;
        }

        public string NotificationMode => SIDOpenIdConstants.StandardNotificationModes.Push;

        public async Task<bool> Notify(BCAuthorize bcAuthorize, CancellationToken cancellationToken)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = false
                };
                using (var httpClient = _httpClientFactory.GetHttpClient(handler))
                {
                    var jObjBody = new JObject();
                    var context = new HandlerContext(new HandlerContextRequest(null, null, jObjBody, null, null, null));
                    var user = await _oauthUserQueryRepository.FindOAuthUserByLogin(bcAuthorize.UserId, cancellationToken);
                    var oauthClient = await _oauthClientQueryRepository.FindOAuthClientById(bcAuthorize.ClientId, cancellationToken);
                    context.SetUser(user);
                    context.SetClient(oauthClient);
                    foreach(var tokenBuilder in _tokenBuilders)
                    {
                        await tokenBuilder.Build(bcAuthorize.Scopes, context, cancellationToken);
                    }

                    var content = new JObject
                    {
                        { BCAuthenticationResponseParameters.AuthReqId, bcAuthorize.Id }
                    };
                    foreach(var resp in context.Response.Parameters)
                    {
                        content.Add(resp.Key, resp.Value);
                    }

                    var httpRequestMessage = new HttpRequestMessage
                    {
                        RequestUri = new Uri(bcAuthorize.NotificationEdp),
                        Content = new StringContent(content.ToString(), Encoding.UTF8, "application/json")
                    };
                    httpRequestMessage.Headers.Add("Authorization", bcAuthorize.NotificationToken);
                    var httpResult = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                    httpResult.EnsureSuccessStatusCode();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }
    }
}
