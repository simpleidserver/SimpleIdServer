// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.FastFed.ApplicationProvider.Provisioning.Scim.IntegrationEvents;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.ApplicationProvider.Provisioning.Scim;

public class ScimProvisioningService : IAppProviderProvisioningService
{
    public const string JwtProfile = "urn:ietf:params:fastfed:1.0:provider_authentication:oauth:2.0:jwt_profile";
    private readonly ScimProvisioningOptions _options;
    private readonly IBusControl _busControl;

    public ScimProvisioningService(IOptions<ScimProvisioningOptions> options, IBusControl busControl)
    {
        _options = options.Value;
        _busControl = busControl;
    }

    public string Name => "urn:ietf:params:fastfed:1.0:provisioning:scim:2.0:enterprise";

    public async Task<JsonObject> EnableCapability(string entityId, JsonWebToken jwt, CancellationToken cancellationToken)
    {
        var result = JsonObject.Parse(JsonSerializer.Serialize(new ScimEntrepriseRegistrationResult
        {
            ScimServiceUri = _options.ScimServiceUri,
            ProviderAuthenticationMethods = JwtProfile,
            JwtProfile = new AuthenticationProfileResult
            {
                Scope = _options.Scope,
                TokenEndpoint = _options.TokenEndpoint
            }
        })).AsObject();
        await _busControl.Publish(new ScimClientCreatedIntegrationEvent
        {
            EntityId = entityId,
            Scope = _options.Scope
        }, cancellationToken);
        return result;
    }
}
