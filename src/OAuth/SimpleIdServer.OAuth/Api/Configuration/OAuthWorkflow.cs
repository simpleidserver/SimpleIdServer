// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Api.Configuration
{
    public class OAuthWorkflow
    {
        public OAuthWorkflow(string workflowName, IEnumerable<string> responseTypes)
        {
            WorkflowName = workflowName;
            ResponseTypes = responseTypes;
        }

        public string WorkflowName { get; set; }
        public IEnumerable<string> ResponseTypes { get; set; }
    }
}
