// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;

namespace SimpleIdServer.Did.Ethr.Tests;

public class DidEthrResolverFixture
{
    [Test]
    public async Task When_Resolve_DidEthr_Then_DidDocumentIsCorrect()
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
        var resolver = new DidEthrResolver(store);

        // https://github.com/ethers-io/ethers.js/blob/6017d3d39a4d428793bddae33d82fd814cacd878/src.ts/address/address.ts#L8
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

        // ACT
        await resolver.Resolve("did:ethr:0xabcabc03e98e0dc2b855be647c39abe984193675", CancellationToken.None);
    }
}