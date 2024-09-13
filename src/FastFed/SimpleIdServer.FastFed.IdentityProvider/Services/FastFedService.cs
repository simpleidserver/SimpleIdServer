// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SimpleIdServer.FastFed.IdentityProvider.Resources;
using SimpleIdServer.FastFed.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Services;

public interface IFastFedService
{

}

public class FastFedService : IFastFedService
{
    private readonly IProviderFederationStore _providerFederationStore;

    public FastFedService(IProviderFederationStore providerFederationStore)
    {
        _providerFederationStore = providerFederationStore;
    }

    public async Task<ValidationResult<string>> StartHandshakeRegistration(string entityId, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(entityId)) return ValidationResult<string>.Fail(ErrorCodes.InvalidRequest, string.Format(Global.MissingParameter, nameof(entityId)));
        var providerFederation = await _providerFederationStore.Get(entityId, cancellationToken);
        if (providerFederation == null) return ValidationResult<string>.Fail(ErrorCodes.InvalidRequest, string.Format(Global.UnknownProviderFederation, entityId));
        var lastCapabilities = providerFederation.LastCapabilities;
        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = DateTime.UtcNow,

        };
        // https://openid.net/specs/fastfed-scim-1_0.html#rfc.section.3.2.1.1 : Add jwks_uri
        var claims = new Dictionary<string, object>();
        if(lastCapabilities.ProvisioningProfiles.Any())
            claims.Add("provisioning_profiles", lastCapabilities.ProvisioningProfiles);
        securityTokenDescriptor.Claims = claims;
        var handler = new JsonWebTokenHandler();
        // TODO : SIGN !!!
        return null;
    }
}
