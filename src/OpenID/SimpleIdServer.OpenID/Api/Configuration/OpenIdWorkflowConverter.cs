// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.OAuth.Api.Authorization.ResponseTypes;
using SimpleIdServer.OAuth.Api.Configuration;
using System.Collections.Generic;

namespace SimpleIdServer.OpenID.Api.Configuration
{
    public class OpenIdWorkflowConverter : OAuthWorkflowConverter
    {
        public override List<OAuthWorkflow> GetOAuthWorkflows()
        {
            var workflows = base.GetOAuthWorkflows();
            workflows.AddRange(new List<OAuthWorkflow>
            {
                new OAuthWorkflow("implicit", new string[] { Authorization.ResponseTypes.IdTokenResponseTypeHandler.RESPONSE_TYPE }),
                new OAuthWorkflow("implicit", new string[] { Authorization.ResponseTypes.IdTokenResponseTypeHandler.RESPONSE_TYPE, TokenResponseTypeHandler.RESPONSE_TYPE }),
                new OAuthWorkflow("hybrid", new string[] { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE, Api.Authorization.ResponseTypes.IdTokenResponseTypeHandler.RESPONSE_TYPE }),
                new OAuthWorkflow("hybrid", new string[] { AuthorizationCodeResponseTypeHandler.RESPONSE_TYPE, Api.Authorization.ResponseTypes.IdTokenResponseTypeHandler.RESPONSE_TYPE, TokenResponseTypeHandler.RESPONSE_TYPE })
            });
            return workflows;
        }
    }
}
