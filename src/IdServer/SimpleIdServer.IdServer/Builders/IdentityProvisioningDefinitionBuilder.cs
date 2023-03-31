// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Serializer;
using System;
using System.Linq;

namespace SimpleIdServer.IdServer.Builders
{
    public class IdentityProvisioningDefinitionBuilder
    {
        private IdentityProvisioningDefinition _idProvisioningDef;

        private IdentityProvisioningDefinitionBuilder(IdentityProvisioningDefinition idProvisioningDef)
        {
            _idProvisioningDef = idProvisioningDef;
        }

        public static IdentityProvisioningDefinitionBuilder Create<T>(string name, string description)
        {
            var properties = PropertiesSerializer.SerializePropertyDefinitions<IdentityProvisioningDefinitionProperty>(typeof(T));
            return new IdentityProvisioningDefinitionBuilder(new IdentityProvisioningDefinition
            {
                Name = name,
                Description = description,
                Properties = properties.ToList()
            });
        }

        public IdentityProvisioningDefinition Build() => _idProvisioningDef;
    }
}
