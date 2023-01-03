// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Api.Token.TokenBuilders;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using SimpleIdServer.Store;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Jobs
{
    public class PushNotificationHandler : IBCNotificationHandler
    {
        private readonly OAuth.Infrastructures.IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PingNotificationHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IEnumerable<ITokenBuilder> _tokenBuilders;

        public PushNotificationHandler(
            OAuth.Infrastructures.IHttpClientFactory httpClientFactory,
            ILogger<PingNotificationHandler> logger,
            IUserRepository userRepository,
            IClientRepository clientRepository,
            IEnumerable<ITokenBuilder> tokenBuilders)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
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
                    var jObjBody = new JsonObject();
                    var context = new HandlerContext(new HandlerContextRequest(null, null, new JsonObject(), null, null, null));
                    var user = await _userRepository.Query().AsNoTracking().FirstOrDefaultAsync(u => u.Id == bcAuthorize.UserId, cancellationToken);
                    var oauthClient = await _clientRepository.Query().AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == bcAuthorize.ClientId, cancellationToken);
                    context.SetUser(user);
                    context.SetClient(oauthClient);
                    foreach(var tokenBuilder in _tokenBuilders)
                        await tokenBuilder.Build(bcAuthorize.Scopes, context, cancellationToken);

                    var content = new JsonObject
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
