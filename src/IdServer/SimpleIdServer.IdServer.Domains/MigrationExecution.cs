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

    public bool IsGroupsMigrated
    {
        get
        {
            return Histories != null && Histories.Any() && Histories.Any(h => h.Type == MigrationExecutionHistoryTypes.GROUPS);
        }
    }

    public void MigrateGroups(DateTime startDatetime, DateTime endDatetime, int nbGroups)
    {
        Histories.Add(new MigrationExecutionHistory
        {
            StartDatetime = startDatetime,
            EndDatetime = endDatetime,
            NbRecords = nbGroups,
            Type = MigrationExecutionHistoryTypes.GROUPS
        });
    }
}
