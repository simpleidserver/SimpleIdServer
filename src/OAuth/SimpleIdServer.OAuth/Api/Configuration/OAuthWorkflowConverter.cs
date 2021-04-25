// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.OAuth.Api.Configuration
{
    public class OAuthWorkflowConverter : IOAuthWorkflowConverter
    {
        public bool TryGetWorkflow(IEnumerable<string> responseTypes, out string workflowName)
        {
            workflowName = null;
            var oauthWorkflows = GetOAuthWorkflows();
            var oauthWorkflow = oauthWorkflows.FirstOrDefault(w => w.ResponseTypes.OrderBy(r => r).SequenceEqual(responseTypes.OrderBy(r => r)));
            if (oauthWorkflow == null)
            {
                return false;
            }

            workflowName = oauthWorkflow.WorkflowName;
            return true;
        }

        public virtual List<OAuthWorkflow> GetOAuthWorkflows()
        {
            return new List<OAuthWorkflow>
            {
                new OAuthWorkflow("code", new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE }),
                new OAuthWorkflow("hybrid", new List<string> { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE, TokenResponseTypeHandler.RESPONSE_TYPE })
            };
        }
    }
}
