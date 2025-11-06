// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("IdentityProvisioningProcesses")]
public class SugarIdentityProvisioningProcess
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; }
    public string IdentityProvisioningId { get; set; } = null!;
    public DateTime CreateDateTime { get; set; }
    [SugarColumn(IsNullable = true)]
    public DateTime? StartExportDateTime { get; set; }
    [SugarColumn(IsNullable = true)]
    public DateTime? EndExportDateTime { get; set; }
    [SugarColumn(IsNullable = true)]
    public DateTime? StartImportDateTime { get; set; }
    [SugarColumn(IsNullable = true)]
    public DateTime? EndImportDateTime { get; set; }
    public int NbExtractedPages { get; set; }
    public int NbExtractedUsers { get; set; }
    public int NbExtractedGroups { get; set; }
    public int NbFilteredRepresentations { get; set; }
    public int NbImportedPages { get; set; }
    public int NbImportedGroups { get; set; }
    public int NbImportedUsers { get; set; }
    public int TotalPageToExtract { get; set; }
    public int TotalPageToImport { get; set; }

    public static SugarIdentityProvisioningProcess Transform(IdentityProvisioningProcess process)
    {
        return new SugarIdentityProvisioningProcess
        {
            Id = process.Id,
            CreateDateTime = process.CreateDateTime,
            StartExportDateTime = process.StartExportDateTime,
            EndExportDateTime = process.EndExportDateTime,
            StartImportDateTime = process.StartImportDateTime,
            EndImportDateTime = process.EndImportDateTime,
            NbExtractedPages = process.NbExtractedPages,
            NbExtractedUsers = process.NbExtractedUsers,
            NbExtractedGroups = process.NbExtractedGroups,
            NbFilteredRepresentations = process.NbFilteredRepresentations,
            NbImportedPages = process.NbImportedPages,
            NbImportedGroups = process.NbImportedGroups,
            NbImportedUsers = process.NbImportedUsers,
            TotalPageToExtract = process.TotalPageToExtract,
            TotalPageToImport = process.TotalPageToImport
        };
    }

    public IdentityProvisioningProcess ToDomain()
    {
        return new IdentityProvisioningProcess
        {
            Id = Id,
            CreateDateTime = CreateDateTime,
            StartExportDateTime = StartExportDateTime,
            EndExportDateTime = EndExportDateTime,
            StartImportDateTime = StartImportDateTime,
            EndImportDateTime = EndImportDateTime,
            NbExtractedPages = NbExtractedPages,
            NbExtractedUsers = NbExtractedUsers,
            NbExtractedGroups = NbExtractedGroups,
            NbFilteredRepresentations = NbFilteredRepresentations,
            NbImportedPages = NbImportedPages,
            NbImportedGroups = NbImportedGroups,
            NbImportedUsers = NbImportedUsers,
            TotalPageToExtract = TotalPageToExtract,
            TotalPageToImport = TotalPageToImport
        };
    }
}
