// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace SimpleIdServer.CredentialIssuer;

public static class Constants
{
    public static class EndPoints
    {
        public const string CredentialIssuer = ".well-known/openid-credential-issuer";
        public const string Credential = "credential";
        public const string CredentialOffer = "credential_offer";
        public const string CredentialConfigurations = "credential_configurations";
        public const string CredentialInstances = "credential_instances";
    }

    public static List<string> AllEncAlgs = new List<string>
    {
        SecurityAlgorithms.RsaPKCS1,
        SecurityAlgorithms.RsaOAEP,
    };

    public static ICollection<string> AllEncryptions = new List<string>
    {
        SecurityAlgorithms.Aes128CbcHmacSha256,
        SecurityAlgorithms.Aes192CbcHmacSha384,
        SecurityAlgorithms.Aes256CbcHmacSha512

    };
}
