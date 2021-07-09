// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.Scim.DTOs
{
    public class UpdateProvisioningConfigurationParameter
    {
        public UpdateProvisioningConfigurationParameter()
        {
            Records = new List<ProvisioningConfigurationRecordParameter>();
        }

        public ICollection<ProvisioningConfigurationRecordParameter> Records { get; set; }
    }
}
