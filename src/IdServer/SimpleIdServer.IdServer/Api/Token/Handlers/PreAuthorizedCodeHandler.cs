// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Api.Token.Validators;
using SimpleIdServer.IdServer.Options;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    public class PreAuthorizedCodeHandler : BaseCredentialsHandler
    {
        private readonly IPreAuthorizedCodeGrantTypeValidator _validator;

        public PreAuthorizedCodeHandler(IPreAuthorizedCodeGrantTypeValidator validator, IClientAuthenticationHelper clientAuthenticationHelper, IOptions<IdServerHostOptions> options) : base(clientAuthenticationHelper, options)
        {
            _validator = validator;
        }

        public const string GRANT_TYPE = "urn:ietf:params:oauth:grant-type:pre-authorized_code";
        public override string GrantType => GRANT_TYPE;

        public override Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Get Token"))
            {
                activity?.SetTag("grant_type", GRANT_TYPE);
                activity?.SetTag("realm", context.Realm);
                _validator.Validate(context);
                var preAuthorizedCode = context.Request.RequestData.GetPreAuthorizedCode();
                var userPin = context.Request.RequestData.GetUserPin();
                // Credential issuer is waiting for the end-user integration to complete (must give his consent to access to the claims).
                // the access token represents the approval of the end-user.
            }

            return null;
        }
    }
}
