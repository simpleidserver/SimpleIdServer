// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.DTOs;
using SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialOffer;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialOffer;

public class CredentialOfferRecordResult
{
    [JsonPropertyName(CredentialOfferRecordNames.Id)]
    public string Id { get; set; }
    [JsonPropertyName(CredentialOfferRecordNames.Subject)]
    public string Subject { get; set; } = null!;
    [JsonPropertyName(CredentialOfferRecordNames.GrantTypes)]
    public List<string> GrantTypes { get; set; } = new List<string>();
    [JsonPropertyName(CredentialOfferRecordNames.CredentialConfigurationIds)]
    public List<string> CredentialConfigurationIds { get; set; } = new List<string>();
    [JsonPropertyName(CredentialOfferRecordNames.CreateDateTime)]
    public DateTime CreateDateTime { get; set; }
    [JsonPropertyName(CredentialOfferRecordNames.Offer)]
    public CredentialOfferResult Offer { get; set; }
    [JsonPropertyName(CredentialOfferRecordNames.OfferUri)]
    public string OfferUri { get; set; }
}
