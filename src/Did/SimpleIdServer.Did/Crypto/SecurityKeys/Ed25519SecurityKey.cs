// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using System;

namespace SimpleIdServer.Did.Crypto.SecurityKeys;

public class Ed25519SecurityKey : AsymmetricSecurityKey
{
    private readonly Ed25519PublicKeyParameters _publicKey;
    private readonly Ed25519PrivateKeyParameters _privateKey;

    public Ed25519SecurityKey()
    {
        this.CryptoProviderFactory.CustomCryptoProvider = new Ed25519CryptoProvider();
    }

    public Ed25519SecurityKey(Ed25519PublicKeyParameters publicKey, Ed25519PrivateKeyParameters privateKey = null) : this()
    {
        if (publicKey == null) throw new ArgumentNullException(nameof(publicKey));
        _publicKey = publicKey;
        _privateKey = privateKey;
    }

    public Ed25519PublicKeyParameters PublicKey => _publicKey;
    public Ed25519PrivateKeyParameters PrivateKey => _privateKey;

    public override bool HasPrivateKey
    {
        get
        {
            return _privateKey != null;
        }
    }

    public override PrivateKeyStatus PrivateKeyStatus
    {
        get
        {
            return _privateKey == null ? PrivateKeyStatus.DoesNotExist : PrivateKeyStatus.Exists;
        }
    }

    public override int KeySize
    {
        get
        {
            return _publicKey.GetEncoded().Length;
        }
    }
}

public class Ed25519CryptoProvider : ICryptoProvider
{
    public object Create(string algorithm, params object[] args)
    {
        var securityKey = args[0] as Ed25519SecurityKey;
        return new Ed25519SignatureProvider(securityKey, algorithm);
    }

    public bool IsSupportedAlgorithm(string algorithm, params object[] args)
    {
        return algorithm == Constants.StandardJwtAlgs.EdDsa;
    }

    public void Release(object cryptoInstance) { }
}

public class Ed25519SignatureProvider : SignatureProvider
{
    private readonly Ed25519SecurityKey _securityKey;

    public Ed25519SignatureProvider(Ed25519SecurityKey securityKey, string algorithm) : base(securityKey, algorithm)
    {
        _securityKey = securityKey;
    }

    public override bool Sign(ReadOnlySpan<byte> data, Span<byte> destination, out int bytesWritten)
    {
        var result = Sign(data.ToArray());
        return Helpers.TryCopyToDestination(result, destination, out bytesWritten);
    }

    public override byte[] Sign(byte[] input)
    {
        var signer = new Ed25519Signer();
        signer.Init(true, _securityKey.PrivateKey);
        signer.BlockUpdate(input, 0, input.Length);
        return signer.GenerateSignature();
    }

    public override bool Verify(byte[] input, byte[] signature)
    {
        var verifier = new Ed25519Signer();
        verifier.Init(false, _securityKey.PublicKey);
        verifier.BlockUpdate(input, 0, input.Length);
        return verifier.VerifySignature(signature);
    }

    protected override void Dispose(bool disposing) { }
}
