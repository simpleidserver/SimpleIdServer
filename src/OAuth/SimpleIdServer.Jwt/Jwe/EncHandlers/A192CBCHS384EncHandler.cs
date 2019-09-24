// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography;

namespace SimpleIdServer.Jwt.Jwe.EncHandlers
{
    public class A192CBCHS384EncHandler : AESCBCHSEncHandler
    {
        public override int KeyLength => 384;
        public override string EncName => ENC_NAME;
        public static string ENC_NAME = "A192CBC-HS384";

        public override HMAC GetHmac(byte[] key)
        {
            return new HMACSHA384(key);
        }
    }
}
