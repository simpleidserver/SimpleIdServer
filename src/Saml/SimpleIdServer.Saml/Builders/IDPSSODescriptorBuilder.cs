// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Saml.Extensions;
using SimpleIdServer.Saml.Xsd;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.Saml.Builders
{
    public class IDPSSODescriptorBuilder
    {
        private readonly IDPSSODescriptorType _idpSSODescriptorType;

        internal IDPSSODescriptorBuilder(IDPSSODescriptorType idpSSODescriptorType)
        {
            _idpSSODescriptorType = idpSSODescriptorType;
        }

        #region Actions

        /// <summary>
        /// Describe endpoint that support the profile of the Authentication Request protocol.
        /// </summary>
        /// <param name="location">Specifies the SAML binding supported by the endpoint.</param>
        /// <param name="binding">Specifies the location of the endpoint</param>
        /// <returns></returns>
        public IDPSSODescriptorBuilder AddSingleSignOnService(string location, string binding)
        {
            var endpoint = new EndpointType
            {
                Location = location,
                Binding = binding
            };
            _idpSSODescriptorType.SingleSignOnService = _idpSSODescriptorType.SingleSignOnService.Add(endpoint);
            return this;
        }

        /// <summary>
        /// Provides information about the cryptographic key that an entity uses to sign data.
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public IDPSSODescriptorBuilder AddSigningKey(X509Certificate2 certificate)
        {
            var keyDescriptor = new KeyDescriptorType
            {
                useSpecified = true,
                use = KeyTypes.signing,
                KeyInfo = KeyInfoBuilder.Build(certificate)
            };
            _idpSSODescriptorType.KeyDescriptor = _idpSSODescriptorType.KeyDescriptor.Add(keyDescriptor);
            return this;
        }


        /// <summary>
        /// Provides information about the cryptographic key that an entity uses to receive encrypted keys.
        /// </summary>
        /// <param name="certificate"></param>
        /// <returns></returns>
        public IDPSSODescriptorBuilder AddEncryptionKey(X509Certificate2 certificate)
        {
            var keyDescriptor = new KeyDescriptorType
            {
                useSpecified = true,
                use = KeyTypes.encryption,
                KeyInfo = KeyInfoBuilder.Build(certificate)
            };
            _idpSSODescriptorType.KeyDescriptor = _idpSSODescriptorType.KeyDescriptor.Add(keyDescriptor);
            return this;
        }

        #endregion
    }
}
