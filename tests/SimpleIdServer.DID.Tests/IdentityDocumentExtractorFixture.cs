// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using DidKeyIdentityDocumentExtractor = SimpleIdServer.Did.Key.IdentityDocumentExtractor;

namespace SimpleIdServer.DID.Tests
{
    public class IdentityDocumentExtractorFixture
    {
        [Test]
        public async Task When_ExtractDidKey_Then_IdentityDocumentIsCorrect()
        {
            const string key = "did:key:zQ3shcFhrFGtxgnmPZKBPJfPRpJtVUz6ZLs8iLBqAmtv6zyxB";
            var extractor = new DidKeyIdentityDocumentExtractor();
            var result = await extractor.Extract(key, CancellationToken.None);
            var json = result.Serialize();
            Assert.NotNull(result);
        }
        // did:key:z6Mkv44joRDBMR8kbSSJDq5bZwpt8NS45X889yyddDPZBVkz
    }
}
