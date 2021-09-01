// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.Saml.Builders;
using SimpleIdServer.Saml.Xsd;

namespace SimpleIdServer.Saml.Idp.Apis.Metadata
{
    public class MetadataHandler : IMetadataHandler
    {
        private readonly SamlIdpOptions _options;

        public MetadataHandler(IOptions<SamlIdpOptions> options)
        {
            _options = options.Value;
        }

        public EntityDescriptorType Get()
        {
            return EntityDescriptorBuilder.Instance(_options.IDPId)
                .AddIdpSSODescriptor(cb =>
                {
                    cb.AddSingleSignOnService($"{_options.BaseUrl}/{Constants.RouteNames.SingleSignOn}/Login", Saml.Constants.Bindings.HttpRedirect);
                    cb.AddSigningKey(_options.SigningCertificate);
                }).Build();
        }
    }
}
