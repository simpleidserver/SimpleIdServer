// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("ExtractedRepresentationsStaging")]
public class SugarExtractedRepresentationStaging
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string RepresentationId { get; set; } = null!;
    public string RepresentationVersion { get; set; } = null!;
    [SugarColumn(IsNullable = true, Length = 5000)]
    public string? Values { get; set; } = null;
    public string IdProvisioningProcessId { get; set; } = null!;
    public string GroupIds { get; set; } = null!;
    public ExtractedRepresentationType Type { get; set; }

    public ExtractedRepresentationStaging ToDomain()
    {
        return new ExtractedRepresentationStaging
        {
            GroupIds = GroupIds.Split(',').ToList(),
            Id = Id,
            IdProvisioningProcessId = IdProvisioningProcessId,
            RepresentationId = RepresentationId,
            RepresentationVersion = RepresentationVersion,
            Type = Type,
            Values = Values
        };
    }
}
