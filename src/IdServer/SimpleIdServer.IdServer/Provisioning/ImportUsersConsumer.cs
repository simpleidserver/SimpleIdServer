﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Transactions;

namespace SimpleIdServer.IdServer.Provisioning;

public class ImportUsersConsumer : 
    IConsumer<StartImportUsersCommand>,
    IConsumer<ImportUsersCommand>,
    IConsumer<CheckUsersImportedCommand>
{
    private int _pageSize = 1000;
    private TimeSpan _checkInterval = TimeSpan.FromSeconds(15);
    private readonly IIdentityProvisioningStore _identityProvisioningStore;
    private readonly IProvisioningStagingStore _provisioningStagingStore;
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;

    public ImportUsersConsumer(
        IIdentityProvisioningStore identityProvisioningStore,
        IProvisioningStagingStore provisioningStagingStore,
        IUserRepository userRepository,
        IGroupRepository groupRepository)
    {
        _identityProvisioningStore = identityProvisioningStore;
        _provisioningStagingStore = provisioningStagingStore;
        _userRepository = userRepository;
        _groupRepository = groupRepository;

    }

    public async Task Consume(ConsumeContext<StartImportUsersCommand> context)
    {
        var message = context.Message;
        var idProvisioning = await _identityProvisioningStore.Query()
            .Include(i => i.Definition).ThenInclude(i => i.MappingRules)
            .Include(i => i.Realms)
            .Include(i => i.Histories)
            .SingleAsync(i => i.Id == message.InstanceId && i.Realms.Any(r => r.Name == message.Realm), context.CancellationToken);
        var nbRecords = await _provisioningStagingStore.NbStagingExtractedRepresentations(context.Message.ProcessId, context.CancellationToken);
        var nbPages = ((int)Math.Ceiling((double)nbRecords / _pageSize));
        var allPages = Enumerable.Range(1, nbPages);
        foreach(var page in allPages)
        {
            await context.Send(new ImportUsersCommand
            {
                InstanceId = message.InstanceId,
                Page = page,
                PageSize = _pageSize,
                ProcessId = message.ProcessId,
                Realm = message.Realm
            });
        }

        await context.ScheduleSend(_checkInterval, new CheckUsersImportedCommand
        {
            InstanceId = message.InstanceId,
            ProcessId = message.ProcessId,
            Realm = message.Realm
        });
        idProvisioning.Import(message.ProcessId, allPages.Last());
        await _identityProvisioningStore.SaveChanges(context.CancellationToken);
    }

    public async Task Consume(ConsumeContext<ImportUsersCommand> context)
    {
        var message = context.Message;
        var idProvisioning = await _identityProvisioningStore.Query()
            .Include(i => i.Definition).ThenInclude(i => i.MappingRules)
            .Include(i => i.Realms)
            .Include(i => i.Histories)
            .SingleAsync(i => i.Id == message.InstanceId && i.Realms.Any(r => r.Name == message.Realm), context.CancellationToken);
        var representations = await _provisioningStagingStore.GetStagingExtractedRepresentations(
            message.ProcessId,
            message.Page - 1, 
            message.PageSize, 
            context.CancellationToken);
        var groupRepresentations = representations.Where(r => r.Type == Domains.ExtractedRepresentationType.GROUP);
        var userRepresentations = representations.Where(r => r.Type == Domains.ExtractedRepresentationType.USER);
        using (var transactionScope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.Snapshot }, TransactionScopeAsyncFlowOption.Enabled))
        {
            await Export(userRepresentations, groupRepresentations, idProvisioning, message.Realm);
            idProvisioning.Import(message.ProcessId, userRepresentations.Count(), groupRepresentations.Count(), message.Page);
            await _identityProvisioningStore.SaveChanges(context.CancellationToken);
            transactionScope.Complete();
        }
    }

    public async Task Consume(ConsumeContext<CheckUsersImportedCommand> context)
    {
        var message = context.Message;
        var instance = await _identityProvisioningStore.Query()
            .Include(d => d.Realms)
            .Include(d => d.Histories)
            .Include(d => d.Definition).ThenInclude(d => d.MappingRules)
            .SingleAsync(i => i.Id == message.InstanceId && i.Realms.Any(r => r.Name == message.Realm));
        var process = instance.GetProcess(message.ProcessId);
        if (process.IsImported)
        {
            instance.FinishImport(message.ProcessId);
            await _identityProvisioningStore.SaveChanges(context.CancellationToken);
            return;
        }

        await context.ScheduleSend(_checkInterval, new CheckUsersImportedCommand
        {
            InstanceId = message.InstanceId,
            ProcessId = message.ProcessId,
            Realm = message.Realm
        });
    }

    private async Task Export(IEnumerable<ExtractedRepresentationStaging> extractedUsers, IEnumerable<ExtractedRepresentationStaging> extractedGroups, IdentityProvisioning idProvisioning, string realm)
    {
        var userClaims = new List<UserClaim>();
        var users = new List<User>();
        var groups = new List<Group>();
        foreach(var extractedGroup in extractedGroups)
        {
            groups.Add(ExtractGroup(idProvisioning, extractedGroup));
        }

        foreach(var extractedUser in extractedUsers)
        {
            var extraction = ExtractUsersAndClaims(idProvisioning,  extractedUser);
            users.Add(extraction.User);
            userClaims.AddRange(extraction.UserClaims);
        }

        await _groupRepository.BulkUpdate(groups);
        await _userRepository.BulkUpdate(users);
        await _userRepository.BulkUpdate(userClaims);
        await _userRepository.BulkUpdate(users.Select(u => new RealmUser
        {
            UsersId = u.Id,
            RealmsName = realm
        }).ToList());
    }

    private Group ExtractGroup(IdentityProvisioning idProvisioning, ExtractedRepresentationStaging extractedGroup)
    {
        var result = new Group
        {
            Id = extractedGroup.RepresentationId,
            CreateDateTime = DateTime.UtcNow,
            UpdateDateTime = DateTime.UtcNow
        };
        if (!string.IsNullOrWhiteSpace(extractedGroup.Values))
        {
            var mappingRules = idProvisioning.Definition.MappingRules.Where(r => r.Usage == IdentityProvisioningMappingUsage.GROUP);
            var jObj = JsonObject.Parse(extractedGroup.Values).AsObject();
            foreach(var kvp in jObj)
            {
                var mappingRule = mappingRules.Single(r => r.Id == kvp.Key);
                switch(mappingRule.MapperType)
                {
                    case MappingRuleTypes.GROUPNAME:
                        var value = jObj[kvp.Key].ToString();
                        result.Name = value;
                        break;
                }
            }
        }

        return result;
    }

    private ExtractionUserResult ExtractUsersAndClaims(IdentityProvisioning idProvisioning, ExtractedRepresentationStaging extractedUser)
    {
        var updatedProperties = new List<string>();
        var visibleAttributes = typeof(User).GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p =>
            {
                var attr = p.GetCustomAttribute<UserPropertyAttribute>();
                return attr == null ? false : attr.IsVisible;
            });
        var userClaims = new List<UserClaim>();
        var user = new User
        {
            Id = extractedUser.Id,
            Source = idProvisioning.Definition.Name,
            IdentityProvisioningId = idProvisioning.Id,
            UpdateDateTime = DateTime.UtcNow
        };
        if (!string.IsNullOrWhiteSpace(extractedUser.Values))
        {
            var mappingRules = idProvisioning.Definition.MappingRules.Where(r => r.Usage == IdentityProvisioningMappingUsage.USER);
            var jObj = JsonObject.Parse(extractedUser.Values).AsObject();
            foreach(var kvp in jObj)
            {
                var mappingRule = mappingRules.Single(r => r.Id == kvp.Key);
                var serializedValue = jObj[kvp.Key].ToJsonString();
                if (string.IsNullOrWhiteSpace(serializedValue)) continue;
                var extractedValues = ExtractValues(serializedValue);
                foreach (var value in extractedValues)
                {
                    switch (mappingRule.MapperType)
                    {
                        case MappingRuleTypes.USERATTRIBUTE:
                            var userClaimId = mappingRule.HasMultipleAttribute ? $"{user.Id}_{mappingRule.TargetUserAttribute}_{value}" : $"{user.Id}_{mappingRule.TargetUserAttribute}";
                            userClaims.Add(new UserClaim
                            {
                                Id = userClaimId,
                                UserId = user.Id,
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
                }
            }
        }

        return new ExtractionUserResult { UserClaims = userClaims, User = user };

        List<string> ExtractValues(string serializedValue)
        {
            try
            {
                var jArr = JsonArray.Parse(serializedValue) as JsonArray;
                return jArr.Select(r => r.AsValue().GetValue<string>()).ToList();
            }
            catch
            {
                return new List<string> { serializedValue };
            }
        }
    }

    private class ExtractionUserResult
    {
        public List<UserClaim> UserClaims { get; set; }
        public User User { get; set; }
    }
}