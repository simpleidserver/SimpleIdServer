// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.OAuth.Api.Management.Requests;
using SimpleIdServer.Store;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management
{
    [Route(Constants.EndPoints.Management)]
    public partial class ManagementController : Controller
    {
        private readonly IClientRepository _clientRepository;

        public ManagementController(
            IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        #region Manage clients

        [HttpPost("clients/.search")]
        [Authorize("ManageClients")]
        public virtual Task<IActionResult> SearchClientsPost([FromBody] SearchClientsRequest request, CancellationToken cancellationToken) => InternalSearchClients(request, cancellationToken);

        [HttpGet("clients/.search")]
        [Authorize("ManageClients")]
        public virtual Task<IActionResult> SearchClientsQuery([FromQuery] SearchClientsRequest request, CancellationToken cancellationToken) => InternalSearchClients(request, cancellationToken);

        [HttpGet("clients/{id}")]
        [Authorize("ManageClients")]
        public virtual async Task<IActionResult> GetClient(string id, CancellationToken cancellationToken)
        {
            var client = await _clientRepository.Query().AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == id, cancellationToken);
            if (client == null) return new NotFoundResult();
            return new OkObjectResult(client);
        }

        [HttpDelete("clients/{id}")]
        [Authorize("ManageClients")]
        public virtual async Task<IActionResult> DeleteClient(string id, CancellationToken cancellationToken)
        {
            var client = await _clientRepository.Query().FirstOrDefaultAsync(c => c.ClientId == id, cancellationToken);
            if (client == null) return new NotFoundResult();
            _clientRepository.Delete(client);
            await _clientRepository.SaveChanges(cancellationToken);
            return new NoContentResult();
        }

        #endregion
    }
}