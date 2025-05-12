// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains;

public class MigrationExecution
{
    [JsonPropertyName(MigrationExecutionNames.Id)]
    public string Id
    {
        get; set;
    }

    [JsonPropertyName(MigrationExecutionNames.Name)]
    public string Name
    {
        get; set;
    }

    [JsonPropertyName(MigrationExecutionNames.Histories)]
    public List<MigrationExecutionHistory> Histories
    {
        get; set;
    }

    [JsonIgnore]
    public string Realm
    {
        get; set;
    }

    public bool IsApiScopesMigrated
    {
        get
        {
            return IsMigrated(MigrationExecutionHistoryTypes.APISCOPES);
        }
    }

    public bool IsIdentityScopesMigrated
    {
        get
        {
            return IsMigrated(MigrationExecutionHistoryTypes.IDENTITYSCOPES);
        }
    }

    public bool IsApiResoucesMigrated
    {
        get
        {
            return IsMigrated(MigrationExecutionHistoryTypes.APIRESOURCES);
        }
    }

    public bool IsClientsMigrated
    {
        get
        {
            return IsMigrated(MigrationExecutionHistoryTypes.CLIENTS);
        }
    }

    public bool IsGroupsMigrated
    {
        get
        {
            return IsMigrated(MigrationExecutionHistoryTypes.GROUPS);
        }
    }

    public bool IsUsersMigrated
    {
        get
        {
            return IsMigrated(MigrationExecutionHistoryTypes.USERS);
        }
    }

    public void MigrateApiScopes(DateTime startDatetime, DateTime endDatetime, int nbScopes)
    {
        Migrate(MigrationExecutionHistoryTypes.APISCOPES, startDatetime, endDatetime, nbScopes);
    }

    public void MigrateIdentityScopes(DateTime startDatetime, DateTime endDatetime, int nbScopes)
    {
        Migrate(MigrationExecutionHistoryTypes.IDENTITYSCOPES, startDatetime, endDatetime, nbScopes);
    }

    public void MigrateApiResources(DateTime startDatetime, DateTime endDatetime, int nbScopes)
    {
        Migrate(MigrationExecutionHistoryTypes.APIRESOURCES, startDatetime, endDatetime, nbScopes);
    }

    public void MigrateClients(DateTime startDatetime, DateTime endDatetime, int nbClients)
    {
        Migrate(MigrationExecutionHistoryTypes.CLIENTS, startDatetime, endDatetime, nbClients);
    }

    public void MigrateGroups(DateTime startDatetime, DateTime endDatetime, int nbGroups)
    {
        Migrate(MigrationExecutionHistoryTypes.GROUPS, startDatetime, endDatetime, nbGroups);
    }

    public void MigrateUsers(DateTime startDateTime, DateTime endDateTime, int nbUsers)
    {
        Migrate(MigrationExecutionHistoryTypes.USERS, startDateTime, endDateTime, nbUsers);
    }

    private bool IsMigrated(MigrationExecutionHistoryTypes type)
    {
        return Histories != null && Histories.Any() && Histories.Any(h => h.Type == type);
    }

    private void Migrate(MigrationExecutionHistoryTypes type, DateTime startDatetime, DateTime endDatetime, int nbGroups)
    {
        Histories.Add(new MigrationExecutionHistory
        {
            StartDatetime = startDatetime,
            EndDatetime = endDatetime,
            NbRecords = nbGroups,
            Type = type
        });
    }
}
