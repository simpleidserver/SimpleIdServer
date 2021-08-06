// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SimpleIdServer.Saml.Helpers
{
    public static class SignatureHelper
    {
        public static bool CheckSignature(XmlElement elt, X509Certificate2 certificate)
        {
            var doc = new XmlDocument();
            doc.LoadXml(elt.OuterXml);
            var sig = doc.SelectSingleNode("//*[local-name()='Signature']") as XmlElement;
            var signedXml = new SignedXml(doc);
            signedXml.LoadXml(sig);
            return signedXml.CheckSignature(certificate.GetRSAPublicKey());
        }
    }
}
