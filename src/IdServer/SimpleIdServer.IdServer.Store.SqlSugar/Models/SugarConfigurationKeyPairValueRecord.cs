// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Configuration.Models;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("ConfigurationKeyPairValueRecords")]
public class SugarConfigurationKeyPairValueRecord
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;

    public ConfigurationKeyPairValueRecord ToDomain()
    {
        return new ConfigurationKeyPairValueRecord
        {
            Name = Name,
            Value = Value
        };
    }
}