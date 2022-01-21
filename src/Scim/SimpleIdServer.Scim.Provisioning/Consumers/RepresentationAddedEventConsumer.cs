// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.ExternalEvents;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Provisioning.Extensions;
using SimpleIdServer.Scim.Provisioning.Helpers;
using SimpleIdServer.Scim.Provisioning.Provisioner;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Provisioning.Consumers
{
    public class RepresentationAddedEventConsumer : BaseEventConsumer<RepresentationAddedEvent>, IConsumer<RepresentationAddedEvent>
    {
        public RepresentationAddedEventConsumer(
            IEnumerable<IProvisioner> provisioners,
            IProvisioningConfigurationRepository provisioningConfigurationRepository,
            ILogger<BaseEventConsumer<RepresentationAddedEvent>> logger) : base(provisioners, provisioningConfigurationRepository, logger)
        {
        }

        protected override ProvisioningOperations Type => ProvisioningOperations.ADD;

        protected override async Task<WorkflowResult> LaunchWorkflow(ProvisioningConfiguration configuration, ConsumeContext<RepresentationAddedEvent> context)
        {
            using (var httpClient = new HttpClient())
            {
                var processInstanceId = await CreateProcessInstance(configuration, httpClient);
                await LaunchProcessInstance(processInstanceId, configuration, httpClient);
                await RaiseEvent(processInstanceId, configuration, context, httpClient);
                return new WorkflowResult(processInstanceId, configuration.GetBpmnFileId());
            }
        }

        private async Task<string> CreateProcessInstance(ProvisioningConfiguration configuration, HttpClient httpClient)
        {
            var content = new JObject();
            content.Add("processFileId", configuration.GetBpmnFileId());
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{configuration.GetBpmnEndpoint()}/processinstances"),
                Method = HttpMethod.Post,
                Content = new StringContent(content.ToString(), Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(request);
            httpResult.EnsureSuccessStatusCode();
            var str = await httpResult.Content.ReadAsStringAsync();
            return JObject.Parse(str).SelectToken("content[0].id").ToString();
        }

        private async Task LaunchProcessInstance(string processInstanceId, ProvisioningConfiguration configuration, HttpClient httpClient)
        {
            var httpResult = await httpClient.GetAsync($"{configuration.GetBpmnEndpoint()}/processinstances/{processInstanceId}/start");
            httpResult.EnsureSuccessStatusCode();
        }

        private async Task RaiseEvent(string processInstanceId, ProvisioningConfiguration configuration, ConsumeContext<RepresentationAddedEvent> context,  HttpClient httpClient)
        {
            var message = ParseMessageToken(configuration, context);
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{configuration.GetBpmnEndpoint()}/processinstances/{processInstanceId}/messages"),
                Method = HttpMethod.Post,
                Content = new StringContent(message, Encoding.UTF8, "application/json")
            };
            var httpResult = await httpClient.SendAsync(request);
            httpResult.EnsureSuccessStatusCode();
        }

        private static string ParseMessageToken(ProvisioningConfiguration configuration, ConsumeContext<RepresentationAddedEvent> context)
        {
            var representation = context.Message.Representation;
            return TemplateParser.ParseMessage(configuration.GetMessageToken(), representation);
        }
    }
}
