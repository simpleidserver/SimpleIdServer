// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.Domains;

public class IdentityProvisioning
{
    public string Id { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public IdentityProvisioningDefinition Definition { get; set; } = null!;
    public ICollection<IdentityProvisioningHistory> Histories { get; set; } = new List<IdentityProvisioningHistory>();
    public ICollection<IdentityProvisioningProcess> Processes
    {
        get
        {
            var grpLst = Histories.GroupBy(h => h.ProcessId);
            var result = new List<IdentityProvisioningProcess>();
            foreach(var grp in grpLst)
            {
                var record = new IdentityProvisioningProcess();
                record.Id = grp.Key;
                foreach (var row in grp)
                    record.Consume(row);
            }

            return result;
        }
    }
    public ICollection<Realm> Realms { get; set; } = new List<Realm>();
    public ICollection<User> Users { get; set; } = new List<User>();

    public void Start(string processId, int totalPages)
    {
        Histories.Add(new IdentityProvisioningHistory
        {
            ProcessId = processId,
            Status = IdentityProvisioningHistoryStatus.START,
            ExecutionDateTime = DateTime.UtcNow,
            TotalPages = totalPages
        });
    }

    public void Extract(string processId, int currentPage, int nbUsers, int nbGroups)
    {
        Histories.Add(new IdentityProvisioningHistory
        {
            ProcessId = processId,
            Status = IdentityProvisioningHistoryStatus.EXPORT,
            ExecutionDateTime = DateTime.UtcNow,
            CurrentPage = currentPage,
            NbUsers = nbUsers,
            NbGroups = nbGroups
        });
    }

    public void FinishExtract(string processId)
    {
        Histories.Add(new IdentityProvisioningHistory
        {
            ProcessId = processId,
            Status = IdentityProvisioningHistoryStatus.EXPORT,
            ExecutionDateTime = DateTime.UtcNow
        });
    }

    public void Import(string processId, int totalPage)
    {
        Histories.Add(new IdentityProvisioningHistory
        {
            ProcessId = processId,
            Status = IdentityProvisioningHistoryStatus.IMPORT,
            ExecutionDateTime = DateTime.UtcNow,
            TotalPages = totalPage
        });
    }

    public void Import(string processId, int nbUsers, int nbGroups, int currentPage)
    {
        Histories.Add(new IdentityProvisioningHistory
        {
            ProcessId = processId,
            Status = IdentityProvisioningHistoryStatus.IMPORT,
            ExecutionDateTime = DateTime.UtcNow,
            CurrentPage = currentPage,
            NbUsers = nbUsers,
            NbGroups = nbGroups
        });
    }

    public void FinishImport(string processId)
    {
        Histories.Add(new IdentityProvisioningHistory
        {
            ProcessId = processId,
            Status = IdentityProvisioningHistoryStatus.FINISHIMPORT,
            ExecutionDateTime = DateTime.UtcNow
        });
    }

    public IdentityProvisioningProcess GetProcess(string processId)
    {
        var result = new IdentityProvisioningProcess
        {
            Id = processId
        };
        foreach(var history in Histories)
        {
            result.Consume(history);
        }

        return result;
    }
}

public class IdentityProvisioningProcess
{
    public string Id { get; set; }
    public int NbExtractedPages { get; set; }
    public int NbImportedPages { get; set; }
    public int TotalPageToExtract { get; set; }
    public int TotalPageToImport { get; set; }

    public bool IsExported
    {
        get
        {
            return NbExtractedPages == TotalPageToExtract;
        }
    }

    public bool IsImported
    {
        get
        {
            return NbImportedPages == TotalPageToImport;
        }
    }

    public void Consume(IdentityProvisioningHistory history)
    {
        switch (history.Status)
        {
            case IdentityProvisioningHistoryStatus.START:
                Id = history.ProcessId;
                TotalPageToExtract = history.TotalPages;
                break;
            case IdentityProvisioningHistoryStatus.EXPORT:
                NbExtractedPages++;
                break;
            case IdentityProvisioningHistoryStatus.STARTIMPORT:
                TotalPageToImport = history.TotalPages;
                break;
            case IdentityProvisioningHistoryStatus.IMPORT:
                NbImportedPages++;
                break;
        }
    }
}