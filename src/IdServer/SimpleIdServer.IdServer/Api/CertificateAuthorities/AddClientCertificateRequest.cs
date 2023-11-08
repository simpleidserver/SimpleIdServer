// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.CertificateAuthorities;

public class AddClientCertificateRequest
{
    [JsonPropertyName(ClientCertificateNames.SubjectName)]
    public string SubjectName { get; set; }
    [JsonPropertyName(ClientCertificateNames.NbDays)]
    public int NbDays { get; set; }
}
