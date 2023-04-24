// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using SimpleIdServer.Did.Crypto;

namespace SimpleIdServer.Did
{
    public class SignatureKeyBuilder
    {
        private SignatureKeyBuilder() { }

        /// <summary>
        /// Documentation: https://w3c-ccg.github.io/lds-ed25519-2018/
        /// </summary>
        /// <returns></returns>
        public static ISignatureKey NewES256K()
        {
            var secureRandom = new SecureRandom();
            var gen = new ECKeyPairGenerator("EC");
            var keyGenParam = new KeyGenerationParameters(secureRandom, 256);
            gen.Init(keyGenParam);
            var keyPair = gen.GenerateKeyPair();
            var privateBytes = ((ECPrivateKeyParameters)keyPair.Private).D.ToByteArrayUnsigned();
            if (privateBytes.Length != 32) return NewES256K();
            return new ES256KSignatureKey(null, privateBytes);
        }

        public static ISignatureKey NewED25519()
        {
            var gen = new Ed25519KeyPairGenerator();
            gen.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));
            var keyPair = gen.GenerateKeyPair();
            var sk = new byte[32];
            ((Ed25519PrivateKeyParameters)keyPair.Private).Encode(sk, 0);
            var pk = ((Ed25519PublicKeyParameters)keyPair.Public).GetEncoded();
            return new Ed25519SignatureKey(pk, sk);
        }
    }
}
