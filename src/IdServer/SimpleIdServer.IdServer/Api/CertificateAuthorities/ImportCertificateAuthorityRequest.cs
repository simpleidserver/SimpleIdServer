// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.CertificateAuthorities;

public class ImportCertificateAuthorityRequest
{
    [JsonPropertyName(CertificateAuthorityNames.StoreLocation)]
    public StoreLocation StoreLocation { get; set; }
    [JsonPropertyName(CertificateAuthorityNames.StoreName)]
    public StoreName StoreName { get; set; }
    [JsonPropertyName(CertificateAuthorityNames.FindType)]
    public X509FindType FindType { get; set; }
    [JsonPropertyName(CertificateAuthorityNames.FindValue)]
    public string FindValue { get; set; }
}