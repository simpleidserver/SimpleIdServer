// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.DTOs;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.Api.CredentialOffer
{
    public class CredentialOfferParameter
    {
        /// <summary>
        /// REQUIRED.The URL of the Credential Issuer, the Wallet is requested to obtain one or more Credentials from
        /// </summary>
        [JsonPropertyName(CredentialOfferParameterNames.CredentialIssuer)]
        public string CredentialIssuer { get; set; }
        /// <summary>
        /// REQUIRED. A JSON array, where every entry is a JSON object or a JSON string. 
        /// </summary>
        [JsonPropertyName(CredentialOfferParameterNames.Credentials)]
        public JsonArray Credentials { get; set; }
        /// <summary>
        /// OPTIONAL. A JSON object indicating to the Wallet the Grant Types the Credential Issuer's AS is prepared to process for this credential offer. 
        /// </summary>
        [JsonPropertyName(CredentialOfferParameterNames.Grants)]
        public JsonObject Grants { get; set; }
    }
}
