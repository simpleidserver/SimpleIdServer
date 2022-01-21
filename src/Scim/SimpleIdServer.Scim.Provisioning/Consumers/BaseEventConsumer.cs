// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Logging;
using SimpleIdServer.Common.ExternalEvents;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Provisioning.Provisioner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Provisioning.Consumers
{
    public abstract class BaseEventConsumer<TMessage> where TMessage : IntegrationEvent
    {
        private readonly IEnumerable<IProvisioner> _provisioners;
        private readonly IProvisioningConfigurationRepository _provisioningConfigurationRepository;
        private readonly ILogger<BaseEventConsumer<TMessage>> _logger;

        public BaseEventConsumer(
            IEnumerable<IProvisioner> provisioners,
            IProvisioningConfigurationRepository provisioningConfigurationRepository,
            ILogger<BaseEventConsumer<TMessage>> logger)
        {
            _provisioners = provisioners;
            _provisioningConfigurationRepository = provisioningConfigurationRepository;
            _logger = logger;
        }

        protected abstract ProvisioningOperations Type { get; }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var token = CancellationToken.None;
            var configurations = await _provisioningConfigurationRepository.GetAll(token);
            foreach (var configuration in configurations)
            {
                if (configuration.IsRepresentationProvisioned(context.Message.Id, context.Message.Version) 
                    || configuration.ResourceType != context.Message.ResourceType)
                {
                    continue;
                }

                var provisioner = _provisioners.First(p => p.Type == configuration.Type);
                ITransaction transaction = null;
                var description = $"{Enum.GetName(typeof(ProvisioningOperations), Type)} the resource {configuration.ResourceType}";
                try
                {
                    transaction = await _provisioningConfigurationRepository.StartTransaction(token);
                    await provisioner.Seed(Type,
                        context.Message.Id,
                        context.Message.Representation,
                        configuration,
                        token);
                    var workflowResult = await LaunchWorkflow(configuration, context);
                    configuration.Complete(context.Message.Id, description, workflowResult.InstanceId, workflowResult.FileId, context.Message.Version);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    configuration.Error(context.Message.Id, description, context.Message.Version, ex.ToString());
                }
                finally
                {
                    await _provisioningConfigurationRepository.Update(configuration, token);
                    await transaction.Commit(token);
                }
            }
        }

        protected abstract Task<WorkflowResult> LaunchWorkflow(ProvisioningConfiguration configuration, ConsumeContext<TMessage> context);
    }
}
