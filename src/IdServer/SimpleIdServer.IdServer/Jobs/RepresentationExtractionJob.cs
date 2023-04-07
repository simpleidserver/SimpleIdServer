// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.ExternalEvents;
using SimpleIdServer.IdServer.Options;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace SimpleIdServer.IdServer.Jobs
{
    public interface IRepresentationExtractionJob
    {
        public string Name { get; }
        public Task Execute(string instanceId, string prefix);
    }

    public abstract class RepresentationExtractionJob<T> : IRepresentationExtractionJob where T : class
    {
        public RepresentationExtractionJob(ILogger<RepresentationExtractionJob<T>> logger, IBusControl busControl, IIdentityProvisioningStore identityProvisioningStore, IExtractedRepresentationRepository  extractedRepresentationRepository, IOptions<IdServerHostOptions> options)
        {
            Logger = logger;
            BusControl = busControl;
            IdentityProvisioningStore = identityProvisioningStore;
            ExtractedRepresentationRepository = extractedRepresentationRepository;
            Options = options.Value;
        }

        public abstract string Name { get; }
        protected IBusControl BusControl { get; private set; }
        protected ILogger<RepresentationExtractionJob<T>> Logger { get; private set; }
        protected IdServerHostOptions Options { get; private set; }
        protected IIdentityProvisioningStore IdentityProvisioningStore { get; private set; }
        protected IExtractedRepresentationRepository ExtractedRepresentationRepository { get; private set; }

        public async Task Execute(string instanceId, string prefix)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity($"Start to export {Name} users"))
            {
                Logger.LogInformation($"Start to export {Name} users");
                var metadata = await IdentityProvisioningStore.Query()
                    .Include(d => d.Properties)
                    .Include(d => d.Realms)
                    .Include(d => d.Histories)
                    .Include(d => d.Definition).ThenInclude(d => d.MappingRules)
                    .FirstOrDefaultAsync(i => i.Id == instanceId && i.Realms.Any(r => r.Name == prefix));
                if (!metadata.IsEnabled)
                {
                    Logger.LogInformation($"The job {instanceId} is disabled");
                    return;
                }

                var startDateTime = DateTime.UtcNow;
                try
                {
                    var destinationFolder = Path.Combine(Options.ExtractRepresentationsFolder, Name);
                    if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
                    var folderName = DateTime.Now.ToString("MMddyyyyHHmm");
                    destinationFolder = Path.Combine(destinationFolder, folderName);
                    Directory.CreateDirectory(destinationFolder);
                    var options = Serializer.PropertiesSerializer.DeserializeOptions<T, IdentityProvisioningProperty>(metadata.Properties);
                    using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        int nbUsers = 0;
                        await foreach(var users in FetchUsers(options, destinationFolder, metadata))
                        {
                            await ExtractedRepresentationRepository.BulkUpdate(users);
                            nbUsers += users.Count();
                        }

                        Logger.LogInformation($"{nbUsers} users has been exported into {destinationFolder}");
                        activity?.SetStatus(ActivityStatusCode.Ok, $"{nbUsers} users has been exported into {destinationFolder}");
                        var endDateTime = DateTime.UtcNow;
                        metadata.Export(startDateTime, endDateTime, folderName, nbUsers);
                        await BusControl.Publish(new ExtractRepresentationsSuccessEvent
                        {
                            IdentityProvisioningName = Name,
                            NbRepresentations = nbUsers,
                            Realm = prefix
                        });
                        transactionScope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.ToString());
                    Logger.LogError(ex.ToString());
                    var endDateTime = DateTime.UtcNow;
                    metadata.FailToImport(startDateTime, endDateTime, ex.ToString());
                    await BusControl.Publish(new ExtractRepresentationsFailureEvent
                    {
                        IdentityProvisioningName = Name,
                        ErrorMessage = ex.ToString(),
                        Realm = prefix
                    });
                }
                finally
                {
                    await IdentityProvisioningStore.SaveChanges(CancellationToken.None);
                }
            }
        }

        protected abstract IAsyncEnumerable<List<ExtractedRepresentation>> FetchUsers(T options, string destinationFolder, IdentityProvisioning identityProvisioning);

        protected string BuildFileColumns(IdentityProvisioningDefinition definition) => $"Id;Version;{string.Join(";", definition.MappingRules.Select(r => r.Id))}";
    }
}
