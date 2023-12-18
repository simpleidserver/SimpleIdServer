// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Did.Models
{
    public class DidDocument
    {
        [JsonPropertyName("@context")]
        public JsonNode Context { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        /// <summary>
        /// DID controller is an entity that is authorized to make changed to a DID Document.
        /// Process of authorizing a DID Controller is defined by the DID Method.
        /// </summary>
        [JsonPropertyName("controller")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonNode Controller { get; set; }
        /// <summary>
        /// A DID subject can have multiple identifiers for different purposes, or at different times. 
        /// The assertion that two or more DIDs (or other types of URI) refer to the same DID subject can be made using the alsoKnownAs property.
        /// </summary>
        [JsonPropertyName("alsoKnownAs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> AlsoKnownAs { get; set; } = null;
        /// <summary>
        /// Expression verification methods, such as cryptographic public keys, which can be used to authenticate or authorize interactions with the DID subject of associated parties.
        /// Public Key can be used as a verification method with respect to a digital signature, it verifies that the signer could use the associated cryptographic private key.
        /// </summary>
        [JsonPropertyName("verificationMethod")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ICollection<DidDocumentVerificationMethod> VerificationMethod { get; set; } = new List<DidDocumentVerificationMethod>();
        /// <summary>
        /// Used to specify how the DID subject is expected to be authenticated.
        /// Each verification method MAY be embedded or referenced, for purposes sur as logging into a website.
        /// </summary>
        [JsonPropertyName("authentication")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonArray Authentication { get; set; } = null;
        /// <summary>
        /// Used to specify how the DID subject is excepted to express claims, such as the purposes of issuing a verifiable credential.
        /// </summary>
        [JsonPropertyName("assertionMethod")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonArray AssertionMethod { get; set; } = null;
        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("keyAgreement")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonArray KeyAgreement { get; set; } = null;
        /// <summary>
        /// The capabilityInvocation verification relationship is used to specify a verification method that might be used by the DID subject to invoke a cryptographic capability,
        /// such as the authorization to update the DID Document.
        /// </summary>
        [JsonPropertyName("capabilityInvocation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonArray CapabilityInvocation { get; set; } = null;
        /// <summary>
        /// The capabilityDelegation verification relationship is used to specify a mechanism that might be used by the DID subject to delegate a cryptographic capability 
        /// to another party, such as delegating the authority to access a specific HTTP API to a subordinate.
        /// </summary>
        [JsonPropertyName("capabilityDelegation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonArray CapabilityDelegation { get; set; } = null;
        /// <summary>
        /// Express ways of communicating with the DID subject or associated entities.
        /// A service can be any type of service the DID subject wants to advertise.
        /// </summary>
        [JsonPropertyName("service")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ICollection<DidDocumentService> Service { get; set; } = null;

        public KeyPurposes GetKeyPurpose(DidDocumentVerificationMethod verificationMethod)
        {
            // if (Authentication != null && Authentication.Any(a => verificationMethod.Id == a)) return KeyPurposes.SigAuthentication;
            // if (AssertionMethod != null && AssertionMethod.Any(m => verificationMethod.Id == m)) return KeyPurposes.VerificationKey;
            throw new InvalidOperationException("enc is not supported");
        }

        public void AddVerificationMethod(DidDocumentVerificationMethod verificationMethod, bool isStandard = true)
        {
            if (VerificationMethod == null) VerificationMethod = new List<DidDocumentVerificationMethod>();
            VerificationMethod.Add(verificationMethod);
        }

        public void AddAuthentication(string authentication)
        {
            // if (Authentication == null) Authentication = new List<string>();
            // Authentication.Add(authentication);
        }

        public void AddAssertionMethod(string assertionMethod)
        {
            // if(AssertionMethod == null) AssertionMethod = new List<string>();
            // AssertionMethod.Add(assertionMethod);
        }

        public void AddService(DidDocumentService service, bool isStandard = true)
        {
            if (Service == null) Service = new List<DidDocumentService>();
            Service.Add(service);
        }

        public string Serialize() => JsonSerializer.Serialize(this);
    }
}
