// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.IdServer.WsFederation
{
    public class WsFederationConstants
    {
        public static class EndPoints
        {
            public const string FederationMetadata = "FederationMetadata/2007-06/FederationMetadata.xml";
            public const string SSO = "SSO";
        }

        public static class SamlNameIdentifierFormats
        {
            public const string UnspecifiedString = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";
        }

        public static class TokenTypes
        {
            public const string Saml11TokenProfile11 = "urn:oasis:names:tc:SAML:1.0:assertion";
            public const string Saml2TokenProfile11 = "urn:oasis:names:tc:SAML:2.0:assertion";
        }
    }
}
