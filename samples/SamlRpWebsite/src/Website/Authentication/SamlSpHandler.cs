// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using Website.Authentication;

namespace Microsoft.AspNetCore.Authentication;

public class SamlSpHandler : RemoteAuthenticationHandler<SamlSpOptions>
{
    private EntityDescriptor _entityDescriptor;

    public SamlSpHandler(IOptionsMonitor<SamlSpOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected new SamlSpEvents Events
    {
        get { return (SamlSpEvents)base.Events; }
        set { base.Events = value; }
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var entityDescriptor = await GetEntityDescriptor(CancellationToken.None);
        var binding = new Saml2RedirectBinding();
        var saml2Configuration = new Saml2Configuration
        {
            Issuer = Options.SPId,
            AllowedIssuer = entityDescriptor.EntityId,
            SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location,
            SingleLogoutDestination = entityDescriptor.IdPSsoDescriptor.SingleLogoutServices.First().Location,
            SigningCertificate = Options.SigningCertificate,
            SignAuthnRequest = Options.SigningCertificate != null
        };
        binding.Bind(new Saml2AuthnRequest(saml2Configuration));
        var redirectionContext = new RedirectContext<SamlSpOptions>(
                Context,
                Scheme,
                Options,
                properties,
                binding.RedirectLocation.OriginalString);
        await Events.RedirectToSsoEndpoint(redirectionContext);
    }

    protected override Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        throw new NotImplementedException();
    }

    private async Task<EntityDescriptor> GetEntityDescriptor(CancellationToken cancellationToken)
    {
        if (_entityDescriptor != null) return _entityDescriptor;
        var httpClient = Options.Backchannel;
        
        var httpResponse = await httpClient.GetAsync(Options.IdpMetadataUrl, cancellationToken);
        var xml = await httpResponse.Content.ReadAsStringAsync();
        _entityDescriptor = new EntityDescriptor();
        _entityDescriptor = _entityDescriptor.ReadIdPSsoDescriptor(xml);
        return _entityDescriptor;
    }
}
