// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using SimpleIdServer.FastFed.IdentityProvider.Services;
using System.Collections.Generic;

namespace SimpleIdServer.FastFed.IdentityProvider.Authentication.Saml;

public class SamlAuthenticationFastFedEnricher : IFastFedEnricher
{
    private readonly FastFedSamlAuthenticationOptions _options;

    public SamlAuthenticationFastFedEnricher(IOptions<FastFedSamlAuthenticationOptions> options)
    {
        _options = options.Value;
    }

    public void EnrichFastFedRequest(Dictionary<string, object> dic)
    {
        var samlEntreprise = new Dictionary<string, object>
        {
            { SimpleIdServer.FastFed.Authentication.Saml.Constants.SamlMetadataUri, _options.SamlMetadataUri }
        };
        dic.Add(SimpleIdServer.FastFed.Authentication.Saml.Constants.SamlAuthentication, samlEntreprise);
    }
}
