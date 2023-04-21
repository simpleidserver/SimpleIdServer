// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using NUnit.Framework;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Models;
using System.Text.Json;

namespace SimpleIdServer.DID.Tests
{
    public class EthrIdentityDocumentBuilderFixture
    {
        [Test]
        public void When_IdentifierContainsEthereumAdr_Then_JSONIsStandard()
        {
            var identityDocument = JsonSerializer.Deserialize<IdentityDocument>(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "files", "did-ethr-0xb9c5714089478a327f09197987f16f9e5d936e8a.json")));
            var expectedJson = identityDocument.Serialize();
            var didDocument = EthrIdentityDocumentBuilder.NewEthr("did:ethr:0xb9c5714089478a327f09197987f16f9e5d936e8a").Build();
            var json = didDocument.Serialize();
            Assert.That(json, Is.EqualTo(expectedJson));
        }
    }
}
