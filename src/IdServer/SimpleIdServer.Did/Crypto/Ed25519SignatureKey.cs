// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using SimpleIdServer.Did.Extensions;
using SimpleIdServer.Did.Models;
using System;
using System.Linq;
using System.Text;

namespace SimpleIdServer.Did.Crypto
{
    public class Ed25519SignatureKey : ISignatureKey
    {
        private Ed25519PublicKeyParameters _publicKey;
        private Ed25519PrivateKeyParameters _privateKey;

        public Ed25519SignatureKey(byte[] publicKey, byte[] privateKey)
        {
            if (publicKey != null)
            {
                if (publicKey.Length != 32) throw new InvalidOperationException("Public key must have 32 bytes");
                _publicKey = new Ed25519PublicKeyParameters(publicKey);
            }

            if (privateKey != null)
            {
                if (privateKey.Length != 64 && privateKey.Length != 32) throw new InvalidOperationException("Private key must have 64 or 32 bytes");
                _privateKey = new Ed25519PrivateKeyParameters(privateKey.Take(32).ToArray());
                if (privateKey.Length == 64) _publicKey = new Ed25519PublicKeyParameters(privateKey.Skip(32).ToArray());
            }
        }

        public string Name => Constants.SupportedSignatureKeyAlgs.Ed25519;

        public bool Check(string content, string signature)
        {
            if (_publicKey == null) throw new InvalidOperationException("There is no public key");
            var payload = Encoding.UTF8.GetBytes(content);
            var verifier = new Ed25519Signer();
            verifier.Init(false, _publicKey);
            verifier.BlockUpdate(payload, 0, payload.Length);
            var signaturePayload = Base64UrlEncoder.DecodeBytes(signature);
            return verifier.VerifySignature(signaturePayload);
        }

        public string Sign(string content)
        {
            if (_privateKey == null) throw new InvalidOperationException("There is no private key");
            var payload = Encoding.UTF8.GetBytes(content);
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
