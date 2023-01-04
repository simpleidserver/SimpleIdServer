// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Security.Cryptography;
using System.Text;

namespace SimpleIdServer.IdServer.Api.Token.PKCECodeChallengeMethods
{
    public class S256CodeChallengeMethodHandler : ICodeChallengeMethodHandler
    {
        public static string DEFAULT_NAME = "S256";

        public string Name => DEFAULT_NAME;

        public string Calculate(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
                return sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier)).Base64EncodeBytes();
        }
    }
}
