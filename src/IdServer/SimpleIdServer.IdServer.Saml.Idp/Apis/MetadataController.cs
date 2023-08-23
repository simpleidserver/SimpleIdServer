// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Saml.Idp.Apis
{
    public class MetadataController : Controller
    {
        private readonly Saml2Configuration _configuration;

        public MetadataController(IOptions<Saml2Configuration> configuration)
        {
            _configuration = configuration.Value;
        }

        public IActionResult Get([FromRoute] string prefix)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var entityDescriptor = new EntityDescriptor(_configuration)
            {
                ValidUntil = 365
            };
            entityDescriptor.IdPSsoDescriptor = new IdPSsoDescriptor
            {
                WantAuthnRequestsSigned = _configuration.SignAuthnRequest,
                SigningCertificates = new X509Certificate2[]
                {
                    _configuration.SigningCertificate
                },
                SingleSignOnServices = new SingleSignOnService[]
                {
                    new SingleSignOnService { Binding = ProtocolBindings.HttpRedirect, Location = new Uri($"{issuer}/{Saml.Idp.Constants.RouteNames.SingleSignOnHttpRedirect}") }
                },
                SingleLogoutServices = new SingleLogoutService[]
                {
                    new SingleLogoutService { Binding = ProtocolBindings.HttpPost, Location = new Uri($"{issuer}/{Saml.Idp.Constants.RouteNames.SingleSignLogout}") }
                },
                ArtifactResolutionServices = new ArtifactResolutionService[]
                {
                    new ArtifactResolutionService { Binding = ProtocolBindings.ArtifactSoap, Index = _configuration.ArtifactResolutionService.Index,Location = new Uri($"{issuer}/{Saml.Idp.Constants.RouteNames.SingleSignOnArtifact}") }
                },
                NameIDFormats = new Uri[] { NameIdentifierFormats.X509SubjectName },
                Attributes = new SamlAttribute[]
                {
                    new SamlAttribute("urn:oid:1.3.6.1.4.1.5923.1.1.1.6", friendlyName: "eduPersonPrincipalName"),
                    new SamlAttribute("urn:oid:1.3.6.1.4.1.5923.1.1.1.1", new string[] { "member", "student", "employee" })
                }
            };
            entityDescriptor.ContactPersons = new[] {
                new ContactPerson(ContactTypes.Administrative)
                {
                    Company = "Some Company",
                    GivenName = "Some Given Name",
                    SurName = "Some Sur Name",
                    EmailAddress = "some@some-domain.com",
                    TelephoneNumber = "11111111",
                },
                new ContactPerson(ContactTypes.Technical)
                {
                    Company = "Some Company",
                    GivenName = "Some tech Given Name",
                    SurName = "Some tech Sur Name",
                    EmailAddress = "sometech@some-domain.com",
                    TelephoneNumber = "22222222",
                }
            };
            return new Saml2Metadata(entityDescriptor).CreateMetadata().ToActionResult();
        }
    }
}
