// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.FastFed.Authentication.Saml;
using SimpleIdServer.FastFed.Models;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Authentication.Saml;

public class SamlIdProviderProvisioningService : IIdProviderProvisioningService
{
    private readonly ISamlClientProvisioningService _samlClientProvisioningService;

    public SamlIdProviderProvisioningService(ISamlClientProvisioningService samlClientProvisioningService)
    {
        _samlClientProvisioningService = samlClientProvisioningService;
    }

    public string Name => FastFed.Authentication.Saml.Constants.ProvisioningProfileName;

    public string RegisterConfigurationName => FastFed.Authentication.Saml.Constants.SamlAuthentication;

    public string Area => null;

    public Task<MigrationResult> Migrate(ProvisioningProfileHistory provisioningProfileHistory, CapabilitySettings settings, CancellationToken cancellationToken)
    {
        return Task.FromResult(new MigrationResult());
    }

    public async Task EnableCapability(IdentityProviderFederation identityProviderFederation, CancellationToken cancellationToken)
    {
        var configuration = identityProviderFederation.LastCapabilities.Configurations.Single(c => c.ProfileName == FastFed.Authentication.Saml.Constants.ProvisioningProfileName);
        var jObj = JsonObject.Parse(configuration.AppProviderHandshakeRegisterConfiguration).AsObject();
        var mappings = JsonSerializer.Deserialize<SamlEntrepriseMappingsResult>(configuration.AppProviderConfiguration);
        var samlMetadataUri = jObj[SimpleIdServer.FastFed.Authentication.Saml.Constants.SamlMetadataUri].ToString();
        await _samlClientProvisioningService.Provision(identityProviderFederation.EntityId, samlMetadataUri, mappings, cancellationToken);
    }
}
