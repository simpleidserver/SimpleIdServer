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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace SimpleIdServer.IdServer.Jobs
{
    public interface IImportRepresentationJob
    {
        Task Execute(string realm, string id);
    }

    public class ImportRepresentationJob : IImportRepresentationJob
    {
        private readonly IIdentityProvisioningStore _identityProvisioningStore;
        private readonly IImportSummaryStore _importSummaryStore;
        private readonly IUserRepository _userRepository;
        private readonly IBusControl _busControl;
        private readonly IdServerHostOptions _options;
        private readonly ILogger<ImportRepresentationJob> _logger;

        public ImportRepresentationJob(IIdentityProvisioningStore identityProvisioningStore, IImportSummaryStore importSummaryStore, IUserRepository userRepository, IBusControl busControl, IOptions<IdServerHostOptions> options, ILogger<ImportRepresentationJob> logger)
        {
            _identityProvisioningStore = identityProvisioningStore;
            _importSummaryStore = importSummaryStore;
            _userRepository = userRepository;
            _busControl = busControl;
            _options = options.Value;
            _logger = logger;
        }

        public async Task Execute(string realm, string id)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Start to import SCIM users"))
            {
                var importSummary = ImportSummary.Create(id, realm);
                _importSummaryStore.Add(importSummary);
                await _importSummaryStore.SaveChanges(CancellationToken.None);
                var destinationFolder = _options.ExtractRepresentationsFolder;
                int nbImport = 0;
                try
                {
                    using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        foreach (var subDirectory in Directory.EnumerateDirectories(destinationFolder))
                            nbImport += await Import(new DirectoryInfo(subDirectory).Name, realm);

                        await _identityProvisioningStore.SaveChanges(CancellationToken.None);
                        transactionScope.Complete();
                    }

                    await _busControl.Publish(new ImportUsersSuccessEvent
                    {
                        NbUsers = nbImport,
                        Realm = realm
                    });
                    activity?.SetStatus(ActivityStatusCode.Ok, $"{nbImport} users has been imported");
                    importSummary.Success(nbImport);
                    await _importSummaryStore.SaveChanges(CancellationToken.None);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    importSummary.Fail(ex.ToString());
                    await _importSummaryStore.SaveChanges(CancellationToken.None);
                }
            }
        }

        private async Task<int> Import(string name, string realm)
        {
            int result = 0;
            _logger.LogInformation($"Start import {name}");
            var identityProvisioningLst = await _identityProvisioningStore.Query()
                .Include(i => i.Definition).ThenInclude(i => i.MappingRules)
                .Include(i => i.Realms)
                .Include(i => i.Histories)
                .Where(i => i.Definition.Name == name && i.Realms.Any(r => r.Name == realm))
                .ToListAsync();
            foreach (var identityProvisioning in identityProvisioningLst)
            {
                foreach (var history in identityProvisioning.Histories.Where(h => h.Status == IdentityProvisioningHistoryStatus.EXPORT).OrderBy(h => h.StartDateTime))
                {
                    var path = Path.Combine(_options.ExtractRepresentationsFolder, name, history.FolderName);
                    foreach (var filePath in Directory.GetFiles(path))
                         await Export(filePath, identityProvisioning.Definition, realm);
                    history.Import();
                    result += history.NbRepresentations;
                }
            }

            _logger.LogInformation($"Finish import {name}");
            return result;
        }

        private async Task Export(string filePath, IdentityProvisioningDefinition idProvisioningDef, string realm)
        {
            var userClaims = new List<UserClaim>();
            var users = new List<User>();
            using(var fs = File.OpenText(filePath))
            {
                var columns = fs.ReadLine().Split(Constants.SEPARATOR);
                var line = string.Empty;
                while((line = fs.ReadLine()) != null)
                {
                    var values = line.Split(Constants.SEPARATOR);
                    var extractionResult = ExtractUsersAndClaims(idProvisioningDef, columns, values);
                    userClaims.AddRange(extractionResult.UserClaims);
                    users.Add(extractionResult.User);
                }
            }

            await _userRepository.BulkUpdate(users);
            await _userRepository.BulkUpdate(userClaims);
            await _userRepository.BulkUpdate(users.Select(u => new RealmUser
            {
                UsersId = u.Id,
                RealmsName = realm
            }).ToList());
        }

        private ExtractionResult ExtractUsersAndClaims(IdentityProvisioningDefinition idProvisioningDef, IEnumerable<string> columns, IEnumerable<string> values)
        {
            var updatedProperties = new List<string>();
            var visibleAttributes = typeof(User).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p =>
                {
                    var attr = p.GetCustomAttribute<UserPropertyAttribute>();
                    return attr == null ? false : attr.IsVisible;
                });
            var userClaims = new List<UserClaim>();
            var externalRepresentationId = values.First();
            var user = new User
            {
                Id = externalRepresentationId,
                Source = idProvisioningDef.Name,
                UpdateDateTime = DateTime.UtcNow
            };
            int index = 2;
            foreach(var column in columns.Skip(2))
            {
                var mappingRule = idProvisioningDef.MappingRules.Single(r => r.Id == column);
                var value = values.ElementAt(index);
                switch(mappingRule.MapperType)
                {
                    case MappingRuleTypes.USERATTRIBUTE:
                        userClaims.Add(new UserClaim
                        {
                            Id = $"{externalRepresentationId}_{mappingRule.TargetUserAttribute}",
                            UserId = externalRepresentationId,
                            Value = value,
                            Name = mappingRule.TargetUserAttribute
                        });
                        break;
                    case MappingRuleTypes.SUBJECT:
                        user.Name = value;
                        break;
                    case MappingRuleTypes.USERPROPERTY:
                        var visibleAttribute = visibleAttributes.SingleOrDefault(a => a.Name == mappingRule.TargetUserProperty);
                        if (visibleAttribute != null)
                            visibleAttribute.SetValue(user, value);
                        break;
                }

                index++;
            }

            return new ExtractionResult { UserClaims = userClaims, User = user };
        }

        private class ExtractionResult
        {
            public List<UserClaim> UserClaims { get; set; }
            public User User { get; set; }
        }
    }
}
