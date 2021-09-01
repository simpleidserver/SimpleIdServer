// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using System;

namespace SimpleIdServer.Scim.DTOs
{
    public class ProvisioningConfigurationHistoryResult
    {
        public string ProvisioningConfigurationId { get; set; }
        public string ProvisioningConfigurationResourceType { get; set; }
        public ProvisioningConfigurationTypes ProvisiongConfigurationType { get; set; }
        public string RepresentationId { get; set; }
        public int RepresentationVersion { get; set; }
        public DateTime ExecutionDateTime { get; set; }
        public string Description { get; set; }
        public string WorkflowInstanceId { get; set; }
        public string WorkflowId { get; set; }
        public string Exception { get; set; }
        public ProvisioningConfigurationHistoryStatus Status { get; set; }

        public static ProvisioningConfigurationHistoryResult ToDto(ProvisioningConfigurationHistory history)
        {
            var result = new ProvisioningConfigurationHistoryResult
            {
                Exception = history.Exception,
                ExecutionDateTime = history.ExecutionDateTime,
                RepresentationId = history.RepresentationId,
                RepresentationVersion = history.RepresentationVersion,
                Status = history.Status,
                Description = history.Description,
                WorkflowInstanceId = history.WorkflowInstanceId,
                WorkflowId = history.WorkflowId
            };
            if (history.ProvisioningConfiguration != null)
            {
                result.ProvisiongConfigurationType = history.ProvisioningConfiguration.Type;
                result.ProvisioningConfigurationId = history.ProvisioningConfiguration.Id;
                result.ProvisioningConfigurationResourceType = history.ProvisioningConfiguration.ResourceType;
            }

            return result;
        }
    }
}
