// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.CredentialIssuer.DTOs;
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.IdServer.CredentialIssuer.Api.CredentialOffer
{
    public class BaseCredentialOfferResult
    {
        /// <summary>
        /// The Credential Issuer URL of the Credential Issuer, the Wallet is requested to obtain one or more Credentials from
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.CredentialIssuer)]
        public string CredentialIssuer { get; set; } = null!;
        /// <summary>
        /// A JSON object indicating to the Wallet the Grant Types the Credential Issuer's AS is prepared to process for this credential offer.
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.Grants)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CredentialOfferGrants Grants { get; set; } = null;
    }

    public class LastCredentialOfferResult : BaseCredentialOfferResult
    {
        /// <summary>
        /// Array of unique strings that each identify one of the keys in the name/value pairs stored in the credential_configurations_supported Credential Issuer metadata.
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.CredentialConfigurationIds)]
        public ICollection<string> CredentialConfigurationIds { get; set; } = new List<string>();
    }

    public class EsbiCredentialOfferResult : BaseCredentialOfferResult
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

    public class CredentialOfferGrants
    {
        /// <summary>
        /// Grant Type urn:ietf:params:oauth:grant-type:pre-authorized_code.
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.PreAuthorizedCodeGrant)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PreAuthorizedCodeGrant PreAuthorizedCodeGrant { get; set; }
        /// <summary>
        /// Grant Type authorization_code
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.AuthorizedCodeGrant)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public AuthorizedCodeGrant AuthorizedCodeGrant { get; set; }
    }

    public class PreAuthorizedCodeGrant
    {
        /// <summary>
        /// The code representing the Credential Issuer's authorization for the Wallet to obtain Credentials of a certain type. 
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.PreAuthorizedCode)]
        public string PreAuthorizedCode { get; set; }
        /// <summary>
        /// Object specifying whether the Authorization Server expects presentation of a Transaction Code by the End-User along with the Token Request in a Pre-Authorized Code Flow.
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.Transaction)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PreAuthorizedCodeGrantTransaction Transaction { get; set; }
        /// <summary>
        /// The minimum amount of time in seconds that the Wallet SHOULD wait between polling requests to the token endpoint.
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.Interval)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Interval { get; set; }
        /// <summary>
        /// Wallet can use to identify the Authorization Server to use with this grant type when authorization_servers parameter in the Credential Issuer metadata has multiple entries.
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.AuthorizationServer)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AuthorizationServer { get; set; }
    }

    public class AuthorizedCodeGrant
    {
        /// <summary>
        ///  String value created by the Credential Issuer and opaque to the Wallet that is used to bind the subsequent Authorization Request with the Credential Issuer to a context set up during previous steps. 
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.IssuerState)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? IssuerState { get; set; } = null;
        /// <summary>
        /// Wallet can use to identify the Authorization Server to use with this grant type when authorization_servers parameter in the Credential Issuer metadata has multiple entries.
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.AuthorizationServer)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AuthorizationServer { get; set; } = null;
    }

    public class PreAuthorizedCodeGrantTransaction
    {
        /// <summary>
        /// String specifying the input character set.
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.InputMode)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? InputMode { get; set; } = "numeric";
        /// <summary>
        /// Integer specifying the length of the Transaction Code. 
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.Length)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? Length { get; set; }
        /// <summary>
        /// String containing guidance for the Holder of the Wallet on how to obtain the Transaction Code, e.g., describing over which communication channel it is delivered.
        /// </summary>
        [JsonPropertyName(CredentialOfferResultNames.Description)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }
    }
}
