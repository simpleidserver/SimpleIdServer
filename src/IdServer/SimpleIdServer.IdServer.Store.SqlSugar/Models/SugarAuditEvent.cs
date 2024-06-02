// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("AuditEvents")]
public class SugarAuditEvent
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "Id")]
    public string Id { get; set; } = null!;
    public string EventName { get; set; } = null!;
    public string Realm { get; set; } = null!;
    public bool IsError { get; set; }
    public string Description { get; set; } = null!;
    public string? ErrorMessage { get; set; } = null;
    public DateTime CreateDateTime { get; set; }
    public string? ClientId { get; set; } = null;
    public string? UserName { get; set; } = null;
    public string? RequestJSON { get; set; } = null;
    public string? RedirectUrl { get; set; } = null;
    public string? AuthMethod { get; set; } = null;
    public string Scopes { get; set; } = null!;
    public string Claims { get; set; } = null!;

    public AuditEvent ToDomain()
    {
        return new AuditEvent
        {
            AuthMethod = AuthMethod,
            ClientId = ClientId,
            CreateDateTime = CreateDateTime,
            Description = Description,
            ErrorMessage = ErrorMessage,
            EventName = EventName,
            Id = Id,
            IsError = IsError,
            Realm = Realm,
            RedirectUrl = RedirectUrl,
            RequestJSON = RequestJSON,
            UserName = UserName,
            Scopes = Scopes.Split(","),
            Claims = Claims.Split(','),
        };
    }
}
