// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Builders;
using System.Collections.Generic;
using System.Security.Cryptography.Xml;

namespace SimpleIdServer.Saml
{
    public static class Constants
    {
        public const string SamlVersion = "2.0";

        public static class ConfirmationMethodIdentifiers
        {
            public const string HolderOfKey = "urn:oasis:names:tc:SAML:2.0:cm:holder-of-key";
        }

        public static class NameIdentifierFormats
        {
            public const string Unspecified = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";
            public const string EmailAddress = "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress";
            public const string X509SubjectName = "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName";
            public const string WindowsDomainQualifiedName = "urn:oasis:names:tc:SAML:1.1:nameid-format:WindowsDomainQualifiedName";
            public const string KerberosPrincipalName = ": urn:oasis:names:tc:SAML:2.0:nameid-format:kerberos";
            public const string EntityIdentifier = "urn:oasis:names:tc:SAML:2.0:nameid-format:entity";
            public const string PersistentIdentifier = "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent";
            public const string TransientIdentifier = " urn:oasis:names:tc:SAML:2.0:nameid-format:transient";
        }

        public static class AuthnContextClassReferences
        {
            public const string Password = "urn:oasis:names:tc:SAML:2.0:ac:classes:Password";
            public const string PasswordProtectedTransport = "urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport";
            public const string TLSClient = "urn:oasis:names:tc:SAML:2.0:ac:classes:TLSClient";
            public const string X509Certificate = "urn:oasis:names:tc:SAML:2.0:ac:classes:X509";
            public const string AuthenticationWindows = "urn:federation:authentication:windows";
            public const string Kerberos = "urn:oasis:names:tc:SAML:2.0:ac:classes:Kerberos";
        }

        public static class SignatureAlgorithms
        {
            public const string RSASHA1 = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
            public const string RSASHA256 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        }

        public static class Digests
        {
            public const string SHA1 = "http://www.w3.org/2000/09/xmldsig#sha1";
            public const string SHA256 = "http://www.w3.org/2000/09/xmldsig#sha1";
        }

        public static class CanonicalizationMethods
        {
            public const string ExclusiveCanonicalXml = SignedXml.XmlDsigExcC14NTransformUrl;
            public const string ExclusiveCanonicalXmlWithComments = SignedXml.XmlDsigExcC14NWithCommentsTransformUrl;
        }

        public static class StatusCodes
        {
            public const string Success = "urn:oasis:names:tc:SAML:2.0:status:Success";
            public const string Requester = "urn:oasis:names:tc:SAML:2.0:status:Requester";
            public const string VersionMismatch = "urn:oasis:names:tc:SAML:2.0:status:VersionMismatch";
            public const string RequestUnsupported = "urn:oasis:names:tc:SAML:2.0:status:RequestUnsupported";
            public const string InvalidNameIDPolicy = "urn:oasis:names:tc:SAML:2.0:status:InvalidNameIDPolicy";
        }

        public static Dictionary<Saml.SignatureAlgorithms, string> MappingDigestMethodToStr = new Dictionary<Saml.SignatureAlgorithms, string>
        {
            { Saml.SignatureAlgorithms.RSASHA1, SignatureAlgorithms.RSASHA1 },
            { Saml.SignatureAlgorithms.RSASHA256, SignatureAlgorithms.RSASHA256 }
        };

        public static Dictionary<Saml.CanonicalizationMethods, string> MappingCanonicalizationMethodToStr = new Dictionary<Saml.CanonicalizationMethods, string>
        {
            { Saml.CanonicalizationMethods.C14, CanonicalizationMethods.ExclusiveCanonicalXml },
            { Saml.CanonicalizationMethods.C14WithComments, CanonicalizationMethods.ExclusiveCanonicalXmlWithComments }
        };

        public static Dictionary<string, string> MappingSignatureToDigest = new Dictionary<string, string>
        {
            { SignatureAlgorithms.RSASHA1, Digests.SHA1 },
            { SignatureAlgorithms.RSASHA256, Digests.SHA256 }
        };
    }
}
