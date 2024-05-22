// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("Users")]
public class SugarUser
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Firstname { get; set; } = null;
    public string? Lastname { get; set; } = null;
    public string? Email { get; set; } = null;
    public bool EmailVerified { get; set; } = false;
    public string? DeviceRegistrationToken { get; set; }
    public UserStatus Status { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public string? Source { get; set; } = null;
    public string? IdentityProvisioningId { get; set; } = null;
    public string? EncodedPicture { get; set; } = null;
    public string NotificationMode { get; set; } = "console";

    [Navigate(NavigateType.OneToMany, nameof(SugarUserSession.UserId))]
    public List<SugarUserSession> Sessions { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarConsent.UserId))]
    public List<SugarConsent> Consents { get; set; }

    public User ToDomain()
    {
        return new User
        {

        };
    }
}
