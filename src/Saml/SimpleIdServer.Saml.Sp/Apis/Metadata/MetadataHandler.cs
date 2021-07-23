// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.Saml.Builders;
using SimpleIdServer.Saml.Xsd;

namespace SimpleIdServer.Saml.Sp.Apis.Metadata
{
    public class MetadataHandler : IMetadataHandler
    {
        private readonly SamlSpOptions _options;

        public MetadataHandler(IOptions<SamlSpOptions> options)
        {
            _options = options.Value;
        }

        public EntityDescriptorType Get(string issuer)
        {
            return EntityDescriptorBuilder.Instance(_options.SPId)
                .AddSpSSODescriptor(cb =>
                {
                    cb.SetAuthnRequestsSigned(_options.AuthnRequestSigned);
                    cb.SetWantAssertionsSigned(_options.WantAssertionSigned);
                    cb.AddAssertionConsumerService(Constants.Bindings.HttpRedirect, $"{issuer}/{Constants.RouteNames.SingleSignOn}/AssertionConsumer");
                    if (_options.SigningCertificate != null)
                    {
                        cb.AddSigningKey(_options.SigningCertificate);
                    }
                }).Build();
        }
    }
}
