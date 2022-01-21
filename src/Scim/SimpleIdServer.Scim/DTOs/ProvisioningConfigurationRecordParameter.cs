// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domains;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.DTOs
{
    public class ProvisioningConfigurationRecordParameter
    {
        public ProvisioningConfigurationRecordParameter()
        {
            Values = new List<ProvisioningConfigurationRecordParameter>();
        }

        public string Name { get; set; }
        public ProvisioningConfigurationRecordTypes Type { get; set; }
        public bool IsArray { get; set; }
        public ICollection<string> ValuesString { get; set; }
        public virtual ICollection<ProvisioningConfigurationRecordParameter> Values { get; set; }

        public ProvisioningConfigurationRecord ToDomain()
        {
            return new ProvisioningConfigurationRecord
            {
                IsArray = IsArray,
                Type = Type,
                Name = Name,
                Values = Values.Select(v => v.ToDomain()).ToList(),
                ValuesString = ValuesString
            };
        }
    }
}
