// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using NUnit.Framework;
using SimpleIdServer.Did.Encoders;

namespace SimpleIdServer.Did.Key.Tests;

public class DidKeyResolverFixture
{
    [Test]
    public async Task When_Resolve_DidDocument_With_DefaultOptions_Then_VerificationMethodIsReturned()
    {
        // ARRANGE
        var key = "did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK";
        var resolver = DidKeyResolver.New(new DidKeyOptions
        {
            PublicKeyFormat = Ed25519VerificationKey2020Standard.TYPE
        });
        
        // ACT
        var didDocument = await resolver.Resolve(key, CancellationToken.None);

        // ASSERT
        Assert.That(didDocument != null);
        Assert.That(didDocument.VerificationMethod.First().Id, Is.EqualTo("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK#z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK"));
        Assert.That(didDocument.VerificationMethod.First().Type, Is.EqualTo("Ed25519VerificationKey2020"));
        Assert.That(didDocument.VerificationMethod.First().Controller, Is.EqualTo("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK"));
        Assert.That(didDocument.VerificationMethod.First().PublicKeyMultibase, Is.EqualTo("z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK"));
        Assert.That(didDocument.Authentication[0].ToString(), Is.EqualTo("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK#z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK"));
        Assert.That(didDocument.AssertionMethod[0].ToString(), Is.EqualTo("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK#z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK"));
        Assert.That(didDocument.CapabilityDelegation[0].ToString(), Is.EqualTo("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK#z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK"));
        Assert.That(didDocument.CapabilityInvocation[0].ToString(), Is.EqualTo("did:key:z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK#z6MkhaXgBZDvotDkL5257faiztiGiC2QtKLGpbnnEGta2doK"));
    }

    [Test]
    public async Task When_Resolve_DidDocument_With_JsonWebKey2020Formatter_Then_VerificationMethodIsCorrect()
    {
        // ARRANGE
        var key = "did:key:z6MkiTBz1ymuepAQ4HEHYSF1H8quG5GLVVQR3djdX3mDooWp";
        var resolver = DidKeyResolver.New(new DidKeyOptions 
        { 
            PublicKeyFormat = JsonWebKey2020Standard.TYPE 
        });

        // ACT
        var didDocument = await resolver.Resolve(key, CancellationToken.None);

        // ASSERT
        Assert.That(didDocument != null);
        Assert.That(didDocument.VerificationMethod.First().Id, Is.EqualTo("did:key:z6MkiTBz1ymuepAQ4HEHYSF1H8quG5GLVVQR3djdX3mDooWp#z6MkiTBz1ymuepAQ4HEHYSF1H8quG5GLVVQR3djdX3mDooWp"));
        Assert.That(didDocument.VerificationMethod.First().Type, Is.EqualTo("JsonWebKey2020"));
        Assert.That(didDocument.VerificationMethod.First().Controller, Is.EqualTo("did:key:z6MkiTBz1ymuepAQ4HEHYSF1H8quG5GLVVQR3djdX3mDooWp"));
        Assert.That(didDocument.VerificationMethod.First().PublicKeyJwk.Kty, Is.EqualTo("OKP"));
        Assert.That(didDocument.VerificationMethod.First().PublicKeyJwk.Crv, Is.EqualTo("Ed25519"));
        Assert.That(didDocument.VerificationMethod.First().PublicKeyJwk.X, Is.EqualTo("O2onvM62pC1io6jQKm8Nc2UyFXcd4kOmOsBIoYtZ2ik"));
        Assert.That(didDocument.Authentication[0].ToString(), Is.EqualTo("did:key:z6MkiTBz1ymuepAQ4HEHYSF1H8quG5GLVVQR3djdX3mDooWp#z6MkiTBz1ymuepAQ4HEHYSF1H8quG5GLVVQR3djdX3mDooWp"));
        Assert.That(didDocument.AssertionMethod[0].ToString(), Is.EqualTo("did:key:z6MkiTBz1ymuepAQ4HEHYSF1H8quG5GLVVQR3djdX3mDooWp#z6MkiTBz1ymuepAQ4HEHYSF1H8quG5GLVVQR3djdX3mDooWp"));
        Assert.That(didDocument.CapabilityDelegation[0].ToString(), Is.EqualTo("did:key:z6MkiTBz1ymuepAQ4HEHYSF1H8quG5GLVVQR3djdX3mDooWp#z6MkiTBz1ymuepAQ4HEHYSF1H8quG5GLVVQR3djdX3mDooWp"));
        Assert.That(didDocument.CapabilityInvocation[0].ToString(), Is.EqualTo("did:key:z6MkiTBz1ymuepAQ4HEHYSF1H8quG5GLVVQR3djdX3mDooWp#z6MkiTBz1ymuepAQ4HEHYSF1H8quG5GLVVQR3djdX3mDooWp"));
    }
}