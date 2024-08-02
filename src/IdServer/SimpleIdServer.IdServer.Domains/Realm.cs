// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains.DTOs;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    [Table("Realms")]
    public class Realm
    {
        [JsonPropertyName(RealmNames.Name)]
        public string Name { get; set; } = null!;
        [JsonPropertyName(RealmNames.Description)]
        public string? Description { get; set; } = null;
        [JsonPropertyName(RealmNames.CreateDateTime)]
        public DateTime CreateDateTime { get; set; }
        [JsonPropertyName(RealmNames.UpdateDateTime)]
        public DateTime UpdateDateTime { get; set; }
        [JsonIgnore]
        public ICollection<Client> Clients { get; set; } = new List<Client>();
        [JsonIgnore]
        public ICollection<RealmUser> Users { get; set; } = new List<RealmUser>();
        [JsonIgnore]
        public ICollection<Scope> Scopes { get; set; } = new List<Scope>();
        [JsonIgnore]
        public ICollection<AuthenticationContextClassReference> AuthenticationContextClassReferences { get; set; } = new List<AuthenticationContextClassReference>();
        [JsonIgnore]
        public ICollection<AuthenticationSchemeProvider> AuthenticationSchemeProviders { get; set; } = new List<AuthenticationSchemeProvider>();
        [JsonIgnore]
        public ICollection<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
        [JsonIgnore]
        public ICollection<SerializedFileKey> SerializedFileKeys { get; set; } = new List<SerializedFileKey>();
        [JsonIgnore]
        public ICollection<CertificateAuthority> CertificateAuthorities { get; set; } = new List<CertificateAuthority>();
        [JsonIgnore]
        public ICollection<IdentityProvisioning> IdentityProvisioningLst { get; set; } = new List<IdentityProvisioning>();
        [JsonIgnore]
        public ICollection<GroupRealm> Groups { get; set; } = new List<GroupRealm>();
        [JsonIgnore]
        public ICollection<RegistrationWorkflow> RegistrationWorkflows { get; set; } = new List<RegistrationWorkflow>();
        [JsonIgnore]
        public ICollection<PresentationDefinition> PresentationDefinitions { get; set; } = new List<PresentationDefinition>();
    }
}