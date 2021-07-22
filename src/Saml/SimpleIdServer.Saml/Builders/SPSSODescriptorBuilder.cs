// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Xsd;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Builders
{
    public class SPSSODescriptorBuilder
    {
        private readonly SPSSODescriptorType _spSSODescriptorType;

        internal SPSSODescriptorBuilder(SPSSODescriptorType spSSODescriptorType)
        {
            _spSSODescriptorType = spSSODescriptorType;
        }

        #region Actions

        /// <summary>
        /// Indicates whether the AuthnRequest messages sent by this service provider will be signed.
        /// </summary>
        /// <param name="authnRequestsSigned"></param>
        /// <returns></returns>
        public SPSSODescriptorBuilder SetAuthnRequestsSigned(bool authnRequestsSigned)
        {
            _spSSODescriptorType.AuthnRequestsSigned = authnRequestsSigned;
            _spSSODescriptorType.AuthnRequestsSignedSpecified = true;
            return this;
        }

        /// <summary>
        /// Want assertion signed.
        /// </summary>
        /// <param name="wantAssertionsSigned"></param>
        /// <returns></returns>
        public SPSSODescriptorBuilder SetWantAssertionsSigned(bool wantAssertionsSigned)
        {
            _spSSODescriptorType.WantAssertionsSigned = wantAssertionsSigned;
            _spSSODescriptorType.WantAssertionsSignedSpecified = true;
            return this;
        }

        /// <summary>
        /// Location to which the IdP will eventually send the user at the SP.
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public SPSSODescriptorBuilder AddAssertionConsumerService(string binding, string location)
        {
            var assertionConsumer = new IndexedEndpointType
            {
                Binding = binding,
                Location = location
            };
            _spSSODescriptorType.AssertionConsumerService = _spSSODescriptorType.AssertionConsumerService.Add(assertionConsumer);
            return this;
        }


        /// <summary>
        /// Provides information about the cryptographic key that an entity uses to sign data.
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public SPSSODescriptorBuilder AddSigningKey(X509Certificate2 certificate)
        {
            var keyDescriptor = new KeyDescriptorType
            {
                useSpecified = true,
                use = KeyTypes.signing,
                KeyInfo = KeyInfoBuilder.Build(certificate)
            };
            _spSSODescriptorType.KeyDescriptor = _spSSODescriptorType.KeyDescriptor.Add(keyDescriptor);
            return this;
        }

        #endregion
    }
}
