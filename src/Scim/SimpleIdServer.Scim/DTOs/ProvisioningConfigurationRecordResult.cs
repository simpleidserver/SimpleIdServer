// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.DTOs
{
    public class ProvisioningConfigurationRecordResult
    {
        public ProvisioningConfigurationRecordResult()
        {
            ValuesString = new List<string>();
            Values = new List<ProvisioningConfigurationRecordResult>();
        }

        public string Name { get; set; }
        public ProvisioningConfigurationRecordTypes Type { get; set; }
        public bool IsArray { get; set; }
        public ICollection<string> ValuesString { get; set; }
        public ICollection<ProvisioningConfigurationRecordResult> Values { get; set; }

        public static ProvisioningConfigurationRecordResult ToDto(ProvisioningConfigurationRecord record)
        {
            return new ProvisioningConfigurationRecordResult
            {
                IsArray = record.IsArray,
                Name = record.Name,
                Type = record.Type,
                ValuesString = record.ValuesString,
                Values = record.Values.Select(v => ToDto(v)).ToList()
            };
        }
    }
}
