// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.CredentialIssuer.DTOs;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.CredentialIssuer.Api.CredentialIssuer;

public class ESBICredentialIssuerResult
{
    /// <summary>
    /// The credential issuer's identifier.
    /// </summary>
    [JsonPropertyName(CredentialIssuerResultNames.CredentialIssuer)]
    public string CredentialIssuer { get; set; } = null!;

    /// <summary>
    /// An array of strings, where each string is an identifier of the OAuth 2.0 Authorization Server the credential issuer relies on for authorization.
    /// </summary>
    [JsonPropertyName(CredentialIssuerResultNames.AuthorizationServer)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string AuthorizationServer { get; set; } = null;

    /// <summary>
    /// URL of the credential issuer's credential endpoint.
    /// </summary>
    [JsonPropertyName(CredentialIssuerResultNames.CredentialEndpoint)]
    public string CredentialEndpoint { get; set; } = null!;

    /// <summary>
    /// URL of the Credential Issuer's Batch Credential Endpoint. 
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName(CredentialIssuerResultNames.BatchCredentialEndpoint)]
    public string? BatchCredentialEndpoint { get; set; } = null;

    /// <summary>
    /// URL of the Credential Issuer's Deferred Credential Endpoint. 
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName(CredentialIssuerResultNames.DeferredCredentialEndpoint)]
    public string? DeferredCredentialEndpoint { get; set; } = null;

    /// <summary>
    /// Array containing a list of the JWE [RFC7516] encryption algorithms (alg values) [RFC7518] supported by the Credential and/or Batch Credential Endpoint to encode the Credential or Batch Credential Response in a JWT.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName(CredentialIssuerResultNames.CredentialResponseEncryptionAlgValuesSupported)]
    public IEnumerable<string> CredentialResponseEncryptionAlgValuesSupported { get; set; } = null;

    /// <summary>
    /// Array containing a list of the JWE [RFC7516] encryption algorithms (enc values) [RFC7518] supported by the Credential and/or Batch Credential Endpoint to encode the Credential or Batch Credential Response in a JWT.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName(CredentialIssuerResultNames.CredentialResponseEncryptionEncValuesSupported)]
    public IEnumerable<string> CredentialResponseEncryptionEncValuesSupported { get; set; } = null;

    /// <summary>
    /// Boolean value specifying whether the Credential Issuer requires additional encryption on top of TLS for the Credential Response and expects encryption parameters to be present in the Credential Request and/or Batch Credential Request, with true indicating support.
    /// </summary>
    [JsonPropertyName(CredentialIssuerResultNames.RequireCredentialResponseEncryption)]
    public bool RequireCredentialResponseEncryption { get; set; }

    /// <summary>
    /// Boolean value specifying whether the Credential Issuer supports returning credential_identifiers parameter in the authorization_details Token Response parameter, with true indicating support. If omitted, the default value is false.
    /// </summary>
    [JsonPropertyName(CredentialIssuerResultNames.CredentialIdentifiersSupported)]
    public bool CredentialIdentifiersSupported { get; set; }

    /// <summary>
    /// Each object contains display properties of a Credential Issuer for a certain language.
    /// </summary>
    [JsonPropertyName(CredentialIssuerResultNames.Display)]
    public ICollection<CredentialIssuerDisplayResult> Display { get; set; } = new List<CredentialIssuerDisplayResult>();

    /// <summary>
    /// List of JSON Objects, each of them representing metadata about a separate credential type that the credential issuer can issue.
    /// </summary>
    [JsonPropertyName(CredentialIssuerResultNames.CredentialsSupported)]
    public List<JsonObject> CredentialsSupported { get; set; } = new List<JsonObject>();
}
