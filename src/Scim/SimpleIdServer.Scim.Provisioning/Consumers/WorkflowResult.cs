// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace SimpleIdServer.Scim.Provisioning.Consumers
{
    public class WorkflowResult
    {
        public WorkflowResult(string instanceId, string fileId)
        {
            InstanceId = instanceId;
            FileId = fileId;
        }

        public string InstanceId { get; set; }
        public string FileId { get; set; }
    }
}
