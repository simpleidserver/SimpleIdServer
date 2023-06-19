// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using System.Text.Json.Nodes;
using DidKeyIdentityDocumentExtractor = SimpleIdServer.Did.Key.IdentityDocumentExtractor;

namespace SimpleIdServer.DID.Tests
{
    public class IdentityDocumentExtractorFixture
    {
        [Test]
        public async Task When_ExtractDidKey_Ed25519VerificationKey2020_Then_IdentityDocumentIsCorrect()
        {
            var expectedJson = JsonObject.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "files", "did-key-zQ3shcFhrFGtxgnmPZKBPJfPRpJtVUz6ZLs8iLBqAmtv6zyxB.json"))).ToJsonString();
            const string key = "did:key:zQ3shcFhrFGtxgnmPZKBPJfPRpJtVUz6ZLs8iLBqAmtv6zyxB";
            var extractor = new DidKeyIdentityDocumentExtractor(new Did.Key.DidKeyOptions());
            var result = await extractor.Extract(key, CancellationToken.None);
            var json = result.Serialize();
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        public async Task When_ExtractDidKey_JsonWebKey2020_Ed25519_Then_IdentityDocumentIsCorrect()
        {
            var expectedJson = JsonObject.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "files", "did-key-z6MkemXVAYXaxbZoJhn1iRuhjQNLJQQYjuxEpc4eN9oQBhEa.json"))).ToJsonString();
            const string key = "did:key:z6MkemXVAYXaxbZoJhn1iRuhjQNLJQQYjuxEpc4eN9oQBhEa";
            var extractor = new DidKeyIdentityDocumentExtractor(new Did.Key.DidKeyOptions { PublicKeyFormat = Did.Constants.VerificationMethodTypes.JsonWebKey2020 });
            var result = await extractor.Extract(key, CancellationToken.None);
            var json = result.Serialize();
            Assert.That(json, Is.EqualTo(expectedJson));
        }
    }
}
