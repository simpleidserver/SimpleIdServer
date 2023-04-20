// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleIdServer.Did.Models
{
    public class IdentityDocument
    {
        [JsonPropertyName("@context")]
        public IEnumerable<string> Context { get; set; }
        [JsonPropertyName("id")]
        public string Id { get; set; }
        /// <summary>
        /// Expression verification methods, such as cryptographic public keys, which can be used to authenticate or authorize interactions with the DID subject of associated parties.
        /// Public Key can be used as a verification method with respect to a digital signature, it verifies that the signer could use the associated cryptographic private key.
        /// </summary>
        [JsonPropertyName("verificationMethod")]
        public ICollection<IdentityDocumentVerificationMethod> VerificationMethod { get; set; } = new List<IdentityDocumentVerificationMethod>();
        /// <summary>
        /// Used to specify how the DID subject is expected to be authenticated.
        /// Each verification method MAY be embedded or referenced.
        /// </summary>
        [JsonPropertyName("authentication")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ICollection<string> Authentication { get; set; } = null;
        [JsonPropertyName("assertionMethod")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ICollection<string> AssertionMethod { get; set; } = null;
        /// <summary>
        /// Express ways of communicating with the DID subject or associated entities.
        /// A service can be any type of service the DID subject wants to advertise.
        /// </summary>
        [JsonPropertyName("service")]
        public ICollection<IdentityDocumentService> Service { get; set; } = new List<IdentityDocumentService>();
    }
}
