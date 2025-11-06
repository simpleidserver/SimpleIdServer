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
    public ICollection<IdentityProvisioningProcess> Processes { get; set; } = new List<IdentityProvisioningProcess>();
    public ICollection<Realm> Realms { get; set; } = new List<Realm>();
    public ICollection<User> Users { get; set; } = new List<User>();

    public string Launch()
    {
        var processId = Guid.NewGuid().ToString();
        var history = new IdentityProvisioningHistory
        {
            ProcessId = processId,
            Status = IdentityProvisioningHistoryStatus.CREATE,
            ExecutionDateTime = DateTime.UtcNow
        };
        Histories.Add(history);
        
        var process = new IdentityProvisioningProcess
        {
            Id = processId
        };
        process.Consume(history);
        Processes.Add(process);
        
        return processId;
    }

    public void Start(string processId, int totalPages)
    {
        var history = new IdentityProvisioningHistory
        {
            ProcessId = processId,
            Status = IdentityProvisioningHistoryStatus.START,
            ExecutionDateTime = DateTime.UtcNow,
            TotalPages = totalPages
        };
        Histories.Add(history);
        
        var process = Processes.FirstOrDefault(p => p.Id == processId);
        if (process != null)
            process.Consume(history);
    }

    public void Extract(string processId, int currentPage, int nbUsers, int nbGroups, int nbFilteredRepresentations)
    {
        var history = new IdentityProvisioningHistory
        {
            ProcessId = processId,
            Status = IdentityProvisioningHistoryStatus.EXPORT,
            ExecutionDateTime = DateTime.UtcNow,
            CurrentPage = currentPage,
            NbUsers = nbUsers,
            NbGroups = nbGroups,
            NbFilteredRepresentations = nbFilteredRepresentations
        };
        Histories.Add(history);
        
        var process = Processes.FirstOrDefault(p => p.Id == processId);
        if (process != null)
            process.Consume(history);
    }

    public void FinishExtract(string processId)
    {
        var history = new IdentityProvisioningHistory
        {
            ProcessId = processId,
            Status = IdentityProvisioningHistoryStatus.FINISHEXPORT,
            ExecutionDateTime = DateTime.UtcNow
        };
        Histories.Add(history);
        
        var process = Processes.FirstOrDefault(p => p.Id == processId);
        if (process != null)
            process.Consume(history);
    }

    public void Import(string processId, int totalPage)
    {
        var history = new IdentityProvisioningHistory
        {
            ProcessId = processId,
            Status = IdentityProvisioningHistoryStatus.STARTIMPORT,
            ExecutionDateTime = DateTime.UtcNow,
            TotalPages = totalPage
        };
        Histories.Add(history);
        
        var process = Processes.FirstOrDefault(p => p.Id == processId);
        if (process != null)
            process.Consume(history);
    }

    public void Import(string processId, int nbUsers, int nbGroups, int currentPage)
    {
        var history = new IdentityProvisioningHistory
        {
            ProcessId = processId,
            Status = IdentityProvisioningHistoryStatus.IMPORT,
            ExecutionDateTime = DateTime.UtcNow,
            CurrentPage = currentPage,
            NbUsers = nbUsers,
            NbGroups = nbGroups
        };
        Histories.Add(history);
        
        var process = Processes.FirstOrDefault(p => p.Id == processId);
        if (process != null)
            process.Consume(history);
    }

    public void FinishImport(string processId)
    {
        var history = new IdentityProvisioningHistory
        {
            ProcessId = processId,
            Status = IdentityProvisioningHistoryStatus.FINISHIMPORT,
            ExecutionDateTime = DateTime.UtcNow
        };
        Histories.Add(history);
        
        var process = Processes.FirstOrDefault(p => p.Id == processId);
        if (process != null)
            process.Consume(history);
    }

    public IdentityProvisioningProcess GetProcess(string processId)
    {
        return Processes.FirstOrDefault(p => p.Id == processId);
    }
    
    public void RefreshProjections()
    {
        var grpLst = Histories.GroupBy(h => h.ProcessId);
        foreach(var grp in grpLst)
        {
            var process = Processes.FirstOrDefault(p => p.Id == grp.Key);
            if (process == null)
            {
                process = new IdentityProvisioningProcess { Id = grp.Key };
                Processes.Add(process);
            }
            else
            {
                // Reset the process state before recalculating from histories
                process.CreateDateTime = DateTime.MinValue;
                process.StartExportDateTime = null;
                process.EndExportDateTime = null;
                process.StartImportDateTime = null;
                process.EndImportDateTime = null;
                process.NbExtractedPages = 0;
                process.NbExtractedUsers = 0;
                process.NbExtractedGroups = 0;
                process.NbFilteredRepresentations = 0;
                process.NbImportedPages = 0;
                process.NbImportedGroups = 0;
                process.NbImportedUsers = 0;
                process.TotalPageToExtract = 0;
                process.TotalPageToImport = 0;
            }
            
            foreach (var history in grp)
                process.Consume(history);
        }
    }
}

public class IdentityProvisioningProcess
{
    public string Id { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime? StartExportDateTime { get; set; }
    public DateTime? EndExportDateTime { get; set; }
    public DateTime? StartImportDateTime { get; set; }
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
    public IdentityProvisioning IdentityProvisioning { get; set; } = null!;

    public bool IsExported
    {
        get
        {
            return EndExportDateTime != null;
        }
    }

    public bool IsImported
    {
        get
        {
            return EndImportDateTime != null;
        }
    }

    public void Consume(IdentityProvisioningHistory history)
    {
        switch (history.Status)
        {
            case IdentityProvisioningHistoryStatus.CREATE:
                Id = history.ProcessId;
                CreateDateTime = history.ExecutionDateTime;
                break;
            case IdentityProvisioningHistoryStatus.START:
                StartExportDateTime = history.ExecutionDateTime;
                TotalPageToExtract = history.TotalPages;
                break;
            case IdentityProvisioningHistoryStatus.EXPORT:
                NbExtractedPages++;
                NbExtractedUsers += history.NbUsers;
                NbExtractedGroups += history.NbGroups;
                NbFilteredRepresentations += history.NbFilteredRepresentations;
                break;
            case IdentityProvisioningHistoryStatus.FINISHEXPORT:
                EndExportDateTime = history.ExecutionDateTime;
                break;
            case IdentityProvisioningHistoryStatus.STARTIMPORT:
                TotalPageToImport = history.TotalPages;
                StartImportDateTime = history.ExecutionDateTime;
                break;
            case IdentityProvisioningHistoryStatus.IMPORT:
                NbImportedPages++;
                NbImportedUsers += history.NbUsers;
                NbImportedGroups += history.NbGroups;
                break;
            case IdentityProvisioningHistoryStatus.FINISHIMPORT:
                EndImportDateTime = history.ExecutionDateTime;
                break;
        }
    }
}