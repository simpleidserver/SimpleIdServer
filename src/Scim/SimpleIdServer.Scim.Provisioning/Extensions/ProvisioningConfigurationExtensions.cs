// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Provisioning.Extensions
{
    public static class ProvisioningConfigurationExtensions
    {
        public static string GetTargetUrl(this ProvisioningConfiguration provisioningConfiguration)
        {
            return provisioningConfiguration.Records.First(r => r.Name == "targetUrl").ValuesString.First();
        }

        public static string GetClientId(this ProvisioningConfiguration provisioningConfiguration)
        {
            return provisioningConfiguration.Records.First(r => r.Name == "clientId").ValuesString.First();
        }

        public static string GetClientSecret(this ProvisioningConfiguration provisioningConfiguration)
        {
            return provisioningConfiguration.Records.First(r => r.Name == "clientSecret").ValuesString.First();
        }

        public static ICollection<string> GetScopes(this ProvisioningConfiguration provisioningConfiguration)
        {
            return provisioningConfiguration.Records.First(r => r.Name == "scopes").ValuesString;
        }

        public static string GetTokenEndpoint(this ProvisioningConfiguration provisioningConfiguration)
        {
            return provisioningConfiguration.Records.First(r => r.Name == "tokenEdp").ValuesString.First();
        }

        public static Dictionary<string, string> GetMappingRules(this ProvisioningConfiguration provisioningConfiguration)
        {
            return new Dictionary<string, string>(provisioningConfiguration.Records.First(r => r.Name == "mapping").Values.Select(v => new KeyValuePair<string, string>(v.Name, v.ValuesString.First())));
        }
    }
}
