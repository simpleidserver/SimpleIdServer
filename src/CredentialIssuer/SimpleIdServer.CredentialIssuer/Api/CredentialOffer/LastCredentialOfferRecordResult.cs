// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialOffer;

public class LastCredentialOfferRecordResult : BaseCredentialOfferRecordResult
{
    [JsonPropertyName(CredentialOfferRecordNames.CredentialConfigurationIds)]
    public List<string> CredentialConfigurationIds { get; set; } = new List<string>();
}