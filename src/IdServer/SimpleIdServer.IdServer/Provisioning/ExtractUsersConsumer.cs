// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using MassTransit;
using MassTransit.Initializers;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace SimpleIdServer.IdServer.Provisioning;

public class ExtractUsersConsumer : 
    IConsumer<StartExtractUsersCommand>,
    IConsumer<ExtractUsersCommand>,
    IConsumer<CheckUsersExtractedCommand>
{
    private TimeSpan _checkInterval = TimeSpan.FromSeconds(5);
    private readonly IIdentityProvisioningStore _identityProvisioningStore;
    private readonly IEnumerable<IProvisioningService> _provisioningServices;
    private readonly IProvisioningStagingStore _stagingStore;
    private readonly ITransactionBuilder _transactionBuilder;
    public static string Queuename = "extract-users";

    public ExtractUsersConsumer(
        IIdentityProvisioningStore identityProvisioningStore,
        IEnumerable<IProvisioningService> provisioningServices,
        IProvisioningStagingStore stagingStore,
        ITransactionBuilder transactionBuilder)
    {

        _identityProvisioningStore = identityProvisioningStore;
        _provisioningServices = provisioningServices;
        _stagingStore = stagingStore;
        _transactionBuilder = transactionBuilder;
    }

    public async Task Consume(ConsumeContext<StartExtractUsersCommand> context)
    {
        using (var transaction = _transactionBuilder.Build())
        {
            var message = context.Message;
            var instance = await _identityProvisioningStore.Get(message.Realm, message.InstanceId, context.CancellationToken);
            if (!instance.IsEnabled)
            {
                return;
            }
            var provisioningService = _provisioningServices.Single(p => p.Name == instance.Definition.Name);
            var pages = (await provisioningService.Paginate(instance.Definition, context.CancellationToken)).OrderBy(p => p.Page);
            var destination = new Uri($"queue:{ExtractUsersConsumer.Queuename}");
            foreach (var page in pages.OrderBy(p => p.Page))
            {
                await context.Send(destination, new ExtractUsersCommand
                {
                    InstanceId = message.InstanceId,
                    ProcessId = message.ProcessId,
                    BatchSize = page.BatchSize,
                    Page = page.Page,
                    Realm = message.Realm
                });
            }

            instance.Start(message.ProcessId, pages.Last().Page);
            _identityProvisioningStore.Update(instance);
            await transaction.Commit(context.CancellationToken);
            await context.ScheduleSend(destination, _checkInterval, new CheckUsersExtractedCommand
            {
                InstanceId = message.InstanceId,
                ProcessId = message.ProcessId,
                Realm = message.Realm
            });
        }
    }

    public async Task Consume(ConsumeContext<ExtractUsersCommand> context)
    {
        var message = context.Message;
        var instance = await _identityProvisioningStore.Get(message.Realm, message.InstanceId, CancellationToken.None);
        var provisioningService = _provisioningServices.Single(p => p.Name == instance.Definition.Name);
        var extractionResult = await provisioningService.Extract(new ExtractionPage
        {
            BatchSize = message.BatchSize,
            Page = message.Page
        }, instance.Definition, context.CancellationToken);
        var newRepresentations = Extract(message.ProcessId, extractionResult, instance.Definition);
        var newRepresentationIds = newRepresentations.Select(r => r.RepresentationId);
        var lastRepresentations = await _stagingStore.GetLastExtractedRepresentations(newRepresentationIds, context.CancellationToken);
        // Filter representations.
        var newOrChangedRepresentations = newRepresentations.Where((r) =>
        {
            var er = lastRepresentations.SingleOrDefault(er => er.ExternalId == r.RepresentationId);
            return er == null || er.Version != r.RepresentationVersion;
        }).ToList();
        var filteredRepresentations = newRepresentations.Where((r) =>
        {
            var er = lastRepresentations.SingleOrDefault(er => er.ExternalId == r.RepresentationId);
            return er != null && er.Version == r.RepresentationVersion;
        }).ToList();

        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }, TransactionScopeAsyncFlowOption.Enabled))
        {
            // Store the representation id and its version.
            await _stagingStore.BulkUpdate(newOrChangedRepresentations.Select(r => new ExtractedRepresentation
            {
                ExternalId = r.RepresentationId,
                Version = r.RepresentationVersion
            }).ToList(), context.CancellationToken);
            // Store the extracted representations.
            await _stagingStore.BulkUpdate(newOrChangedRepresentations, context.CancellationToken);
            instance.Extract(message.ProcessId,
                message.Page,
                newOrChangedRepresentations.Count(r => r.Type == ExtractedRepresentationType.USER),
                newOrChangedRepresentations.Count(u => u.Type == ExtractedRepresentationType.GROUP),
                filteredRepresentations.Count());
            // await _identityProvisioningStore.SaveChanges(context.CancellationToken);
            transactionScope.Complete();
        }
    }

    public async Task Consume(ConsumeContext<CheckUsersExtractedCommand> context)
    {
        var message = context.Message;
        var instance = await _identityProvisioningStore.Get(message.Realm, message.InstanceId, CancellationToken.None);
        var process = instance.GetProcess(message.ProcessId);
        if(process.TotalPageToExtract == process.NbExtractedPages)
        {
            instance.FinishExtract(message.ProcessId);
            // await _identityProvisioningStore.SaveChanges(context.CancellationToken);
            return;
        }

        var destination = new Uri($"queue:{ExtractUsersConsumer.Queuename}");
        await context.ScheduleSend(destination, _checkInterval, new CheckUsersExtractedCommand
        {
            InstanceId = message.InstanceId,
            ProcessId = message.ProcessId,
            Realm = message.Realm
        });
    }

    private List<ExtractedRepresentationStaging> Extract(string processId, ExtractedResult extractedResult, IdentityProvisioningDefinition definition)
    {
        var extractedRepresentations = new List<ExtractedRepresentationStaging>();
        var userColumns = definition.MappingRules.Where(r => r.Usage == IdentityProvisioningMappingUsage.USER).Select(r => r.Id);
        foreach (var user in extractedResult.Users)
        {
            var json = new JsonObject();
            for(var i = 0; i < userColumns.Count(); i++)
            {
                if (user.Values.Count() <= i) break;
                json.Add(userColumns.ElementAt(i), user.Values[i]);
            }

            extractedRepresentations.Add(new ExtractedRepresentationStaging
            {
                Id = $"{user.Id}_{user.Version}",
                RepresentationId = user.Id,
                RepresentationVersion = user.Version,
                Values = json.ToJsonString(),
                GroupIds = user.GroupIds,
                IdProvisioningProcessId = processId,
                Type = ExtractedRepresentationType.USER
            });
        }

        var groupColumns = definition.MappingRules.Where(r => r.Usage == IdentityProvisioningMappingUsage.GROUP).Select(r => r.Id);
        foreach (var group in extractedResult.Groups)
        {
            var json = new JsonObject();
            for (var i = 0; i < groupColumns.Count(); i++)
            {
                if (group.Values.Count() <= i) break;
                json.Add(groupColumns.ElementAt(i), group.Values[i]);
            }

            extractedRepresentations.Add(new ExtractedRepresentationStaging
            {
                Id = $"{group.Id}_{group.Version}",
                RepresentationId = group.Id,
                RepresentationVersion = group.Version,
                Values = json.ToJsonString(),
                IdProvisioningProcessId = processId,
                Type = ExtractedRepresentationType.GROUP
            });
        }

        return extractedRepresentations;
    }
}

public class ExtractUsersConsumerDefinition : ConsumerDefinition<ExtractUsersConsumer>
{
    public ExtractUsersConsumerDefinition()
    {
        EndpointName = ExtractUsersConsumer.Queuename;
        ConcurrentMessageLimit = 8;
    }
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<ExtractUsersConsumer> consumerConfigurator)
    {
        // configure message retry with millisecond intervals
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));

        // use the outbox to prevent duplicate events from being published
        endpointConfigurator.UseInMemoryOutbox();
    }
}