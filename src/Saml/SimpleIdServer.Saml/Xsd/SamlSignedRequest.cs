// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace SimpleIdServer.Saml.Xsd
{
    public class SamlSignedRequest : SignedXml
    {
        private readonly XmlElement _xmlElement;
        private readonly X509Certificate2 _certificate;
        private readonly SignatureAlgorithms _signatureAlgorithm;
        private readonly CanonicalizationMethods _canonicalizationMethod;

        public SamlSignedRequest() { }

        internal SamlSignedRequest(XmlElement xmlElement, X509Certificate2 certificate)
        {
            _xmlElement = xmlElement;
            _certificate = certificate;
        }

        internal SamlSignedRequest(XmlElement xmlElement, X509Certificate2 certificate, SignatureAlgorithms signatureAlgorithm, CanonicalizationMethods canonicalizationMethod): base(xmlElement)
        {
            _xmlElement = xmlElement;
            _certificate = certificate;
            _signatureAlgorithm = signatureAlgorithm;
            _canonicalizationMethod = canonicalizationMethod;
        }

        #region Actions

        public void ComputeSignature(string id)
        {
            var signatureMethod = Constants.MappingDigestMethodToStr[_signatureAlgorithm];
            var reference = new Reference($"#{id}");
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.DigestMethod = Constants.MappingSignatureToDigest[signatureMethod];
            reference.AddTransform(BuildC14(_canonicalizationMethod));
            SignedInfo.CanonicalizationMethod = Constants.MappingCanonicalizationMethodToStr[_canonicalizationMethod];
            AddReference(reference);
            SignedInfo.SignatureMethod = signatureMethod;
            SigningKey = _certificate.GetRSAPrivateKey();
            base.ComputeSignature();
            KeyInfo = new KeyInfo();
            KeyInfo.AddClause(new KeyInfoX509Data(_certificate));
        }

        #endregion

        private static XmlDsigExcC14NTransform BuildC14(CanonicalizationMethods canonicalizationMethod)
        {
            switch (canonicalizationMethod)
            {
                case CanonicalizationMethods.C14WithComments:
                    return new XmlDsigExcC14NWithCommentsTransform();
                default:
                    return new XmlDsigExcC14NTransform();
            }
        }
    }
}
