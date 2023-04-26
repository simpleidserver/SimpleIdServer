// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SimpleIdServer.Did.Crypto
{
    public abstract class BaseESSignatureKey : ISignatureKey
    {
        private ECPublicKeyParameters _publicKeyParameters;
        private readonly ECPrivateKeyParameters _privateKeyParameters;
        private readonly ECDomainParameters _domainParameters;
        private readonly X9ECParameters _secp256k1;

        public BaseESSignatureKey(byte[] publicKey, byte[] privateKey = null)
        {
            _secp256k1 = SecNamedCurves.GetByName(CurveName);
            _domainParameters = new ECDomainParameters(_secp256k1.Curve, _secp256k1.G, _secp256k1.N, _secp256k1.H);
            if (publicKey != null)
            {
                var q = _secp256k1.Curve.DecodePoint(publicKey);
                _publicKeyParameters = new ECPublicKeyParameters("EC", q, _domainParameters);
            }

            if (privateKey != null) _privateKeyParameters = new ECPrivateKeyParameters(new Org.BouncyCastle.Math.BigInteger(1, privateKey), _domainParameters);
        }

        public abstract string Name { get; }
        public abstract string CurveName { get; }

        public byte[] PrivateKey
        {
            get
            {
                if (_privateKeyParameters == null) return null;
                return _privateKeyParameters.D.ToByteArrayUnsigned();
            }
        }

        public ECPublicKeyParameters PublicKey
        {
            get
            {
                if (_publicKeyParameters == null)
                {
                    var q = _secp256k1.G.Multiply(_privateKeyParameters.D);
                    _publicKeyParameters = new ECPublicKeyParameters("EC", q, _domainParameters);
                }

                return _publicKeyParameters;
            }
        }

        public byte[] GetPubKey()
        {
            var q = PublicKey.Q;
            q = q.Normalize();
            var publicKey = _secp256k1.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger()).GetEncoded(false);
            return publicKey;
        }

        public bool Check(string content, string signature) => Check(System.Text.Encoding.UTF8.GetBytes(content), Base64UrlEncoder.DecodeBytes(signature));

        public bool Check(byte[] content, byte[] signature)
        {
            var sig = ExtractSignature(signature);
            var signer = new ECDsaSigner();
            var hash = Hash(content);
            signer.Init(false, PublicKey);
            return signer.VerifySignature(hash, sig.R, sig.S);
        }

        public string Sign(string content)
        {
            var payload = System.Text.Encoding.UTF8.GetBytes(content);
            return Sign(payload);
        }

        public string Sign(byte[] content)
        {
            if (_privateKeyParameters == null) throw new InvalidOperationException("There is no private key");
            var hash = Hash(content);
            var signer = new DeterministicECDSA();
            signer.SetPrivateKey(_privateKeyParameters);
            var sig = ECDSASignature.FromDER(signer.SignHash(hash));
            var lst = new List<byte>();
            lst.AddRange(sig.R.ToByteArrayUnsigned());
            lst.AddRange(sig.S.ToByteArrayUnsigned());
            return Base64UrlEncoder.Encode(lst.ToArray());
        }

        private static byte[] Hash(string content) => Hash(System.Text.Encoding.UTF8.GetBytes(content));

        private static byte[] Hash(byte[] payload)
        {
            byte[] result = null;
            using (var sha256 = SHA256.Create())
                result = sha256.ComputeHash(payload);

            return result;
        }

        private static ECDSASignature ExtractSignature(string signature)
        {
            var payload = Base64UrlEncoder.DecodeBytes(signature);
            return ExtractSignature(payload);
        }

        private static ECDSASignature ExtractSignature(byte[] payload)
        {
            byte? v = null;
            if (payload.Length > 64)
            {
                v = payload[64];
                if (v == 0 || v == 1)
                    v = (byte)(v + 27);
            }

            var r = new byte[32];
            Array.Copy(payload, r, 32);
            var s = new byte[32];
            Array.Copy(payload, 32, s, 0, 32);
            var result = new ECDSASignature(new BigInteger(1, r), new BigInteger(1, s));
            if (v != null) result.V = new byte[] { v.Value };
            return result;
        }

        public IdentityDocumentVerificationMethod ExtractVerificationMethodWithPublicKey()
        {
            return new IdentityDocumentVerificationMethod
            {
                PublicKeyHex = GetPubKey().ToHex()
            };
        }
    }
}
