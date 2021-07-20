// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Xsd;
using System;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Builders
{
    public class AuthnRequestBuilder
    {
        private static AuthnRequestBuilder _instance;
        private AuthnRequestType _authRequest;

        private AuthnRequestBuilder(string providerName)
        {
            _authRequest = new AuthnRequestType
            {
                ID = $"pfx{Guid.NewGuid().ToString()}",
                Version = Constants.SamlVersion,
                IssueInstant = DateTime.UtcNow,
                ProviderName = providerName
            };
        }

        public static AuthnRequestBuilder New(string providerName)
        {
            if (_instance == null)
            {
                _instance = new AuthnRequestBuilder(providerName);
            }

            return _instance;
        }

        #region Actions

        /// <summary>
        /// Set the issuer.
        /// </summary>
        /// <param name="format">A URI reference representing the classification of string-based identifier information. If no value is specified then the value "nameid-format-unspecified" is in effect.</param>
        /// <param name="value">Value of the name</param>
        /// <param name="spProviderId">Name Identifier established by a service provider or affiliation providers for the entity.</param>
        /// <param name="nameQualifier">The security or administrative domain that qualifies the name.</param>
        /// <param name="spNameQualifier">Further qualifies a name with the name of the service provider or affiliaton provider.</param>
        /// <returns></returns>
        public AuthnRequestBuilder SetIssuer(string format, string value, string spProviderId = null, string nameQualifier = null, string spNameQualifier = null)
        {
            _authRequest.Issuer = new NameIDType
            {
                Format = format,
                Value = value,
                SPProvidedID = spProviderId,
                NameQualifier = nameQualifier,
                SPNameQualifier = spNameQualifier
            };
            return this;
        }

        /// <summary>
        /// Specifies the requested subject of the resulting assertion(s).
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public AuthnRequestBuilder SetSubject(Action<SubjectBuilder> callback)
        {
            var subject = new SubjectType();
            var builder = new SubjectBuilder(subject);
            callback(builder);
            _authRequest.Subject = subject;
            return this;
        }

        /// <summary>
        /// Specifies constraints on the name identifier to be used to represent the requested subject.
        /// </summary>
        /// <param name="format">Name identifier format in this or another specification.</param>
        /// <param name="nameQualifier">Assertion subject's identifier be returned (or created)</param>
        /// <param name="allowCreate">Indicate whether the identity provider is allowed to crete a new identifier to represent the principal.</param>
        /// <returns></returns>
        public AuthnRequestBuilder SetNameIDPolicy(string format, string nameQualifier, bool allowCreate = false)
        {
            _authRequest.NameIDPolicy = new NameIDPolicyType
            {
                AllowCreate = allowCreate,
                SPNameQualifier = nameQualifier,
                Format = format,
                AllowCreateSpecified = true
            };
            return this;
        }

        /// <summary>
        /// Authentication context requirements of authentication statements returned in response to a request or query.
        /// </summary>
        /// <param name="comparison">Comparison method used to evaluate the requested context classes or statements.</param>
        /// <returns></returns>
        public AuthnRequestBuilder AddAuthnContextClassPassword(AuthnContextComparisonType? comparison = null)
        {
            _authRequest.RequestedAuthnContext = new RequestedAuthnContextType
            {
                Items = new []
                {
                    Constants.AuthnContextClassReferences.Password
                },
                ItemsElementName = new ItemsChoiceType7[]
                {
                    ItemsChoiceType7.AuthnContextClassRef
                }
            };
            if(comparison != null)
            {
                _authRequest.RequestedAuthnContext.ComparisonSpecified = true;
                _authRequest.RequestedAuthnContext.Comparison = comparison.Value;
            }

            return this;
        }

        /// <summary>
        /// Sign and build.
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="digestMethod"></param>
        /// <param name="canonicalizationMethod"></param>
        /// <returns></returns>
        public AuthnRequestType SignAndBuild(X509Certificate2 certificate, SignatureAlgorithms signatureAlgorithm, CanonicalizationMethods canonicalizationMethod)
        {
            var signedRequest = new SamlSignedRequest(_authRequest.SerializeToXmlElement(), certificate, signatureAlgorithm, canonicalizationMethod);
            signedRequest.ComputeSignature(_authRequest.ID);
            var signature = signedRequest.GetXml().OuterXml.DeserializeXml<SignatureType>();
            _authRequest.Signature = signature;
            return _authRequest;
        }

        public AuthnRequestType Build()
        {
            return _authRequest;
        }

        #endregion
    }
}
