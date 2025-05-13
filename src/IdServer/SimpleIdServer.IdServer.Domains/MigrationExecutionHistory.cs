// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains;

public class MigrationExecutionHistory
{
    public string Id
    {
        get; set;
    }

    [JsonPropertyName(MigrationExecutionHistoryNames.StartDatetime)]
    public DateTime StartDatetime
    {
        get; set;
    }

    [JsonPropertyName(MigrationExecutionHistoryNames.EndDatetime)]
    public DateTime? EndDatetime
    {
        get; set;
    }

    [JsonPropertyName(MigrationExecutionHistoryNames.Type)]
    public MigrationExecutionHistoryTypes Type
    {
        get; set;
    }

    [JsonPropertyName(MigrationExecutionHistoryNames.NbRecords)]
    public int NbRecords
    {
        get; set;
    }

    [JsonPropertyName(MigrationExecutionHistoryNames.Errors)]
    public List<string> Errors
    {
        get; set;
    } = new List<string>();
}

public enum MigrationExecutionHistoryTypes
{
    APISCOPES = 0,
    IDENTITYSCOPES = 1,
    APIRESOURCES = 2,
    CLIENTS = 3,
    GROUPS = 4,
    USERS = 5
}