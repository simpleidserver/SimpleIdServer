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
    } = new List<MigrationExecutionHistory>();

    [JsonIgnore]
    public string Realm
    {
        get; set;
    }

    [JsonIgnore]
    public MigrationExecutionHistory? LastExecutionHistory
    {
        get
        {
            return Histories.OrderByDescending(h => h.StartDatetime).FirstOrDefault();
        }
    }

    public int TotalMigratedUsers
    {
        get
        {
            return NbMigrated(MigrationExecutionHistoryTypes.USERS);
        }
    }

    public int TotalMigratedGroups
    {
        get
        {
            return NbMigrated(MigrationExecutionHistoryTypes.GROUPS);
        }
    }

    public int TotalMigratedScopes
    {
        get
        {
            return NbMigrated(MigrationExecutionHistoryTypes.APISCOPES, MigrationExecutionHistoryTypes.IDENTITYSCOPES);
        }
    }

    public int TotalMigratedClients
    {
        get
        {
            return NbMigrated(MigrationExecutionHistoryTypes.CLIENTS);
        }
    }

    public int TotalMigratedApiResources
    {
        get
        {
            return NbMigrated(MigrationExecutionHistoryTypes.APIRESOURCES);
        }
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

    public void LogErrors(MigrationExecutionHistoryTypes type, List<string> errorMessages)
    {
        var history = Histories.SingleOrDefault(h => h.Type == type);
        if (history == null)
        {
            history = new MigrationExecutionHistory
            {
                Id = Guid.NewGuid().ToString(),
                Type = type,
                StartDatetime = DateTime.UtcNow
            };
            Histories.Add(history);
        }

        history.Errors = errorMessages;
    }

    public void MigrateUsers(DateTime startDateTime, DateTime endDateTime, int nbUsers)
    {
        Migrate(MigrationExecutionHistoryTypes.USERS, startDateTime, endDateTime, nbUsers);
    }

    public List<string> GetErrors(params MigrationExecutionHistoryTypes[] types)
    {
        if(Histories == null || !Histories.Any()) return new List<string>();
        var filtered = Histories.Where(h => types.Contains(h.Type) && h.EndDatetime == null);
        return filtered.SelectMany(h => h.Errors).ToList();
    }

    public bool IsMigrated(params MigrationExecutionHistoryTypes[] types)
    {
        return Histories != null && Histories.Any() && types.All(t => Histories.Any(h => h.Type == t && h.EndDatetime != null));
    }

    private void Migrate(MigrationExecutionHistoryTypes type, DateTime startDatetime, DateTime endDatetime, int nbRecords)
    {
        var history = Histories.SingleOrDefault(h => h.Type == type);
        if (history == null)
        {
            history = new MigrationExecutionHistory
            {
                Id = Guid.NewGuid().ToString(),
                Type = type
            };
            Histories.Add(history);
        }

        history.StartDatetime = startDatetime;
        history.EndDatetime = endDatetime;
        history.NbRecords = nbRecords;
    }

    private int NbMigrated(params MigrationExecutionHistoryTypes[] types)
    {
        if (Histories == null || !Histories.Any()) return 0;
        var history = Histories.FirstOrDefault(h => types.Contains(h.Type));
        if (history == null) return 0;
        return history.NbRecords;
    }
}
