// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.CredentialOffer
{
    public class CredentialOfferController : BaseController
    {
        public CredentialOfferController() { }

        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            // Chaque utilisateur possède son propre "wallet".
            // Il peut choisir d'exposer un ou plusieurs credentials en générant une offre (credential offer) : smartphone va scanner le QR Code.
            // Lorsque ce QRCode est scanné alors un code "pre-auth" et un code PIN peut être envoyé à l'utilisateur connecté sur le site. C'est l'utilisateur du wallet qui doit rentrer ce code PIN.
            // Tous ces paramètres sont ensuite transférés au token edp.
            var parameter = new CredentialOfferParameter();
            return null;
        } 
    }
}
