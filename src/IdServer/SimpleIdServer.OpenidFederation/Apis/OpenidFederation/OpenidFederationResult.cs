// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace SimpleIdServer.OpenidFederation.Apis.OpenidFederation;

[DebuggerDisplay("Subject = {Sub}")]
public class OpenidFederationResult
{
    [JsonIgnore]
    public DateTime? ValidTo
    {
        get
        {
            return GetDateTime(Exp);
        }
    }

    [JsonIgnore]
    public DateTime? IssuedAt
    {
        get
        {
            return GetDateTime(Iat);
        }
    }

    /// <summary>
    /// The Entity Identifier of the issuer of the Entity Statement.
    /// </summary>
    [JsonPropertyName("iss")]
    public string Iss { get; set; } = null!;
    /// <summary>
    /// The Entity Identifier of the subject.
    /// </summary>
    [JsonPropertyName("sub")]
    public string Sub { get; set; } = null!;
    /// <summary>
    /// Number. Time when this statement was issued
    /// </summary>
    [JsonPropertyName("iat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? Iat { get; set; }
    /// <summary>
    /// Number. Expiration time after which the statement MUST NOT be accepted for processing.
    /// </summary>
    [JsonPropertyName("exp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? Exp { get; set; }
    /// <summary>
    /// A JSON Web Key Set (JWKS) [RFC7517] representing the public part of the subject's Federation Entity signing keys. 
    /// </summary>
    [JsonPropertyName("jwks")]
    public OpenidFederationJwksResult Jwks { get; set; } = new OpenidFederationJwksResult();
    /// <summary>
    /// An array of strings representing the Entity Identifiers of Intermediate Entities or Trust Anchors that MAY issue Subordinate Statements about the Entity. 
    /// </summary>
    [JsonPropertyName("authority_hints")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> AuthorityHints { get; set; }
    [JsonPropertyName("metadata")]
    public OpenidFederationMetadataResult Metadata { get; set; } = new OpenidFederationMetadataResult();

    private DateTime? GetDateTime(long? time)
    {
        if (time == null) return null;
        return EpochTime.DateTime(time.Value);
    }
}