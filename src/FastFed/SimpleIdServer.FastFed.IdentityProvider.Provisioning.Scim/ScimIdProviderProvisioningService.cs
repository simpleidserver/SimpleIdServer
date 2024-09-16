// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading.Tasks;

namespace SimpleIdServer.FastFed.IdentityProvider.Provisioning.Scim;

public class ScimIdProviderProvisioningService : IIdProviderProvisioningService
{
    public string Name => Constants.ProvisioningProfileName;
    
    public string Area => Constants.Areas.Scim;
    
    public async Task Add()
    {
        // when a user is added in the IdServer.
        // save the representation the entire representation.
        // each 30 minutes, transform the representation into SCIM (according to the grammar from application provider).
        // for each configured application provider
        // // call the provisioning profile service (scim).
        // support "desired_attributes" in application provider.
    }
}
