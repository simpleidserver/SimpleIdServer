// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.CredentialOffer
{
    public class CredentialOfferResult
    {
        /// <summary>
        /// The Credential Issuer URL of the Credential Issuer, the Wallet is requested to obtain one or more Credentials from
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.CredentialIssuer)]
        public string CredentialIssuer { get; set; } = null!;
        [JsonPropertyName(CredentialOfferResultNames.Credentials)]
        public ICollection<string> Credentials { get; set; } = new List<string>();
        /// <summary>
        /// A JSON object indicating to the Wallet the Grant Types the Credential Issuer's AS is prepared to process for this credential offer.
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.Grants)]
        public Dictionary<string, object> Grants { get; set; } = new Dictionary<string, object>();
    }
}
