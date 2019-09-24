// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography;

namespace SimpleIdServer.Jwt.Jwe.EncHandlers
{
    public class A256CBCHS512EncHandler : AESCBCHSEncHandler
    {
        public override int KeyLength => 512;
        public override string EncName => ENC_NAME;
        public static string ENC_NAME = "A256CBC-HS512";

        public override HMAC GetHmac(byte[] key)
        {
            return new HMACSHA512(key);
        }
    }
}
