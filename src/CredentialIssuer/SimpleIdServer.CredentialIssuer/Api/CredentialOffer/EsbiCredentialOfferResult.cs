// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialOffer;

public class EsbiCredentialOfferResult : BaseCredentialOfferRecordResult
{
    [JsonPropertyName("credentials")]
    public List<EsbiCredential> Credentials { get; set; }
}

public class EsbiCredential
{
    [JsonPropertyName(CredentialOfferRecordNames.Format)]
    public string Format { get; set; }
    [JsonPropertyName(CredentialOfferRecordNames.Types)]
    public List<string> Types { get; set; }
}