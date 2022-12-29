// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OAuth.Api.DTOs.Requests;
using SimpleIdServer.Store;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management
{
    public class ClientManagementController : Controller
    {
        private readonly IClientRepository _clientRepository;

        public ClientManagementController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddClientRequest jObj)
        {
            return new OkObjectResult("coucou");
        }
    }
}