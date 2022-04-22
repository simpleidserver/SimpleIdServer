// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Options;
using SimpleIdServer.Saml.Builders;
using SimpleIdServer.Saml.Idp.Apis.SSO;
using SimpleIdServer.Saml.Xsd;
using System.Collections.Generic;

namespace SimpleIdServer.Saml.Idp.Apis.Metadata
{
    public class MetadataHandler : IMetadataHandler
    {
        private readonly SamlIdpOptions _options;
        private readonly IEnumerable<IResponseBuilder> _responseBuilders;

        public MetadataHandler(IOptions<SamlIdpOptions> options, IEnumerable<IResponseBuilder> responseBuilder)
        {
            _options = options.Value;
            _responseBuilders = responseBuilder;
        }

        public EntityDescriptorType Get()
        {
            return EntityDescriptorBuilder.Instance(_options.IDPId)
                .AddIdpSSODescriptor(cb =>
                {
                    foreach(var responseBuilder in _responseBuilders)
                    {
                        cb.AddSingleSignOnService($"{_options.BaseUrl}/{Constants.RouteNames.SingleSignOn}/Login", responseBuilder.Binding, Constants.Protocols.Saml2Protocol);
                    }

                    cb.AddSigningKey(_options.SigningCertificate);
                }).Build();
        }
    }
}
