// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("ExtractedRepresentations")]
public class SugarExtractedRepresentation
{
    [SugarColumn(IsPrimaryKey = true)]
    public string ExternalId { get; set; } = null!;
    public string Version { get; set; } = null!;

    public ExtractedRepresentation ToDomain()
    {
        return new ExtractedRepresentation
        {
            ExternalId = ExternalId,
            Version = Version,
        };
    }
}
