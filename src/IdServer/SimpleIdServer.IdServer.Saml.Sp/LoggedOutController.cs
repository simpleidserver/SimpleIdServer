// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SimpleIdServer.IdServer.Saml.Sp;

[Route("LoggedOut")]
public class LoggedOutController : Controller
{
    private readonly SamlSpOptions _options;

    public LoggedOutController(IOptions<SamlSpOptions> options)
    {
        _options = options.Value;
    }

    [HttpPost]
    public async Task<IActionResult> Get()
    {
        var config = await GetIdpConfiguration();
        var httpRequest = Request.ToGenericHttpRequest(validate: true);
        httpRequest.Binding.Unbind(httpRequest, new Saml2LogoutResponse(config));
        return Redirect(Url.Content("~/"));
    }

    private async Task<Saml2Configuration> GetIdpConfiguration()
    {
        var entityDescriptor = await GetEntityDescriptor(CancellationToken.None);
        var signingCertificate = entityDescriptor.IdPSsoDescriptor.SigningCertificates.First();
        var saml2Configuration = new Saml2Configuration
        {
            Issuer = _options.SPId,
            AllowedIssuer = entityDescriptor.EntityId,
            SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location,
            SingleLogoutDestination = entityDescriptor.IdPSsoDescriptor.SingleLogoutServices.First().Location,
            SignAuthnRequest = signingCertificate != null,
            CertificateValidationMode = _options.CertificateValidationMode,
            RevocationMode = _options.RevocationMode
        };
        saml2Configuration.SignatureValidationCertificates.Add(signingCertificate);
        return saml2Configuration;
    }

    private async Task<EntityDescriptor> GetEntityDescriptor(CancellationToken cancellationToken)
    {
        var httpClient = _options.Backchannel;
        var httpResponse = await httpClient.GetAsync(_options.IdpMetadataUrl, cancellationToken);
        var xml = await httpResponse.Content.ReadAsStringAsync();
        var entityDescriptor = new EntityDescriptor();
        entityDescriptor = entityDescriptor.ReadIdPSsoDescriptor(xml);
        return entityDescriptor;
    }
}
