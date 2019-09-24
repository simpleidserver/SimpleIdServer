// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography;

namespace SimpleIdServer.Jwt.Jwe.EncHandlers
{
    public class A128CBCHS256EncHandler : AESCBCHSEncHandler
    {
        public override int KeyLength => 256;
        public override string EncName => ENC_NAME;
        public static string ENC_NAME = "A128CBC-HS256";

        public override HMAC GetHmac(byte[] key)
        {
            return new HMACSHA256(key);
        }
    }
}
