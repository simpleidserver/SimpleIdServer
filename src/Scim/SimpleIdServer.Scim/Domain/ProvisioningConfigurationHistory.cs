// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Scim.Domain
{
    public class ProvisioningConfigurationHistory : ICloneable
    {
        public string RepresentationId { get; set; }
        public int RepresentationVersion { get; set; }
        public DateTime ExecutionDateTime { get; set; }
        public string Exception { get; set; }
        public ProvisioningConfigurationHistoryStatus Status { get; set; }

        public static ProvisioningConfigurationHistory Complete(string representationId, int version)
        {
            return new ProvisioningConfigurationHistory
            {
                RepresentationId = representationId,
                RepresentationVersion = version,
                ExecutionDateTime = DateTime.UtcNow,
                Status = ProvisioningConfigurationHistoryStatus.FINISHED
            };
        }

        public static ProvisioningConfigurationHistory Error(string representation, int version, string exception)
        {
            return new ProvisioningConfigurationHistory
            {
                RepresentationId = representation,
                RepresentationVersion = version,
                ExecutionDateTime = DateTime.UtcNow,
                Status = ProvisioningConfigurationHistoryStatus.EXCEPTION
            };
        }

        public object Clone()
        {
            return new ProvisioningConfigurationHistory
            {
                RepresentationId = RepresentationId,
                RepresentationVersion = RepresentationVersion,
                ExecutionDateTime = ExecutionDateTime,
                Exception = Exception,
                Status = Status
            };
        }
    }
}
