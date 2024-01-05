// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using NUnit.Framework;
using SimpleIdServer.Did.Ethr.Services;

/*
namespace SimpleIdServer.Did.Ethr.Tests
{
    public class IdentityDocumentEthrFixture
    {
        private const string privateKey = "0fda34d0029c91481b1f54b0b68efea94c4572c80b2902cb3a2ab722b41fc1e1";
        private const string publicKey = "0x6f13168149E94B7BaC11B590cbE772593eD0742f";
        private const string contractAdr = "0x5721d5e733a2da7d805bdfb5177b4801cd86d3ae";
        private const string network = "sepolia";

        public async Task When_DeployContract_Then_NoException()
        {
            var accountService = new SmartContractService(new MockIdentityConfigurationStore(), Options.Create(new DidEthrOptions()));
            var balance = await accountService.UseAccount(privateKey).UseNetwork(network).DeployContractAndGetService();
        }

        public async Task When_AddIdentityDocument_Then_NoException()
        {
            var identityDocument = DidDocumentBuilder.New($"did:ethr:{network}:{publicKey}")
                .AddServiceEndpoint("github", "https://shorturl.at/eiDKO")
                // .AddVerificationMethod(SignatureKeyBuilder.NewES256K(), SimpleIdServer.Did.Constants.VerificationMethodTypes.Secp256k1VerificationKey2018)
                .Build();
            var sync = new IdentityDocumentSynchronizer(new DIDRegistryServiceFactory(new MockIdentityConfigurationStore(), Options.Create(new DidEthrOptions())));
            await sync.Sync(identityDocument, publicKey, privateKey, contractAdr);
        }

        // [Test]
        public async Task When_ReadIdentityDocument_Then_MetadataAreCorrect()
        {
            const string id = $"did:ethr:{network}:{publicKey}";
            var extractor = new IdentityDocumentExtractor(new MockIdentityConfigurationStore(), Options.Create(new DidEthrOptions()));
            var etherDidDocument = await extractor.Extract(id, CancellationToken.None);
            Assert.NotNull(etherDidDocument);
            Assert.That(etherDidDocument.Id, Is.EqualTo(id));
        }
    }
}
*/