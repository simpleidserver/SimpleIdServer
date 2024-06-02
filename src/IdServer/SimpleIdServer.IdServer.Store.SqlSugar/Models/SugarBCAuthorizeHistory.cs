// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("BCAuthorizeHistory")]
public class SugarBCAuthorizeHistory
{
    [SugarColumn(IsPrimaryKey = true)]
    public int Id { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public string? Message { get; set; } = null;
    public BCAuthorizeStatus Status { get; set; }
    public string BCAuthorizeId { get; set; }

    public BCAuthorizeHistory ToDomain()
    {
        return new BCAuthorizeHistory
        {
            StartDateTime = StartDateTime,
            EndDateTime = EndDateTime,
            Message = Message,
            Status = Status
        };
    }
}