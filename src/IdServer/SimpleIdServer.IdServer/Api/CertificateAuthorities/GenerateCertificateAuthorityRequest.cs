// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.CertificateAuthorities;

public class GenerateCertificateAuthorityRequest
{
    [JsonPropertyName(CertificateAuthorityNames.SubjectName)]
    public string SubjectName { get; set; }
    [JsonPropertyName(CertificateAuthorityNames.NumberOfDays)]
    public int NumberOfDays { get; set; }
}
