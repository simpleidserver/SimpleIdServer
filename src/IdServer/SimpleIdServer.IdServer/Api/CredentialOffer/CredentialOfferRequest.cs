// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.CredentialOffer
{
    public class CredentialOfferRequest
    {
        [JsonPropertyName(CredentialOfferRequestNames.CredentialOffer)]
        public string CredentialOffer { get; set; }
        [JsonPropertyName(CredentialOfferRequestNames.CredentialOfferUri)]
        public string CredentialOfferUri { get; set; }
    }
}
