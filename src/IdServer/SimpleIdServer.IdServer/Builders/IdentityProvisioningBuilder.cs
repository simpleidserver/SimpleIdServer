// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using System;

namespace SimpleIdServer.IdServer.Builders
{
    public class IdentityProvisioningBuilder
    {
        private IdentityProvisioning _identityProvisioning;

        private IdentityProvisioningBuilder(IdentityProvisioning identityProvisioning)
        {
            _identityProvisioning = identityProvisioning;
        }

        public static IdentityProvisioningBuilder Create(IdentityProvisioningDefinition definition, string name, string description, Domains.Realm realm = null)
        {
            var result = new IdentityProvisioning
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Definition = definition
            };
            if (realm == null) result.Realms.Add(Constants.StandardRealms.Master);
            else result.Realms.Add(realm);
            return new IdentityProvisioningBuilder(result);
        }

        public IdentityProvisioning Build() => _identityProvisioning;
    }
}
