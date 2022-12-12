// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Store;

namespace SimpleIdServer.OAuth.Api.Management
{
    [Route(Constants.EndPoints.Management)]
    public partial class ManagementController : Controller
    {
        private readonly IClientRepository _clientRepository;

        public ManagementController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
    }
}