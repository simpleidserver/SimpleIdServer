// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Provisioning.Extensions;
using SimpleIdServer.Scim.Provisioning.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Provisioning.Provisioner
{
    public class HTTPProvisioner : IProvisioner
    {
        private readonly ILogger<HTTPProvisioner> _logger;

        public HTTPProvisioner(ILogger<HTTPProvisioner> logger)
        {
            _logger = logger;
        }

        public ProvisioningConfigurationTypes Type => ProvisioningConfigurationTypes.API;

        public async Task Seed(ProvisioningOperations operation, string representationId, JObject representation, ProvisioningConfiguration configuration, CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessToken(configuration, cancellationToken);
            HttpRequestMessage request = null;
            switch (operation)
            {
                case ProvisioningOperations.ADD:
                    {
                        var content = BuildHTTPRequest(representation, configuration);
                        request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Post,
                            RequestUri = new Uri(configuration.GetTargetUrl()),
                            Content = new StringContent(content, Encoding.UTF8, "application/json")
                        };
                    }
                    break;
                case ProvisioningOperations.UPDATE:
                    {
                        var httpRequest = BuildHTTPRequest(representation, configuration);
                        request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Put,
                            RequestUri = new Uri($"{configuration.GetTargetUrl()}/{representationId}"),
                            Content = new StringContent(httpRequest.ToString(), Encoding.UTF8, "application/json")
                        };
                    }
                    break;
                case ProvisioningOperations.DELETE:
                    request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri($"{configuration.GetTargetUrl()}/{representationId}")
                    };
                    break;
            }

            using (var httpClient = new HttpClient())
            {
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                var httpResult = await httpClient.SendAsync(request, cancellationToken);
                httpResult.EnsureSuccessStatusCode();
            }
        }

        protected async Task<string> GetAccessToken(ProvisioningConfiguration configuration, CancellationToken cancellatinToken)
        {
            using (var httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(configuration.GetTokenEndpoint()),
                    Method = HttpMethod.Post,
                    Content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "client_id", configuration.GetClientId() },
                        { "client_secret", configuration.GetClientSecret() },
                        { "grant_type", "client_credentials" },
                        { "scope", string.Join(" ", configuration.GetScopes()) }
                    })
                };
                var httpResult = await httpClient.SendAsync(request, cancellatinToken);
                var json = await httpResult.Content.ReadAsStringAsync(cancellatinToken);
                var jObj = JObject.Parse(json);
                return jObj["access_token"].ToString();
            }
        }

        protected string BuildHTTPRequest(JObject representation, ProvisioningConfiguration configuration)
        {
            var template = configuration.GetHttpRequestTemplate();
            return TemplateParser.ParseMessage(template, representation);
        }
    }
}
