// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Domains
{
    public class ProvisioningConfigurationRecord : ICloneable
    {
        public ProvisioningConfigurationRecord()
        {
            ValuesString = new List<string>();
            Values = new List<ProvisioningConfigurationRecord>();
        }

        public string Name { get; set; }
        public ProvisioningConfigurationRecordTypes Type { get; set; }
        public bool IsArray { get; set; }
        public ICollection<string> ValuesString { get; set; }
        public virtual ICollection<ProvisioningConfigurationRecord> Values { get; set; }

        public object Clone()
        {
            return new ProvisioningConfigurationRecord
            {
                Name = Name,
                Type = Type,
                IsArray = IsArray,
                ValuesString = ValuesString,
                Values = Values.Select(v => (ProvisioningConfigurationRecord)v.Clone()).ToList()
            };
        }
    }
}
