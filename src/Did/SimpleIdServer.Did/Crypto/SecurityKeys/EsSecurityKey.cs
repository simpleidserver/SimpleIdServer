// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.SecurityKeys;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Did.Crypto.SecurityKeys
{
    public class EsSecurityKey : AsymmetricSecurityKey
    {
        private X9ECParameters _curve;
        private ECPublicKeyParameters _publicKey;
        private ECPrivateKeyParameters _privateKey;

        public EsSecurityKey()
        {
            CryptoProviderFactory.CustomCryptoProvider = new EsCryptoProvider();
        }

        public EsSecurityKey(X9ECParameters curve, ECPublicKeyParameters publicKey, ECPrivateKeyParameters privateKey = null) : this()
        {
            if (curve == null) throw new ArgumentNullException(nameof(curve));
            if (publicKey == null) throw new ArgumentNullException(nameof(publicKey));
            _curve = curve;
            _publicKey = publicKey;
            _privateKey = privateKey;
        }

        public ECPublicKeyParameters PublicKey => _publicKey;
        public ECPrivateKeyParameters PrivateKey => _privateKey;

        public override bool HasPrivateKey
        {
            get
            {
                return _publicKey != null;
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
                var q = _publicKey.Q;
                q = q.Normalize();
                var publicKey = _curve.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger()).GetEncoded();
                return publicKey.Length;
            }
        }
    }
}

public class EsCryptoProvider : ICryptoProvider
{
    public object Create(string algorithm, params object[] args)
    {
        var securityKey = args[0] as EsSecurityKey;
        return new EsSignatureProvider(securityKey, algorithm);
    }

    public bool IsSupportedAlgorithm(string algorithm, params object[] args)
    {
        return algorithm == Constants.StandardJwtAlgs.Es256 ||
            algorithm == Constants.StandardJwtAlgs.Es384 ||
            algorithm == Constants.StandardJwtAlgs.Es256K;
    }

    public void Release(object cryptoInstance) { }
}

public class EsSignatureProvider : SignatureProvider
{
    private readonly EsSecurityKey _securityKey;

    public EsSignatureProvider(EsSecurityKey securityKey, string algorithm) : base(securityKey, algorithm)
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
        var signer = new DeterministicECDSA();
        signer.SetPrivateKey(_securityKey.PrivateKey);
        var sig = ECDSASignature.FromDER(signer.SignHash(input));
        var lst = new List<byte>();
        lst.AddRange(sig.R.ToByteArrayUnsigned());
        lst.AddRange(sig.S.ToByteArrayUnsigned());
        return lst.ToArray();
    }

    public override bool Verify(byte[] input, byte[] signature)
    {
        var sig = BaseESSignatureKey.ExtractSignature(signature);
        var signer = new ECDsaSigner();
        signer.Init(false, _securityKey.PublicKey);
        return signer.VerifySignature(input, sig.R, sig.S);
    }

    protected override void Dispose(bool disposing) { }
}