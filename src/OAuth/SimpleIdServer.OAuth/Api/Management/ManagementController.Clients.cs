// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleIdServer.Domains;
using SimpleIdServer.Domains.DTOs;
using SimpleIdServer.OAuth.Api.Management.Requests;
using SimpleIdServer.OAuth.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Management
{
    /*
    public partial class ClientManagementController
    {
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

        private async Task<IActionResult> InternalSearchClients(SearchClientsRequest request, CancellationToken cancellationToken)
        {
            var dic = new Dictionary<string, string>
            {
                { OAuthClientParameters.ClientId, "ClientId" },
                { OAuthClientParameters.UpdateDateTime, "UpdateDateTime" },
                { OAuthClientParameters.CreateDateTime, "CreateDateTime" }
            };
            var result = _clientRepository.Query().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(request.RegistrationAccessToken))
                result = result.Where(c => c.RegistrationAccessToken == request.RegistrationAccessToken);

            if (dic.ContainsKey(request.OrderBy))
                result = result.InvokeOrderBy(dic[request.OrderBy], request.Order);
            else
                result = result.OrderByDescending(r => r.UpdateDateTime);

            int total = await result.CountAsync(cancellationToken);
            var content = await result.Skip(request.StartIndex).Take(request.Count).ToListAsync(cancellationToken);
            return new OkObjectResult(new SearchResult<Client>
            {
                Content = content,
                StartIndex = request.StartIndex,
                Count = request.Count,
                TotalLength = total
            });
        }
    }
    */
}
