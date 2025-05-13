// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Migrations.Api;

public class MigrationDefinitionResult
{
    [JsonPropertyName(MigrationDefinitionResultNames.Name)]
    public string Name
    {
        get; set;
    }
}
