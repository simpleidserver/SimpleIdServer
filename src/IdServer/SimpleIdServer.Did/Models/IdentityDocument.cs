// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Did.Events;
using System.Collections.Generic;
using System.Text.Json;
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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ICollection<IdentityDocumentVerificationMethod> VerificationMethod { get; set; } = null;
        /// <summary>
        /// Used to specify how the DID subject is expected to be authenticated.
        /// Each verification method MAY be embedded or referenced, for purposes sur as logging into a website.
        /// </summary>
        [JsonPropertyName("authentication")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ICollection<string> Authentication { get; set; } = null;
        /// <summary>
        /// Used to specify how the DID subject is excepted to express claims, such as the purposes of issuing a verifiable credential.
        /// </summary>
        [JsonPropertyName("assertionMethod")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ICollection<string> AssertionMethod { get; set; } = null;
        /// <summary>
        /// Express ways of communicating with the DID subject or associated entities.
        /// A service can be any type of service the DID subject wants to advertise.
        /// </summary>
        [JsonPropertyName("service")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ICollection<IdentityDocumentService> Service { get; set; } = null;
        [JsonIgnore]
        public ICollection<IEvent> Events { get; set; } = new List<IEvent>();

        public void AddVerificationMethod(IdentityDocumentVerificationMethod verificationMethod)
        {
            if (VerificationMethod == null) VerificationMethod = new List<IdentityDocumentVerificationMethod>();
            VerificationMethod.Add(verificationMethod);
        }

        public void AddAuthentication(string authentication)
        {
            if (Authentication == null) Authentication = new List<string>();
            Authentication.Add(authentication);
        }

        public void AddAssertionMethod(string assertionMethod)
        {
            if(AssertionMethod == null) AssertionMethod = new List<string>();
            AssertionMethod.Add(assertionMethod);
        }

        public void AddService(IdentityDocumentService service, bool isStandard = true)
        {
            if (Service == null) Service = new List<IdentityDocumentService>();
            Service.Add(service);
            if (!isStandard) Events.Add(new ServiceAdded
            {
                Id = service.Id,
                ServiceEndpoint = service.ServiceEndpoint,
                Type = service.Type
            });
        }

        public string Serialize() => JsonSerializer.Serialize(this);
    }
}
