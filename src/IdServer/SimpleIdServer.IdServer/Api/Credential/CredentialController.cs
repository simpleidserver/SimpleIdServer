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
            // 1. Stocker la demande d'accès au VC d'un utilisateur connecté.
            // 2. retourner les credentials du "credential offer". chaque credential possède son propre DID.

            // communication MUST utilize TLS.
            // access token represents te approval.
            // Issued Credential SHOULD be cryptographically bound to the identifier of the End-User who posses the Credential.
            // Claims in the Credential are usually about the End-User who possesses it.
            // Publish the DID to the blockchain in order to issue to a wallet.
            return null;
        }
    }
}
