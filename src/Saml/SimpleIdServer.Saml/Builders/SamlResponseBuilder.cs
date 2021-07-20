// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Xsd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Builders
{
    public class SamlResponseBuilder
    {
        private static SamlResponseBuilder _instance;
        private ResponseType _response;

        private SamlResponseBuilder()
        {
            _response = new ResponseType
            {
                ID = $"pfx_{Guid.NewGuid()}"
            };
        }

        public static SamlResponseBuilder New()
        {
            if (_instance == null)
            {
                _instance = new SamlResponseBuilder();
            }

            return _instance;
        }

        #region Actions

        /// <summary>
        /// Add an assertion.
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public SamlResponseBuilder AddAssertion(Action<AssertionBuilder> callback)
        {
            var assertion = new AssertionType
            {
                 Version = Constants.SamlVersion,
                 ID = $"assertion_{Guid.NewGuid()}",
                 IssueInstant = DateTime.UtcNow
            };
            var builder = new AssertionBuilder(assertion);
            callback(builder);
            AddAssertion(assertion);
            return this;
        }

        /// <summary>
        /// Sign and build.
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="digestMethod"></param>
        /// <param name="canonicalizationMethod"></param>
        /// <returns></returns>
        public ResponseType SignAndBuild(X509Certificate2 certificate, SignatureAlgorithms signatureAlgorithm, CanonicalizationMethods canonicalizationMethod)
        {
            foreach(var assertion in _response.Items.Where(i => i is AssertionType).Cast<AssertionType>())
            {
                var assertionSigned = new SamlSignedRequest(assertion.SerializeToXmlElement(), certificate, signatureAlgorithm, canonicalizationMethod);
                assertionSigned.ComputeSignature(assertion.ID);
                var assertionSignature = assertionSigned.GetXml().OuterXml.DeserializeXml<SignatureType>();
                assertion.Signature = assertionSignature;
            }

            var signedRequest = new SamlSignedRequest(_response.SerializeToXmlElement(), certificate, signatureAlgorithm, canonicalizationMethod);
            signedRequest.ComputeSignature(_response.ID);
            var signature = signedRequest.GetXml().OuterXml.DeserializeXml<SignatureType>();
            _response.Signature = signature;
            return _response;
        }

        public ResponseType Build()
        {
            return _response;
        }

        #endregion

        private void AddAssertion(AssertionType o)
        {
            var items = new List<object>();
            if (_response.Items != null)
            {
                items = _response.Items.ToList();
            }

            items.Add(o);
            _response.Items = items.ToArray();
        }
    }
}
