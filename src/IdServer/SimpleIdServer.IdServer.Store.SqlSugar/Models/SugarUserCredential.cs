// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("UserCredential")]
public class SugarUserCredential
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string CredentialType { get; set; } = null!;
    public string Value { get; set; } = null!;
    [SugarColumn(IsNullable = true)]
    public OTPAlgs? OTPAlg { get; set; } = null;
    public bool IsActive { get; set; }
    public int OTPCounter { get; set; }
    public int TOTPStep { get; set; }
    public int HOTPWindow { get; set; }
    public string UserId { get; set; } = null!;
    [Navigate(NavigateType.ManyToOne, nameof(UserId))]
    public SugarUser User { get; set; }

    public static SugarUserCredential Transform(UserCredential c)
    {
        return new SugarUserCredential
        {
            Id = c.Id,
            CredentialType = c.CredentialType,
            HOTPWindow = c.HOTPWindow,
            IsActive = c.IsActive,
            OTPAlg = c.OTPAlg,
            OTPCounter = c.OTPCounter,
            TOTPStep = c.TOTPStep,
            Value = c.Value
        };
    }

    public UserCredential ToDomain()
    {
        return new UserCredential
        {
            CredentialType = CredentialType,
            HOTPWindow = HOTPWindow,
            Id = Id,
            IsActive = IsActive,
            OTPAlg = OTPAlg,
            OTPCounter = OTPCounter,
            TOTPStep = TOTPStep,
            Value = Value
        };
    }
}
