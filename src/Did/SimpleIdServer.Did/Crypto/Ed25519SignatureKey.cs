// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Security;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Models;
using System;

namespace SimpleIdServer.Did.Crypto
{
    public class Ed25519SignatureKey : ISignatureKey
    {
        private Ed25519PublicKeyParameters _publicKey;
        private Ed25519PrivateKeyParameters _privateKey;

        private Ed25519SignatureKey(Ed25519PublicKeyParameters publicKey, Ed25519PrivateKeyParameters privateKey)
        {
            _publicKey = publicKey;
            _privateKey = privateKey;
        }

        public string Name => Constants.SupportedSignatureKeyAlgs.Ed25519;

        public static Ed25519SignatureKey New()
        {
            var gen = new Ed25519KeyPairGenerator();
            gen.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
            var keyPair = gen.GenerateKeyPair();
            return new Ed25519SignatureKey((Ed25519PublicKeyParameters)keyPair.Public, (Ed25519PrivateKeyParameters)keyPair.Private);
        }

        public byte[] PrivateKey
        {
            get
            {
                if (_privateKey == null) return null;
                return _privateKey.GetEncoded();
            }
        }

        public byte[] GetPublicKey(bool compressed = false)
        {
            if (_publicKey == null) return null;
            return _publicKey.GetEncoded();
        }

        public JsonWebKey GetPublicKeyJwk()
        {
            var result = new JsonWebKey();
            result.Kty = "OKP";
            result.Crv = "Ed25519";
            result.X = Base64UrlEncoder.Encode(_publicKey.GetEncoded());
            return result;
        }

        public bool Check(string content, string signature) => Check(System.Text.Encoding.UTF8.GetBytes(content), Base64UrlEncoder.DecodeBytes(signature));

        public bool Check(byte[] payload, byte[] signaturePayload)
        {
            if (_publicKey == null) throw new InvalidOperationException("There is no public key");
            var verifier = new Ed25519Signer();
            verifier.Init(false, _publicKey);
            verifier.BlockUpdate(payload, 0, payload.Length);
            return verifier.VerifySignature(signaturePayload);
        }

        public string Sign(string content) => Sign(System.Text.Encoding.UTF8.GetBytes(content));

        public string Sign(byte[] payload)
        {
            if (_privateKey == null) throw new InvalidOperationException("There is no private key");
            var signer = new Ed25519Signer();
            signer.Init(true, _privateKey);
            signer.BlockUpdate(payload, 0, payload.Length);
            return Base64UrlEncoder.Encode(signer.GenerateSignature());
        }

        public IdentityDocumentVerificationMethod ExtractVerificationMethodWithPublicKey()
        {
            return new IdentityDocumentVerificationMethod
            {
                PublicKeyHex = _publicKey.GetEncoded().ToHex()
            };
        }
    }
}
