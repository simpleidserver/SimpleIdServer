// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.FastFed.Models;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.Authentication.Saml;

public class SamlAuthenticationProvisioningService : IAppProviderProvisioningService
{
    private readonly SamlAuthenticationOptions _options;

    public SamlAuthenticationProvisioningService(IOptions<SamlAuthenticationOptions> options)
    {
        _options = options.Value;
    }

    public string Name => FastFed.Authentication.Saml.Constants.ProvisioningProfileName;

    public string RegisterConfigurationName => FastFed.Authentication.Saml.Constants.SamlAuthentication;

    public Task<JsonObject> EnableCapability(IdentityProviderFederation identityProviderFederation, JsonWebToken jwt, CancellationToken cancellationToken)
    {
        var result = new JsonObject
        {
            { FastFed.Authentication.Saml.Constants.SamlMetadataUri, _options.SamlMetadataUri }
        };
        return Task.FromResult(result);
    }
}