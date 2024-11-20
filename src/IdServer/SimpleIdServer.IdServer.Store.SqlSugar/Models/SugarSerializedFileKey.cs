// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("SerializedFileKeys")]
public class SugarSerializedFileKey
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string KeyId { get; set; } = null!;
    public string Usage { get; set; } = null!;
    public string Alg { get; set; } = null!;
    [SugarColumn(IsNullable = true)]
    public string? Enc { get; set; }
    [SugarColumn(IsNullable = true, Length = 5000)]
    public string? PublicKeyPem { get; set; } = null;
    [SugarColumn(IsNullable = true, Length = 5000)]
    public string? PrivateKeyPem { get; set; } = null;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public bool IsSymmetric { get; set; } = false;
    public byte[] Key { get; set; } = new byte[0];
    [Navigate(typeof(SugarRealmSerializedFileKey), nameof(SugarRealmSerializedFileKey.SerializedFileKeysId), nameof(SugarRealmSerializedFileKey.RealmsName))]
    public List<SugarRealm> Realms { get; set; }

    public SerializedFileKey ToDomain()
    {
        return new SerializedFileKey
        {
            Id = Id,
            Alg = Alg,
            CreateDateTime = CreateDateTime,
            Enc = Enc,
            IsSymmetric = IsSymmetric,
            KeyId = KeyId,
            Key = Key,
            PrivateKeyPem = PrivateKeyPem,
            PublicKeyPem = PublicKeyPem,
            UpdateDateTime = UpdateDateTime,
            Usage = Usage,
            Realms = Realms == null ? new List<Realm>(): Realms.Select(r => r.ToDomain()).ToList()
        };
    }
}
