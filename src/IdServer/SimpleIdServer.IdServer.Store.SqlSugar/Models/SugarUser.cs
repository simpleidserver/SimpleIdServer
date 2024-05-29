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
    [Navigate(NavigateType.ManyToOne, nameof(IdentityProvisioningId))]
    public SugarIdentityProvisioning IdentityProvisioning { get; set; }

    [Navigate(NavigateType.OneToMany, nameof(SugarUserSession.UserId))]
    public List<SugarUserSession> Sessions { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarUserClaim.UserId))]
    public List<SugarUserClaim> Claims { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarUserCredential.UserId))]
    public List<SugarUserCredential> Credentials { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarUserExternalAuthProvider.UserId))]
    public List<SugarUserExternalAuthProvider> ExternalAuthProviders { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarConsent.UserId))]
    public List<SugarConsent> Consents { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarUserDevice.UserId))]
    public List<SugarUserDevice> Devices { get; set; }
    [Navigate(typeof(SugarGroupUser), nameof(SugarGroupUser.UsersId), nameof(SugarGroupUser.GroupsId))]
    public List<SugarGroup> Groups { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarRealmUser.UsersId))]
    public List<SugarRealmUser> Realms { get; set; }

    public static SugarUser Transform(User user)
    {
        return new SugarUser
        {
            Id = user.Id,
            EmailVerified = user.EmailVerified,
            Email = user.Email,
            Firstname = user.Firstname,
            DeviceRegistrationToken = user.DeviceRegistrationToken,
            CreateDateTime = user.CreateDateTime,
            Lastname = user.Lastname,
            NotificationMode = user.NotificationMode,
            Name = user.Name,
            Source = user.Source,
            Status = user.Status,
            IdentityProvisioningId = user.IdentityProvisioningId,
            EncodedPicture = user.EncodedPicture,
            UpdateDateTime = user.UpdateDateTime,
            Consents = user.Consents == null ? new List<SugarConsent>() : user.Consents.Select(c => SugarConsent.Transform(c)).ToList(),
            Realms = user.Realms == null ? new List<SugarRealmUser>() : user.Realms.Select(r => new SugarRealmUser
            {
                RealmsName = r.RealmsName ?? r.Realm.Name
            }).ToList(),
            Claims = user.Consents == null ? new List<SugarUserClaim>() : user.OAuthUserClaims.Select(r => SugarUserClaim.Transform(r)).ToList(),
            Credentials = user.Credentials == null ? new List<SugarUserCredential>() : user.Credentials.Select(c => SugarUserCredential.Transform(c)).ToList(),
            Devices = user.Devices == null ? new List<SugarUserDevice>() : user.Devices.Select(d => SugarUserDevice.Transform(d)).ToList(),
            ExternalAuthProviders = user.ExternalAuthProviders == null ? new List<SugarUserExternalAuthProvider>() : user.ExternalAuthProviders.Select(e => SugarUserExternalAuthProvider.Transform(e)).ToList(),
            Groups = user.Groups == null ? new List<SugarGroup>() : user.Groups.Select(g => new SugarGroup
            {
                Id = g.GroupsId
            }).ToList()
        };
    }

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
            Sessions = Sessions == null ? new List<UserSession>() : Sessions.Select(s => s.ToDomain()).ToList(),
            OAuthUserClaims = Claims == null ? new List<UserClaim>() : Claims.Select(c => c.ToDomain()).ToList(),
            Credentials = Credentials == null ? new List<UserCredential>() : Credentials.Select(c => c.ToDomain()).ToList(),
            ExternalAuthProviders = ExternalAuthProviders == null ? new List<UserExternalAuthProvider>() : ExternalAuthProviders.Select(e => e.ToDomain()).ToList(),
            Consents = Consents == null ? new List<Consent>() : Consents.Select(c => c.ToDomain()).ToList(),
            Groups = Groups == null ? new List<GroupUser>() : Groups.Select(c => new GroupUser
            {
                GroupsId = c.Id,
                Group = c?.ToDomain()
            }).ToList(),
            Realms = Realms == null ? new List<RealmUser>() : Realms.Select(r => r.ToDomain()).ToList()
        };
    }
}
