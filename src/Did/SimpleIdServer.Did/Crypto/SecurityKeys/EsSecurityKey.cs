// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using SimpleIdServer.Did;
using SimpleIdServer.Did.Crypto;
using SimpleIdServer.Did.Crypto.SecurityKeys;
using System;
using System.Security.Cryptography;

namespace SimpleIdServer.Did.Crypto.SecurityKeys
{
    public class EsSecurityKey : AsymmetricSecurityKey
    {
        private X9ECParameters _curve;
        private ECPublicKeyParameters _publicKey;
        private ECPrivateKeyParameters _privateKey;

        public EsSecurityKey()
        {
            this.CryptoProviderFactory.CustomCryptoProvider = new EsCryptoProvider();
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

        public override bool CanComputeJwkThumbprint()
        {
            return true;
        }

        public override byte[] ComputeJwkThumbprint()
        {
            var q = _publicKey.Q;
            var x = Base64UrlEncoder.Encode(q.XCoord.GetEncoded());
            var y = Base64UrlEncoder.Encode(q.YCoord.GetEncoded());
            return GenerateSha256Hash($"{{\"{"crv"}\":\"{Constants.StandardCrvOrSize.P256}\",\"{"kty"}\":\"{"EC"}\",\"{"x"}\":\"{x}\",\"{"y"}\":\"{y}\"}}");
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

        private static byte[] GenerateSha256Hash(string input)
            => SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(input));
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
        var ecDsaSigner = new ECDsaSigner();
        ecDsaSigner.Init(true, _securityKey.PrivateKey);
        byte[] hashedInput;
        using (var hasher = SHA256.Create())
        {
            hashedInput = hasher.ComputeHash(input);
        }

        var output = ecDsaSigner.GenerateSignature(hashedInput);

        var r = output[0].ToByteArrayUnsigned();
        var s = output[1].ToByteArrayUnsigned();

        var signature = new byte[r.Length + s.Length];
        r.CopyTo(signature, 0);
        s.CopyTo(signature, r.Length);

        return signature;
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