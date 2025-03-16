// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Config;

public static class DefaultTokenSecurityAlgs
{
    public static class JwkUsages
    {
        public const string Enc = "enc";
        public const string Sig = "sig";
    }


    public static ICollection<string> AllSigAlgs = new List<string>
    {
        SecurityAlgorithms.RsaSha256,
        SecurityAlgorithms.RsaSha384,
        SecurityAlgorithms.RsaSha512,
        SecurityAlgorithms.EcdsaSha256,
        SecurityAlgorithms.EcdsaSha384,
        SecurityAlgorithms.EcdsaSha512,
        SecurityAlgorithms.None
    };

    public static ICollection<string> AllEncAlgs = new List<string>
    {
        SecurityAlgorithms.RsaPKCS1,
        SecurityAlgorithms.RsaOAEP
    };

    public static ICollection<string> AllEncs = new List<string>
    {
        SecurityAlgorithms.Aes128CbcHmacSha256,
        SecurityAlgorithms.Aes192CbcHmacSha384,
        SecurityAlgorithms.Aes256CbcHmacSha512
    };
}
