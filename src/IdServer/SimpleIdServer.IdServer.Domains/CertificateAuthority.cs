// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class CertificateAuthority
    {
        [JsonPropertyName(CertificateAuthorityNames.Id)]
        public string Id { get; set; } = null!;
        [JsonPropertyName(CertificateAuthorityNames.SubjectName)]
        public string SubjectName { get; set; } = null!;
        [JsonPropertyName(CertificateAuthorityNames.Source)]
        public CertificateAuthoritySources Source { get; set; } = CertificateAuthoritySources.DB;
        [JsonPropertyName(CertificateAuthorityNames.StoreLocation)]
        public StoreLocation? StoreLocation { get; set; } = null;
        [JsonPropertyName(CertificateAuthorityNames.StoreName)]
        public StoreName? StoreName { get; set; } = null;
        [JsonPropertyName(CertificateAuthorityNames.FindType)]
        public X509FindType? FindType { get; set; } = null;
        [JsonPropertyName(CertificateAuthorityNames.FindValue)]
        public string? FindValue { get; set; } = null;
        [JsonPropertyName(CertificateAuthorityNames.PublicKey)]
        public string? PublicKey { get; set; } = null;
        [JsonPropertyName(CertificateAuthorityNames.PrivateKey)]
        public string? PrivateKey { get; set; } = null;
        [JsonPropertyName(CertificateAuthorityNames.StartDateTime)]
        public DateTime StartDateTime { get; set; }
        [JsonPropertyName(CertificateAuthorityNames.EndDateTime)]
        public DateTime EndDateTime { get; set; }
        [JsonPropertyName(CertificateAuthorityNames.UpdateDateTime)]
        public DateTime UpdateDateTime { get; set; }
        [JsonPropertyName(CertificateAuthorityNames.ClientCertificates)]
        public ICollection<ClientCertificate> ClientCertificates { get; set; } = new List<ClientCertificate>();
        [JsonIgnore]
        public ICollection<Realm> Realms { get; set; } = new List<Realm>();
    }

    public enum CertificateAuthoritySources
    {
        DB = 0,
        CERTIFICATESTORE = 1
    }
}
