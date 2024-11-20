﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("UmaPendingRequest")]
public class SugarUMAPendingRequest
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    public string TicketId { get; set; } = null!;
    [SugarColumn(IsNullable = true)]
    public string? Requester { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? Owner { get; set; } = null!;
    public string Scopes { get; set; } = null!;
    public DateTime CreateDateTime { get; set; }
    public UMAPendingRequestStatus Status { get; set; }
    public string Realm { get; set; }
    public string ResourceId { get; set; } = null!;
    [Navigate(NavigateType.ManyToOne, nameof(ResourceId))]
    public SugarUMAResource Resource { get; set; }

    public UMAPendingRequest ToDomain()
    {
        return new UMAPendingRequest
        {
            TicketId = TicketId,
            Requester = Requester,
            Owner = Owner,
            CreateDateTime = CreateDateTime,
            Status = Status,
            Realm = Realm,
            Resource = Resource?.ToDomain(),
            Scopes = Scopes.Split(',').ToList()
        };
    }
}
