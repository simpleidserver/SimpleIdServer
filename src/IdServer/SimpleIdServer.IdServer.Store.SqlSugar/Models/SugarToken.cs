// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("Tokens")]
public class SugarToken
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int PkID { get; set; }
    public string Id { get; set; } = null!;
    public string? SessionId { get; set; } = null;
    public string ClientId { get; set; } = null!;
    public string TokenType { get; set; } = null!;
    public AccessTokenTypes? AccessTokenType { get; set; } = null;
    public string? Data { get; set; } = null;
    public string? OriginalData { get; set; } = null;
    public string? AuthorizationCode { get; set; } = null;
    public string? GrantId { get; set; } = null;
    public string? Jkt { get; set; } = null;
    public DateTime? ExpirationTime { get; set; }
    public DateTime CreateDateTime { get; set; }

    public Token ToDomain()
    {
        return new Token
        {
            AccessTokenType = AccessTokenType,
            AuthorizationCode = AuthorizationCode,
            ClientId = ClientId,
            CreateDateTime = CreateDateTime,
            Data = Data,
            ExpirationTime = ExpirationTime,
            GrantId = GrantId,
            Id = Id,
            Jkt = Jkt,
            OriginalData = OriginalData,
            SessionId = SessionId,
            TokenType = TokenType
        };
    }
}
