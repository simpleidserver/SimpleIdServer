// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Credential
{
    public class CredentialController : BaseController
    {
        public CredentialController()
        {

        }

        public async Task<IActionResult> Post([FromBody] CredentialRequest request, CancellationToken cancellationToken)
        {
            // generate a DID from block chain...
            return null;
        }
    }
}
