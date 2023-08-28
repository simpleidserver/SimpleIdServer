// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Saml.Idp.Factories;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Saml.Idp.Apis
{
    public class SamlMetadataController : Controller
    {
        private readonly ISaml2ConfigurationFactory _saml2ConfigurationFactory;
        private readonly SamlIdpOptions _options;

        public SamlMetadataController(ISaml2ConfigurationFactory saml2ConfigurationFactory, IOptions<SamlIdpOptions> options)
        {
            _saml2ConfigurationFactory = saml2ConfigurationFactory;
            _options = options.Value;
        }

        public IActionResult Get([FromRoute] string prefix)
        {
            prefix = prefix ?? IdServer.Constants.DefaultRealm;
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var configuration = _saml2ConfigurationFactory.BuildSamlIdpConfiguration(issuer, prefix);
            var entityDescriptor = new EntityDescriptor(configuration)
            {
                ValidUntil = 365
            };
            entityDescriptor.IdPSsoDescriptor = new IdPSsoDescriptor
            {
                WantAuthnRequestsSigned = configuration.SignAuthnRequest,
                SigningCertificates = new X509Certificate2[]
                {
                    configuration.SigningCertificate
                },
                SingleSignOnServices = new SingleSignOnService[]
                {
                    new SingleSignOnService { Binding = ProtocolBindings.HttpRedirect, Location = new Uri($"{issuer}/{prefix}/{Constants.RouteNames.SingleSignOnHttpRedirect}") }
                },
                SingleLogoutServices = new SingleLogoutService[]
                {
                    new SingleLogoutService { Binding = ProtocolBindings.HttpPost, Location = new Uri($"{issuer}/{prefix}/{Constants.RouteNames.SingleSignLogout}") }
                },
                ArtifactResolutionServices = new ArtifactResolutionService[]
                {
                    new ArtifactResolutionService { Binding = ProtocolBindings.ArtifactSoap, Index = 1, Location = new Uri($"{issuer}/{Constants.RouteNames.SingleSignOnArtifact}") }
                },
                NameIDFormats = new Uri[] { NameIdentifierFormats.Persistent }
            };
            entityDescriptor.ContactPersons = _options.ContactPersons;
            var metadata = new Saml2Metadata(entityDescriptor);
            var m = metadata.CreateMetadata();
            return new Saml2Metadata(entityDescriptor).CreateMetadata().ToActionResult();
        }
    }
}
