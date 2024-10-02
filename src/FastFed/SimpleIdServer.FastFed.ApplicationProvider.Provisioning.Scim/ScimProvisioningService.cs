// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.FastFed.ApplicationProvider.Provisioning.Scim.IntegrationEvents;
using SimpleIdServer.FastFed.Models;
using SimpleIdServer.FastFed.Provisioning.Scim;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.Provisioning.Scim;

public class ScimProvisioningService : IAppProviderProvisioningService
{
    private readonly ScimProvisioningOptions _options;
    private readonly IBusControl _busControl;

    public ScimProvisioningService(IOptions<ScimProvisioningOptions> options, IBusControl busControl)
    {
        _options = options.Value;
        _busControl = busControl;
    }

    public string Name => SimpleIdServer.FastFed.Provisioning.Scim.Constants.ProvisioningProfileName;

    public string RegisterConfigurationName => SimpleIdServer.FastFed.Provisioning.Scim.Constants.ProvisioningProfileName;

    public async Task<JsonObject> EnableCapability(IdentityProviderFederation identityProviderFederation, JsonWebToken jwt, CancellationToken cancellationToken)
    {
        var result = JsonObject.Parse(JsonSerializer.Serialize(new ScimEntrepriseRegistrationResult
        {
            ScimServiceUri = _options.ScimServiceUri,
            ProviderAuthenticationMethods = FastFed.Provisioning.Scim.Constants.JwtProfile,
            JwtProfile = new AuthenticationProfileResult
            {
                Scope = _options.Scope,
                TokenEndpoint = _options.TokenEndpoint
            }
        })).AsObject();
        await _busControl.Publish(new ScimClientCreatedIntegrationEvent
        {
            EntityId = identityProviderFederation.EntityId,
            Scope = _options.Scope
        }, cancellationToken);
        return result;
    }
}
