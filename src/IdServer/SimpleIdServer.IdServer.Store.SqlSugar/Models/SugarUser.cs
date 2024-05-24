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
    public string NotificationMode { get; set; }
    public IdentityProvisioning IdentityProvisioning { get; set; }

    [Navigate(NavigateType.OneToMany, nameof(SugarUserSession.UserId))]
    public List<SugarUserSession> Sessions { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarUserClaim.UserId))]
    public List<SugarUserClaim> Claims { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarUserCredential.User))]
    public List<SugarUserCredential> Credentials { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarUserExternalAuthProvider.UserId))]
    public List<SugarUserExternalAuthProvider> ExternalAuthProviders { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarConsent.UserId))]
    public List<SugarConsent> Consents { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarUserDevice.UserId))]
    public List<SugarUserDevice> Devices { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarGroupUser.UsersId))]
    public List<SugarGroupUser> Groups { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarRealmUser.UsersId))]
    public List<SugarRealmUser> Realms { get; set; }

    public User ToDomain()
    {
        return new User
        {
            Id = Id,
            Name = Name,
            CreateDateTime = CreateDateTime,
            DeviceRegistrationToken = DeviceRegistrationToken,
            EncodedPicture = EncodedPicture,
            IdentityProvisioningId = IdentityProvisioningId,
            Source = Source,
            Email = Email,
            EmailVerified = EmailVerified,
            Firstname = Firstname,
            Status = Status,
            UpdateDateTime = UpdateDateTime,
            NotificationMode = NotificationMode,
            Lastname = Lastname,
            Sessions = Sessions.Select(s => s.ToDomain()).ToList(),
            OAuthUserClaims = Claims.Select(c => c.ToDomain()).ToList(),
            Credentials = Credentials.Select(c => c.ToDomain()).ToList(),
            ExternalAuthProviders = ExternalAuthProviders.Select(e => e.ToDomain()).ToList(),
            Consents = Consents.Select(c => c.ToDomain()).ToList(),
            Groups = Groups.Select(c => c.ToDomain()).ToList(),
            Realms = Realms.Select(r => r.ToDomain()).ToList()
        };
    }
}
