// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Vc.Models;

public class DataIntegrityProof
{
    /// <summary>
    /// An optional identifier for the proof, which MUST be a URL [URL], such as a UUID as a URN.
    /// </summary>
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Id { get; set; }
    /// <summary>
    /// The specific proof type used for the cryptographic proof MUST be specified as a string that maps to a URL.
    /// </summary>
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Type { get; set; }
    /// <summary>
    /// The reason the proof was created MUST be specified as a string that maps to a URL.
    /// assertionMethod : using cryptographic material typically used to create a Verifiable Credential.
    /// authentication : login process.
    /// </summary>
    [JsonPropertyName("proofPurpose")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ProofPurposeTypes? ProofPurpose { get; set; }
    /// <summary>
    /// The means and information needed to verify the proof MUST be specified as a string that maps to a URL.
    /// An example of a verification method is a link to a public key.
    /// </summary>
    [JsonPropertyName("verificationMethod")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string VerificationMethod { get; set; }
    /// <summary>
    /// The date and time the proof was created.
    /// </summary>
    [JsonPropertyName("created")]
    public DateTime? Created { get; set; }
    /// <summary>
    /// When the proof expires.
    /// </summary>
    [JsonPropertyName("expires")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? Expires { get; set; }
    /// <summary>
    /// It conveys one or more security domains in which the proof is meant to be used.
    /// </summary>
    [JsonPropertyName("domain")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> Domain { get; set; } = null;
    /// <summary>
    /// A string value that SHOULD be included in a proof if a domain is specified. 
    /// </summary>
    [JsonPropertyName("challenge")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Challenge { get; set; }
    /// <summary>
    /// A string value that contains the base-encoded binary data necessary to verify the digital proof using the verificationMethod specified.
    /// </summary>
    [JsonPropertyName("proofValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ProofValue { get; set; }
    /// <summary>
    /// An OPTIONAL string value or unordered list of string values. 
    /// Each value identifies another data integrity proof that MUST verify before the current proof is processed.
    /// </summary>
    [JsonPropertyName("previousProof")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<string> PreviousProof { get; set; } = null;
    /// <summary>
    /// An OPTIONAL string value supplied by the proof creator. 
    /// One use of this field is to increase privacy by decreasing linkability that is the result of deterministically generated signatures.
    /// </summary>
    [JsonPropertyName("nonce")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Nonce { get; set; }
    [JsonPropertyName("jws")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Jws { get; set; }
}
