// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did.Crypto.Multicodec;

namespace SimpleIdServer.Did.Ethr.Tests;

public class DidEthrResolverFixture
{
    /*
    var shaKec = new Sha3Keccack();
    var chars = "b9c5714089478a327f09197987f16f9e5d936e8a".ToArray();
    var hashed = shaKec.CalculateHash("b9c5714089478a327f09197987f16f9e5d936e8a");

    for (var i = 0; i < 40; i += 2)
    {
        if ((hashed[i >> 1] >> 4) >= 8)
        {
            chars[i] = chars[i];
        }
        if ((hashed[i >> 1] & 0x0f) >= 8)
        {
            chars[i + 1] = chars[i + 1];
        }
    }

    var s = ($"0x" + new string(chars)).ToUpper();
    */

    [Test]
    public async Task When_Resolve_EthereumAddress_DID_With_No_Transactions_Then_DID_Document_Is_Resolved()
    {
        // ARRANGE
        var store = new NetworkConfigurationStore();
        store.NetworkConfigurations.Add(new Models.NetworkConfiguration
        {
            Name = "mainnet",
            ContractAdr = "0xdca7ef03e98e0dc2b855be647c39abe984fcf21b",
            RpcUrl = "https://mainnet.infura.io/v3/405e16111db4419e8d94431737f8ba53",
            UpdateDateTime = DateTime.UtcNow,
            CreateDateTime = DateTime.UtcNow
        });
        var serializer = MulticodecSerializerFactory.Build();
        var resolver = new DidEthrResolver(store, serializer);
        
        // ACT
        var didDocument = await resolver.Resolve("did:ethr:0xb9c5714089478a327f09197987f16f9e5d936e8a", CancellationToken.None);
        var serialize = didDocument.Serialize();

        // ASSERT
        var contextLst = didDocument.Context.AsArray().Select(c => c.ToString());
        Assert.True(contextLst.Contains("https://www.w3.org/ns/did/v1"));
        Assert.True(contextLst.Contains("https://w3id.org/security/suites/secp256k1recovery-2020/v2"));
        Assert.That(didDocument.Id, Is.EqualTo("did:ethr:0xb9c5714089478a327f09197987f16f9e5d936e8a"));
        Assert.That(didDocument.VerificationMethod.Count(), Is.EqualTo(1));
        Assert.That(didDocument.VerificationMethod.First().Id, Is.EqualTo("did:ethr:0xb9c5714089478a327f09197987f16f9e5d936e8a#controller"));
        Assert.That(didDocument.VerificationMethod.First().Type, Is.EqualTo("EcdsaSecp256k1RecoveryMethod2020"));
        Assert.That(didDocument.VerificationMethod.First().Controller, Is.EqualTo("did:ethr:0xb9c5714089478a327f09197987f16f9e5d936e8a"));
        Assert.That(didDocument.VerificationMethod.First().BlockChainAccountId, Is.EqualTo("eip155:1:0xb9c5714089478a327f09197987f16f9e5d936e8a"));
        Assert.That(didDocument.Authentication.Count(), Is.EqualTo(1));
        Assert.That(didDocument.AssertionMethod.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task When_Resolve_PublicKey_DID_With_No_Transactions_Then_DID_Document_Is_Resolved()
    {
        // ARRANGE
        var store = new NetworkConfigurationStore();
        store.NetworkConfigurations.Add(new Models.NetworkConfiguration
        {
            Name = "mainnet",
            ContractAdr = "0xdca7ef03e98e0dc2b855be647c39abe984fcf21b",
            RpcUrl = "https://mainnet.infura.io/v3/405e16111db4419e8d94431737f8ba53",
            UpdateDateTime = DateTime.UtcNow,
            CreateDateTime = DateTime.UtcNow
        });
        var serializer = MulticodecSerializerFactory.Build();
        var resolver = new DidEthrResolver(store, serializer);

        // ACT
        var didDocument = await resolver.Resolve("did:ethr:0x0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798", CancellationToken.None);
        var json = didDocument.Serialize();

        // ASSERT
        var contextLst = didDocument.Context.AsArray().Select(c => c.ToString());
        Assert.True(contextLst.Contains("https://www.w3.org/ns/did/v1"));
        Assert.True(contextLst.Contains("https://w3id.org/security/suites/secp256k1recovery-2020/v2"));
        Assert.True(contextLst.Contains("https://w3id.org/security/suites/secp256k1-2019/v1"));
        Assert.That(didDocument.Id, Is.EqualTo("did:ethr:0x0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798"));
        Assert.That(didDocument.VerificationMethod.Count(), Is.EqualTo(2));
        Assert.That(didDocument.VerificationMethod.First().Id, Is.EqualTo("did:ethr:0x0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798#controller"));
        Assert.That(didDocument.VerificationMethod.First().Type, Is.EqualTo("EcdsaSecp256k1RecoveryMethod2020"));
        Assert.That(didDocument.VerificationMethod.First().Controller, Is.EqualTo("did:ethr:0x0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798"));
        Assert.That(didDocument.VerificationMethod.First().BlockChainAccountId, Is.EqualTo("eip155:1:0x7E5F4552091A69125d5DfCb7b8C2659029395Bdf"));
        Assert.That(didDocument.VerificationMethod.Last().Id, Is.EqualTo("did:ethr:0x0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798#controllerKey"));
        Assert.That(didDocument.VerificationMethod.Last().Type, Is.EqualTo("EcdsaSecp256k1VerificationKey2019"));
        Assert.That(didDocument.VerificationMethod.Last().Controller, Is.EqualTo("did:ethr:0x0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798"));
        Assert.That(didDocument.VerificationMethod.Last().PublicKeyHex, Is.EqualTo("0279be667ef9dcbbac55a06295ce870b07029bfcdb2dce28d959f2815b16f81798"));
        Assert.That(didDocument.Authentication.Count(), Is.EqualTo(2));
        Assert.That(didDocument.AssertionMethod.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task When_Resolve_DID_With_Transactions_Then_DID_Document_Is_Resolved()
    {
        var store = new NetworkConfigurationStore();
        store.NetworkConfigurations.Add(new Models.NetworkConfiguration
        {
            Name = "mainnet",
            ContractAdr = "0xdca7ef03e98e0dc2b855be647c39abe984fcf21b",
            RpcUrl = "https://mainnet.infura.io/v3/405e16111db4419e8d94431737f8ba53",
            UpdateDateTime = DateTime.UtcNow,
            CreateDateTime = DateTime.UtcNow
        });
        var serializer = MulticodecSerializerFactory.Build();
        var resolver = new DidEthrResolver(store, serializer);

        // ACT
        // var didDocument = await resolver.Resolve("did:ethr:0x6918893854B2Eb01B194c46c4Efe2ea1ef36B7BC", CancellationToken.None);
        var didDocument = await resolver.Resolve("did:ethr:0x19711CD19e609FEBdBF607960220898268B7E24b", CancellationToken.None);
        var serialize = didDocument.Serialize();

        // did:ethr:0x26bf14321004e770e7a8b080b7a526d8eed8b388
        // https://github.com/decentralized-identity/ethr-did-resolver/blob/master/doc/did-method-spec.md#read-resolve
    }
}