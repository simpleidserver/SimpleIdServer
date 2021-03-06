﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.OAuth
{
    public static class Constants
    {
        public static class EndPoints
        {
            public const string Token = "token";
            public const string Jwks = "jwks";
            public const string Authorization = "authorization";
            public const string Registration = "register";
            public const string OAuthConfiguration = ".well-known/oauth-authorization-server";
            public const string Form = "form";
            public const string Management = "management";
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

        public static string AuthenticationScheme = "SimpleIdServerOAuth";
        public static string AuthorizationHeaderName = "Authorization";
    }
}