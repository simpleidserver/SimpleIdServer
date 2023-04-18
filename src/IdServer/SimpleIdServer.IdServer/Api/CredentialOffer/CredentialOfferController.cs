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
            // Chaque utilisateur possède un credential offer.
            // Ce credential offer est retourné au wallet.
            // Le wallet va ensuite continuer la suite afin de récupérer ces credentials.
            // Return the list of credentials supported by the Identity Provider.
            var parameter = new CredentialOfferParameter();
            return null;
        } 
    }
}
