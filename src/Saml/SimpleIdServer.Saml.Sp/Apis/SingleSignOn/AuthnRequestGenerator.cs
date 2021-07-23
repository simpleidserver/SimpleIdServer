// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.Saml.Builders;
using SimpleIdServer.Saml.Xsd;
using System.Xml;

namespace SimpleIdServer.Saml.Sp.Apis.SingleSignOn
{
    public class AuthnRequestGenerator : IAuthnRequestGenerator
    {
        private readonly SamlSpOptions _options;

        public AuthnRequestGenerator(IOptions<SamlSpOptions> options)
        {
            _options = options.Value;
        }

        public virtual XmlElement BuildHttpGetBinding()
        {
            return Build(Constants.Bindings.HttpRedirect);
        }

        protected virtual XmlElement Build(string binding)
        {
            var request = AuthnRequestBuilder.New(_options.SPName)
                .SetBinding(binding)
                .SetIssuer(Constants.NameIdentifierFormats.EntityIdentifier, _options.SPId);
            if (_options.AuthnRequestSigned && _options.SignatureAlg != null && _options.SigningCertificate != null)
            {
                return request.SignAndBuild(_options.SigningCertificate, _options.SignatureAlg.Value, _options.CanonicalizationMethod);
            }

            return request.Build();
        }
    }
}
