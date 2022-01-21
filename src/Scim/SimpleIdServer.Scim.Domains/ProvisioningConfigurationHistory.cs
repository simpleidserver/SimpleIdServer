// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;

namespace SimpleIdServer.Scim.Domains
{
    public class ProvisioningConfigurationHistory : ICloneable
    {
        public string RepresentationId { get; set; }
        public int RepresentationVersion { get; set; }
        public string Description { get; set; }
        public string WorkflowInstanceId { get; set; }
        public string WorkflowId { get; set; }
        public DateTime ExecutionDateTime { get; set; }
        public string Exception { get; set; }
        public ProvisioningConfigurationHistoryStatus Status { get; set; }
        public virtual ProvisioningConfiguration ProvisioningConfiguration { get; set; }

        public static ProvisioningConfigurationHistory Complete(string representationId,string description, string workflowInstanceId, string workflowId, int version)
        {
            return new ProvisioningConfigurationHistory
            {
                RepresentationId = representationId,
                RepresentationVersion = version,
                Description = description,
                WorkflowInstanceId = workflowInstanceId,
                WorkflowId = workflowId,
                ExecutionDateTime = DateTime.UtcNow,
                Status = ProvisioningConfigurationHistoryStatus.FINISHED
            };
        }

        public static ProvisioningConfigurationHistory Error(string representation, string description, int version, string exception)
        {
            return new ProvisioningConfigurationHistory
            {
                RepresentationId = representation,
                RepresentationVersion = version,
                Description = description,
                Exception = exception,
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
                Status = Status,
                Description = Description,
                WorkflowInstanceId = WorkflowInstanceId,
                WorkflowId = WorkflowId
            };
        }
    }
}
