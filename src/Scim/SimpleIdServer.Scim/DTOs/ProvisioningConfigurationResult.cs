// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Domains;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.DTOs
{
    public class ProvisioningConfigurationResult
    {
        public ProvisioningConfigurationResult()
        {
            Records = new List<ProvisioningConfigurationRecordResult>();
        }

        public string Id { get; set; }
        public ProvisioningConfigurationTypes Type { get; set; }
        public string ResourceType { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<ProvisioningConfigurationRecordResult> Records { get; set; }

        public static ProvisioningConfigurationResult ToDto(ProvisioningConfiguration provisioning)
        {
            return new ProvisioningConfigurationResult
            {
                Id = provisioning.Id,
                ResourceType = provisioning.ResourceType,
                Type = provisioning.Type,
                UpdateDateTime = provisioning.UpdateDateTime,
                Records = provisioning.Records.Select(r => ProvisioningConfigurationRecordResult.ToDto(r)).ToList()
            };
        }
    }
}
