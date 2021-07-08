// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Provisioning.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Provisioning.Provisioner
{
    public class HTTPProvisioner : IProvisioner
    {
        public ProvisioningConfigurationTypes Type => ProvisioningConfigurationTypes.API;

        public async Task Seed(ProvisioningOperations operation, string representationId, JObject representation, ProvisioningConfiguration configuration, CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessToken(configuration, cancellationToken);
            HttpRequestMessage request = null;
            switch(operation)
            {
                case ProvisioningOperations.ADD:
                    {
                        var httpRequest = BuildHTTPRequest(representation, configuration);
                        var content = new JObject();
                        content.Add("scim_id", representationId);
                        content.Add("content", httpRequest);
                        request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Post,
                            RequestUri = new Uri(configuration.GetTargetUrl()),
                            Content = new StringContent(content.ToString(), Encoding.UTF8, "application/json")
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

        protected JObject BuildHTTPRequest(JObject representation, ProvisioningConfiguration configuration)
        {
            var result = new JObject();
            var mappingRules = configuration.GetMappingRules();
            foreach (var mappingRule in mappingRules)
            {
                var token = representation.SelectToken(mappingRule.Key);
                if (token == null)
                {
                    continue;
                }

                var splitted = mappingRule.Value.Split('.');
                JObject childRecord = null;
                if (splitted.Length == 1)
                {
                    result.Add(splitted.First(), token);
                }
                else
                {
                    for (int i = splitted.Length - 1; i >= 0; i--)
                    {
                        var name = splitted[i];
                        if (childRecord == null)
                        {
                            childRecord = new JObject();
                            childRecord.Add(name, token);
                            continue;
                        }

                        if (i == 0)
                        {
                            var cl = result.SelectToken(name) as JObject;
                            if (cl != null)
                            {
                                foreach(var kvp in childRecord)
                                {
                                    cl.Add(kvp.Key, kvp.Value);
                                }
                            }
                            else
                            {
                                result.Add(name, childRecord);
                            }
                        }
                        else
                        {
                            var rec = new JObject();
                            rec.Add(name, childRecord);
                            childRecord = rec;
                        }
                    }
                }
            }

            return result;
        }
    }
}
