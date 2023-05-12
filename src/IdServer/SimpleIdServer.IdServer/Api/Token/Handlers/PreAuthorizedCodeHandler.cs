// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Api.Token.Helpers;
using SimpleIdServer.IdServer.Options;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token.Handlers
{
    /// <summary>
    /// The code representing the Credential Issuer's authorization for the Wallet to obtain Credentials of a certain type.
    /// </summary>
    public class PreAuthorizedCodeHandler : BaseCredentialsHandler
    {
        public PreAuthorizedCodeHandler(
            IClientAuthenticationHelper clientAuthenticationHelper,
            IOptions<IdServerHostOptions> options) : base(clientAuthenticationHelper, options)
        {
        }

        public override string GrantType => GRANT_TYPE;
        public static string GRANT_TYPE = "urn:ietf:params:oauth:grant-type:pre-authorized_code";

        public override Task<IActionResult> Handle(HandlerContext context, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
