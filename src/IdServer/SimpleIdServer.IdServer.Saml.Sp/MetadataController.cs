// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace SimpleIdServer.IdServer.Saml.Sp;

[Route("Metadata")]
public class MetadataController : Controller
{
    private readonly SamlSpOptions _options;

    public MetadataController(IOptions<SamlSpOptions> options)
    {
        _options = options.Value;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var issuer = GetAbsoluteUriWithVirtualPath();
        var entityDescriptor = new EntityDescriptor(new Saml2Configuration
        {
            Issuer = issuer
        });
        entityDescriptor.ValidUntil = 365;
        entityDescriptor.SPSsoDescriptor = new SPSsoDescriptor
        {
            WantAssertionsSigned = _options.SigningCertificate != null,
            SigningCertificates = new X509Certificate2[]
            {
                _options.SigningCertificate
            },
            AssertionConsumerServices = new AssertionConsumerService[]
            {
                new AssertionConsumerService {  Binding = ProtocolBindings.HttpPost, Location = new Uri(issuer + "/signin-saml2") },
                new AssertionConsumerService {  Binding = ProtocolBindings.HttpArtifact, Location = new Uri(issuer + "/signin-saml2") }
            },
            AttributeConsumingServices = new AttributeConsumingService[]
            {
            },
            SingleLogoutServices = new SingleLogoutService[]
            {
                new SingleLogoutService { Binding = ProtocolBindings.HttpPost, Location = new Uri(issuer + "/LoggedOut") }
            }
        };
        entityDescriptor.ContactPersons = _options.ContactPersons;        
        return new Saml2Metadata(entityDescriptor).CreateMetadata().ToActionResult();
    }

    private string GetAbsoluteUriWithVirtualPath()
    {
        var host = Request.Host.Value;
        var http = "http://";
        if (Request.IsHttps) http = "https://";
        var relativePath = Request.PathBase.Value;
        return http + host + relativePath;
    }

    private IEnumerable<RequestedAttribute> CreateRequestedAttributes()
    {
        yield return new RequestedAttribute("urn:oid:2.5.4.4");
        yield return new RequestedAttribute("urn:oid:2.5.4.3", false);
    }
}
