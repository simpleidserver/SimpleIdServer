// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Xsd;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace SimpleIdServer.Saml.Helpers
{
    public static class SignatureHelper
    {
        public static bool CheckSignature(XmlElement elt, X509Certificate2 certificate)
        {
            var signedRequest = new SamlSignedRequest(elt);
            var sig = elt.GetElementsByTagName("Signature", "http://www.w3.org/2000/09/xmldsig#");
            signedRequest.LoadXml((XmlElement)sig[0]);
            return signedRequest.CheckSignature(certificate.GetRSAPublicKey());
        }
    }
}
