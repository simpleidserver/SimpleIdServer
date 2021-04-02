// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Infrastructures;
using SimpleIdServer.OpenID.Domains;
using SimpleIdServer.OpenID.DTOs;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Jobs
{
    public class PingNotificationHandler : IBCNotificationHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PingNotificationHandler> _logger;

        public PingNotificationHandler(
            IHttpClientFactory httpClientFactory,
            ILogger<PingNotificationHandler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public string NotificationMode => SIDOpenIdConstants.StandardNotificationModes.Ping;

        public async Task<bool> Notify(BCAuthorize bcAuthorize, CancellationToken cancellationToken)
        {
            try
            {
                using (var httpClient = _httpClientFactory.GetHttpClient())
                {
                    var content = new JObject
                    {
                        { BCAuthenticationResponseParameters.AuthReqId, bcAuthorize.Id }
                    };
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
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }
    }
}
