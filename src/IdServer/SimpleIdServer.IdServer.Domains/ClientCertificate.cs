// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Domains
{
    public class ClientCertificate
    {
        [JsonPropertyName(ClientCertificateNames.Id)]
        public string Id { get; set; } = null!;
        [JsonPropertyName(ClientCertificateNames.Name)]
        public string Name { get; set; } = null!;
        [JsonPropertyName(ClientCertificateNames.StartDateTime)]
        public DateTime StartDateTime { get; set; }
        [JsonPropertyName(ClientCertificateNames.EndDateTime)]
        public DateTime EndDateTime { get; set; }
        [JsonPropertyName(ClientCertificateNames.CreateDateTime)]
        public DateTime CreateDateTime { get; set; }
        [JsonPropertyName(ClientCertificateNames.PublicKey)]
        public string PublicKey { get; set; } = null!;
        [JsonPropertyName(ClientCertificateNames.PrivateKey)]
        public string PrivateKey { get; set; } = null!;
        [JsonIgnore]
        public CertificateAuthority CertificateAuthority { get; set; }
    }
}
