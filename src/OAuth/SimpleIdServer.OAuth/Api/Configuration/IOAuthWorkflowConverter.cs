// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Collections.Generic;

namespace SimpleIdServer.OAuth.Api.Configuration
{
    public interface IOAuthWorkflowConverter
    {
        bool TryGetWorkflow(IEnumerable<string> responseTypes, out string workflowName);
        List<OAuthWorkflow> GetOAuthWorkflows();
    }
}
