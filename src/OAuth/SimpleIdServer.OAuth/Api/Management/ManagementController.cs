// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Domains;
using SimpleIdServer.Domains.DTOs;
using SimpleIdServer.OAuth.Api.Management.Requests;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.Store;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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