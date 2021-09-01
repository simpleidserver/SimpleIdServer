// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;

namespace SimpleIdServer.Saml.Helpers
{
    public static class MessageEncodingBuilder
    {
        public static string EncodeHTTPBindingRequest(Uri uri, XmlElement obj, string relayState, X509Certificate2 certificate, SignatureAlgorithms? sigAlg)
        {
            return EncodeHTTPBinding(uri, obj, "SAMLRequest", relayState, certificate, sigAlg);
        }

        public static string EncodeHTTPBindingResponse(Uri uri, XmlElement obj, string relayState, X509Certificate2 certificate, SignatureAlgorithms? sigAlg)
        {
            return EncodeHTTPBinding(uri, obj, "SAMLResponse", relayState, certificate, sigAlg);
        }

        public static string EncodeHTTPBinding(Uri uri, XmlElement obj, string samlName, string relayState, X509Certificate2 certificate, SignatureAlgorithms? sigAlg)
        {
            var dic = Encode(obj, samlName, relayState);
            if (sigAlg != null)
            {
                dic.Add("SigAlg", Constants.MappingDigestMethodToStr[sigAlg.Value]);
            }

            foreach (var kvp in dic)
            {
                uri = uri.AddParameter(kvp.Key, kvp.Value);
            }

            if (sigAlg != null)
            {
                var prk = certificate.PrivateKey as RSA;
                var hashed = Hash.Compute(uri.Query.TrimStart('?'), sigAlg.Value);
                var signed = prk.SignHash(hashed, Constants.MappingSignatureAlgToHash[sigAlg.Value], RSASignaturePadding.Pkcs1);
                uri = uri.AddParameter("Signature", Convert.ToBase64String(signed));
            }

            return uri.ToString();
        }

        public static Dictionary<string, string> Encode(XmlElement obj, string samlName, string relayState)
        {
            var xml = obj.OuterXml;
            var dic = new Dictionary<string, string>
            {
                { samlName, Compression.Compress(xml) },
                { "RelayState", relayState }
            };
            return dic;
        }
    }
}
