// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Vc.Tests
{
    public class VerifiableCredentialBuilderFixture
    {
        [Test]
        public void When_BuildVerifiableCredential_Then_JSONIsCorrect()
        {
            var fileContent = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "files", "verifiablecredential.json"));
            var exceptedJson = JsonObject.Parse(fileContent).AsObject().ToJsonString();
            const string credentialSubject = "{\"degree\": { \"type\": \"BachelorDegree\", \"name\": \"Baccalauréat en musiques numériques\" }}";
            var verifiableCredential = VerifiableCredentialBuilder
                .New()
                .SetCredentialSubject(JsonObject.Parse(credentialSubject).AsObject()).Build();
            var json = verifiableCredential.Serialize();
            Assert.That(json, Is.EqualTo(exceptedJson));
        }
    }
}