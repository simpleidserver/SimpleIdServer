// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using NUnit.Framework;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Key;
using SimpleIdServer.Vc;
using SimpleIdServer.Vc.Models;
using SimpleIdServer.Vp.Models;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.Vp.Tests;

public class SecuredVerifiablePresentationFixture
{
    #region Ed25519VerificationKey2020

    [Test]
    public async Task When_Secure_VerifiablePresentation_With_Random_Ed25519VerificationKey2020_VerificationKey_Then_Proof_Is_Valid()
    {
        // ARRANGE
        var verifiableCredential = BuildVerifiableCredential();
        var verifiablePresentation = BuildVerifiablePresentation(verifiableCredential);

        // ACT
        var resolver = new DidFactoryResolver(new List<IDidResolver>
        {
            DidKeyResolver.New()
        });
        var vpVerifier = new VpVerifier(resolver);
        await vpVerifier.Verify(verifiablePresentation, CancellationToken.None);

        // ASSERT
        Assert.IsNotNull(vpVerifier);
    }

    #endregion

    private W3CVerifiableCredential BuildVerifiableCredential()
    {
        var ed25119Sig = Ed25519SignatureKey.Generate();
        var did = DidKeyGenerator.New().Generate(ed25119Sig);
        var didDocument = DidKeyResolver.New().Resolve(did, CancellationToken.None).Result;
        var credential = VcBuilder.New("https://example.com/credentials/1872", null, did, "IDCardCredential")
            .AddCredentialSubject((act) =>
            {
                act.AddClaim("given_name", "Fredrik");
                act.AddClaim("family_name", "Strömberg");
                act.AddClaim("birthdate", "1949-01-22");
            })
            .Build();
        var vc = SecuredDocument.New();
        vc.Secure(
            credential, 
            didDocument,
            didDocument.VerificationMethod.First().Id,
            asymKey: ed25119Sig);
        return credential;
    }

    private VerifiablePresentation BuildVerifiablePresentation(W3CVerifiableCredential credential)
    {
        var ed25119Sig = Ed25519SignatureKey.Generate();
        var did = DidKeyGenerator.New().Generate(ed25119Sig);
        var didDocument = DidKeyResolver.New().Resolve(did, CancellationToken.None).Result;
        var presentation = VpBuilder.New("ebc6f1c2", did)
            .AddVerifiableCredential(credential)
            .Build();
        var vc = SecuredDocument.New();
        vc.Secure(
            presentation,
            didDocument,
            didDocument.VerificationMethod.First().Id,
            asymKey: ed25119Sig);
        return presentation;
    }
}
