// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace SimpleIdServer.OAuth
{
    public static class Constants
    {
        public static class EndPoints
        {
            public const string Token = "token";
            public const string TokenRevoke = "token/revoke";
            public const string TokenInfo = "token_info";
            public const string Jwks = "jwks";
            public const string Authorization = "authorization";
            public const string Registration = "register";
            public const string OAuthConfiguration = ".well-known/oauth-authorization-server";
            public const string Form = "form";
            public const string ClientManagement = "management/clients";
            public const string MtlsPrefix = "mtls";
            public const string MtlsToken = MtlsPrefix + "/" + Token;
        }

        public static class ScopeNames
        {
            public const string Register = "register";
        }

        public static class CertificateOIDS
        {
            public const string SubjectAlternativeName = "2.5.29.17";
        }

        public class Policies
        {
            public const string Register = "register";
        }

        public static ICollection<string> AllSigningAlgs = new List<string>
        {
            SecurityAlgorithms.EcdsaSha256,
            SecurityAlgorithms.EcdsaSha384,
            SecurityAlgorithms.EcdsaSha512,
            SecurityAlgorithms.HmacSha256,
            SecurityAlgorithms.HmacSha384,
            SecurityAlgorithms.HmacSha512,
            SecurityAlgorithms.RsaSsaPssSha256,
            SecurityAlgorithms.RsaSsaPssSha384,
            SecurityAlgorithms.RsaSsaPssSha512,
            SecurityAlgorithms.RsaSha256,
            SecurityAlgorithms.RsaSha384,
            SecurityAlgorithms.RsaSha512,
            SecurityAlgorithms.None
        };

        public static ICollection<string> AllEncAlgs = new List<string>
        {
            SecurityAlgorithms.RsaPKCS1,
            SecurityAlgorithms.RsaOAEP,
            SecurityAlgorithms.Aes128KW,
            SecurityAlgorithms.Aes192KW,
            SecurityAlgorithms.Aes256KW,
            SecurityAlgorithms.EcdhEs,
            SecurityAlgorithms.EcdhEsA128kw,
            SecurityAlgorithms.EcdhEsA192kw,
            SecurityAlgorithms.EcdhEsA256kw
        };

        public static ICollection<string> AllEncryptions = new List<string>
        {
            SecurityAlgorithms.Aes128CbcHmacSha256,
            SecurityAlgorithms.Aes192CbcHmacSha384,
            SecurityAlgorithms.Aes256CbcHmacSha512,
            SecurityAlgorithms.Aes128Gcm,
            SecurityAlgorithms.Aes192Gcm,
            SecurityAlgorithms.Aes256Gcm
        };

        public static string AuthenticationScheme = "SimpleIdServerOAuth";
        public static string AuthorizationHeaderName = "Authorization";
        public const string CertificateAuthenticationScheme = "Certificate";
        public const string Prefix = "prefix";
        /// <summary>
        /// Direct use of a shared symmetric key as the CEK.
        /// </summary>
        public const string AlgDir = "dir";
    }
}