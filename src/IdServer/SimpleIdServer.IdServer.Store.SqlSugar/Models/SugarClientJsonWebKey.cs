// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("ClientJsonWebKey")]
public class SugarClientJsonWebKey
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Kid { get; set; } = null!;
    public string Alg { get; set; } = null!;
    public string Usage { get; set; } = null!;
    public SecurityKeyTypes? KeyType { get; set; } = null;
    public string SerializedJsonWebKey { get; set; } = null!;
    public string ClientId { get; set; } = null;

    public ClientJsonWebKey ToDomain()
    {
        return new ClientJsonWebKey
        {
            KeyType = KeyType,
            Alg = Alg,
            Kid = Kid,
            SerializedJsonWebKey = SerializedJsonWebKey,
            Usage = Usage
        };
    }
}