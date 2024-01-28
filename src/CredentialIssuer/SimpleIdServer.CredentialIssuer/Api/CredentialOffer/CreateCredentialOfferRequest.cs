// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialOffer;

public class CreateCredentialOfferRequest
{
    [JsonPropertyName(CredentialOfferResultNames.Grants)]
    public List<string> Grants { get; set; }
    [JsonPropertyName(CredentialOfferResultNames.CredentialConfigurationIds)]
    public List<string> CredentialConfigurationIds { get; set; } = new List<string>();
    [JsonPropertyName("sub")]
    public string Subject { get; set; }
}